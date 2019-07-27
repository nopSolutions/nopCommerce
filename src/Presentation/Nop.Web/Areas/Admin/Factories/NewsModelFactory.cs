using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.News;
using Nop.Core.Html;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the news model factory implementation
    /// </summary>
    public partial class NewsModelFactory : INewsModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsService _newsService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public NewsModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INewsService newsService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreService storeService,
            IUrlRecordService urlRecordService)
        {
            _catalogSettings = catalogSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _dateTimeHelper = dateTimeHelper;
            _languageService = languageService;
            _localizationService = localizationService;
            _newsService = newsService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
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

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

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
            var model = new NewsItemListModel().PrepareToGrid(searchModel, newsItems, () =>
            {
                return newsItems.Select(newsItem =>
                {
                    //fill in model values from the entity
                    var newsItemModel = newsItem.ToModel<NewsItemModel>();

                    //little performance optimization: ensure that "Full" is not returned
                    newsItemModel.Full = string.Empty;

                    //convert dates to the user time
                    if (newsItem.StartDateUtc.HasValue)
                        newsItemModel.StartDateUtc = _dateTimeHelper.ConvertToUserTime(newsItem.StartDateUtc.Value, DateTimeKind.Utc);
                    if (newsItem.EndDateUtc.HasValue)
                        newsItemModel.EndDateUtc = _dateTimeHelper.ConvertToUserTime(newsItem.EndDateUtc.Value, DateTimeKind.Utc);
                    newsItemModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(newsItem.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    newsItemModel.SeName = _urlRecordService.GetSeName(newsItem, newsItem.LanguageId, true, false);
                    newsItemModel.LanguageName = _languageService.GetLanguageById(newsItem.LanguageId)?.Name;
                    newsItemModel.ApprovedComments = _newsService.GetNewsCommentsCount(newsItem, isApproved: true);
                    newsItemModel.NotApprovedComments = _newsService.GetNewsCommentsCount(newsItem, isApproved: false);

                    return newsItemModel;
                });
            });

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
                if (model == null)
                {
                    model = newsItem.ToModel<NewsItemModel>();
                    model.SeName = _urlRecordService.GetSeName(newsItem, newsItem.LanguageId, true, false);
                }

                model.StartDateUtc = newsItem.StartDateUtc;
                model.EndDateUtc = newsItem.EndDateUtc;
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
                commentText: searchModel.SearchText).ToPagedList(searchModel);

            //prepare store names (to avoid loading for each comment)
            var storeNames = _storeService.GetAllStores().ToDictionary(store => store.Id, store => store.Name);

            //prepare list model
            var model = new NewsCommentListModel().PrepareToGrid(searchModel, comments, () =>
            {
                return comments.Select(newsComment =>
                {
                    //fill in model values from the entity
                    var commentModel = newsComment.ToModel<NewsCommentModel>();

                    //convert dates to the user time
                    commentModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(newsComment.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    commentModel.NewsItemTitle = newsComment.NewsItem.Title;
                    commentModel.CustomerInfo = newsComment.Customer.IsRegistered()
                        ? newsComment.Customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    commentModel.CommentText = HtmlHelper.FormatText(newsComment.CommentText, false, true, false, false, false, false);
                    commentModel.StoreName = storeNames.ContainsKey(newsComment.StoreId) ? storeNames[newsComment.StoreId] : "Deleted";

                    return commentModel;
                });
            });

            return model;
        }

        #endregion
    }
}