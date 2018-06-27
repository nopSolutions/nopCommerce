using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.News;
using Nop.Core.Html;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.News;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the news model factory implementation
    /// </summary>
    public partial class NewsModelFactory : INewsModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsService _newsService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public NewsModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INewsService newsService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreService storeService)
        {
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._dateTimeHelper = dateTimeHelper;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._newsService = newsService;
            this._storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            this._storeService = storeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare news content model
        /// </summary>
        /// <param name="newsContentModel">News content model</param>
        /// <param name="filterByNewsItemId">Filter by news item ID</param>
        /// <returns>News content model</returns>
        public virtual NewsContentModel PrepareNewsContentModel(NewsContentModel newsContentModel, int? filterByNewsItemId)
        {
            if (newsContentModel == null)
                throw new ArgumentNullException(nameof(newsContentModel));

            //prepare nested search models
            PrepareNewsItemSearchModel(newsContentModel.NewsItems);
            var newsItem = _newsService.GetNewsById(filterByNewsItemId ?? 0);
            PrepareNewsCommentSearchModel(newsContentModel.NewsComments, newsItem);

            return newsContentModel;
        }

        /// <summary>
        /// Prepare news item search model
        /// </summary>
        /// <param name="searchModel">News item search model</param>
        /// <returns>News item search model</returns>
        public virtual NewsItemSearchModel PrepareNewsItemSearchModel(NewsItemSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged news item list model
        /// </summary>
        /// <param name="searchModel">News item search model</param>
        /// <returns>News item list model</returns>
        public virtual NewsItemListModel PrepareNewsItemListModel(NewsItemSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get news items
            var newsItems = _newsService.GetAllNews(showHidden: true,
                storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new NewsItemListModel
            {
                Data = newsItems.Select(newsItem =>
                {
                    //fill in model values from the entity
                    var newsItemModel = newsItem.ToModel<NewsItemModel>();

                    //little performance optimization: ensure that "Full" is not returned
                    newsItemModel.Full = string.Empty;

                    //convert dates to the user time
                    if (newsItem.StartDateUtc.HasValue)
                        newsItemModel.StartDate = _dateTimeHelper.ConvertToUserTime(newsItem.StartDateUtc.Value, DateTimeKind.Utc);
                    if (newsItem.EndDateUtc.HasValue)
                        newsItemModel.EndDate = _dateTimeHelper.ConvertToUserTime(newsItem.EndDateUtc.Value, DateTimeKind.Utc);
                    newsItemModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(newsItem.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    newsItemModel.LanguageName = _languageService.GetLanguageById(newsItem.LanguageId)?.Name;
                    newsItemModel.ApprovedComments = _newsService.GetNewsCommentsCount(newsItem, isApproved: true);
                    newsItemModel.NotApprovedComments = _newsService.GetNewsCommentsCount(newsItem, isApproved: false);

                    return newsItemModel;
                }),
                Total = newsItems.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare news item model
        /// </summary>
        /// <param name="model">News item model</param>
        /// <param name="newsItem">News item</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>News item model</returns>
        public virtual NewsItemModel PrepareNewsItemModel(NewsItemModel model, NewsItem newsItem, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (newsItem != null)
            {
                model = model ?? newsItem.ToModel<NewsItemModel>();

                model.StartDate = newsItem.StartDateUtc;
                model.EndDate = newsItem.EndDateUtc;
            }

            //set default values for the new model
            if (newsItem == null)
            {
                model.Published = true;
                model.AllowComments = true;
            }

            //prepare available languages
            _baseAdminModelFactory.PrepareLanguages(model.AvailableLanguages, false);

            //prepare available stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, newsItem, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare news comment search model
        /// </summary>
        /// <param name="searchModel">News comment search model</param>
        /// <param name="newsItem">News item</param>
        /// <returns>News comment search model</returns>
        public virtual NewsCommentSearchModel PrepareNewsCommentSearchModel(NewsCommentSearchModel searchModel, NewsItem newsItem)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare "approved" property (0 - all; 1 - approved only; 2 - disapproved only)
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.ContentManagement.News.Comments.List.SearchApproved.All"),
                Value = "0"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.ContentManagement.News.Comments.List.SearchApproved.ApprovedOnly"),
                Value = "1"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.ContentManagement.News.Comments.List.SearchApproved.DisapprovedOnly"),
                Value = "2"
            });

            searchModel.NewsItemId = newsItem?.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged news comment list model
        /// </summary>
        /// <param name="searchModel">News comment search model</param>
        /// <param name="newsItemId">News item Id; pass null to prepare comment models for all news items</param>
        /// <returns>News comment list model</returns>
        public virtual NewsCommentListModel PrepareNewsCommentListModel(NewsCommentSearchModel searchModel, int? newsItemId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter comments
            var createdOnFromValue = searchModel.CreatedOnFrom == null ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);
            var createdOnToValue = searchModel.CreatedOnTo == null ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var isApprovedOnly = searchModel.SearchApprovedId == 0 ? null : searchModel.SearchApprovedId == 1 ? true : (bool?)false;

            //get comments
            var comments = _newsService.GetAllComments(newsItemId: newsItemId,
                approved: isApprovedOnly,
                fromUtc: createdOnFromValue,
                toUtc: createdOnToValue,
                commentText: searchModel.SearchText);

            //prepare store names (to avoid loading for each comment)
            var storeNames = _storeService.GetAllStores().ToDictionary(store => store.Id, store => store.Name);

            //prepare list model
            var model = new NewsCommentListModel
            {
                Data = comments.PaginationByRequestModel(searchModel).Select(newsComment =>
                {
                    //fill in model values from the entity
                    var commentModel = new NewsCommentModel
                    {
                        Id = newsComment.Id,
                        NewsItemId = newsComment.NewsItemId,
                        NewsItemTitle = newsComment.NewsItem.Title,
                        CustomerId = newsComment.CustomerId,
                        IsApproved = newsComment.IsApproved,
                        StoreId = newsComment.StoreId,
                        CommentTitle = newsComment.CommentTitle
                    };

                    //convert dates to the user time
                    commentModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(newsComment.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    commentModel.CustomerInfo = newsComment.Customer.IsRegistered()
                        ? newsComment.Customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    commentModel.CommentText = HtmlHelper.FormatText(newsComment.CommentText, false, true, false, false, false, false);
                    commentModel.StoreName = storeNames.ContainsKey(newsComment.StoreId) ? storeNames[newsComment.StoreId] : "Deleted";

                    return commentModel;
                }),
                Total = comments.Count
            };

            return model;
        }

        #endregion
    }
}