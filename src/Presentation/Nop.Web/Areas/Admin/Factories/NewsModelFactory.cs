using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.News;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Html;
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

        protected CatalogSettings CatalogSettings { get; }
        protected IBaseAdminModelFactory BaseAdminModelFactory { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected ILanguageService LanguageService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INewsService NewsService { get; }
        protected INopHtmlHelper NopHtmlHelper { get; }
        protected IStoreMappingSupportedModelFactory StoreMappingSupportedModelFactory { get; }
        protected IStoreService StoreService { get; }
        protected IUrlRecordService UrlRecordService { get; }

        #endregion

        #region Ctor

        public NewsModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INewsService newsService,
            INopHtmlHelper nopHtmlHelper,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreService storeService,
            IUrlRecordService urlRecordService)
        {
            CatalogSettings = catalogSettings;
            CustomerService = customerService;
            BaseAdminModelFactory = baseAdminModelFactory;
            DateTimeHelper = dateTimeHelper;
            LanguageService = languageService;
            LocalizationService = localizationService;
            NewsService = newsService;
            NopHtmlHelper = nopHtmlHelper;
            StoreMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            StoreService = storeService;
            UrlRecordService = urlRecordService;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Prepare news content model
        /// </summary>
        /// <param name="newsContentModel">News content model</param>
        /// <param name="filterByNewsItemId">Filter by news item ID</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the news content model
        /// </returns>
        public virtual async Task<NewsContentModel> PrepareNewsContentModelAsync(NewsContentModel newsContentModel, int? filterByNewsItemId)
        {
            if (newsContentModel == null)
                throw new ArgumentNullException(nameof(newsContentModel));

            //prepare nested search models
            await PrepareNewsItemSearchModelAsync(newsContentModel.NewsItems);
            var newsItem = await NewsService.GetNewsByIdAsync(filterByNewsItemId ?? 0);
            await PrepareNewsCommentSearchModelAsync(newsContentModel.NewsComments, newsItem);

            return newsContentModel;
        }
        
        /// <summary>
        /// Prepare paged news item list model
        /// </summary>
        /// <param name="searchModel">News item search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the news item list model
        /// </returns>
        public virtual async Task<NewsItemListModel> PrepareNewsItemListModelAsync(NewsItemSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get news items
            var newsItems = await NewsService.GetAllNewsAsync(showHidden: true,
                storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
                title: searchModel.SearchTitle);

            //prepare list model
            var model = await new NewsItemListModel().PrepareToGridAsync(searchModel, newsItems, () =>
            {
                return newsItems.SelectAwait(async newsItem =>
                {
                    //fill in model values from the entity
                    var newsItemModel = newsItem.ToModel<NewsItemModel>();

                    //little performance optimization: ensure that "Full" is not returned
                    newsItemModel.Full = string.Empty;

                    //convert dates to the user time
                    if (newsItem.StartDateUtc.HasValue)
                        newsItemModel.StartDateUtc = await DateTimeHelper.ConvertToUserTimeAsync(newsItem.StartDateUtc.Value, DateTimeKind.Utc);
                    if (newsItem.EndDateUtc.HasValue)
                        newsItemModel.EndDateUtc = await DateTimeHelper.ConvertToUserTimeAsync(newsItem.EndDateUtc.Value, DateTimeKind.Utc);
                    newsItemModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(newsItem.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    newsItemModel.SeName = await UrlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, true, false);
                    newsItemModel.LanguageName = (await LanguageService.GetLanguageByIdAsync(newsItem.LanguageId))?.Name;
                    newsItemModel.ApprovedComments = await NewsService.GetNewsCommentsCountAsync(newsItem, isApproved: true);
                    newsItemModel.NotApprovedComments = await NewsService.GetNewsCommentsCountAsync(newsItem, isApproved: false);

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the news item model
        /// </returns>
        public virtual async Task<NewsItemModel> PrepareNewsItemModelAsync(NewsItemModel model, NewsItem newsItem, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (newsItem != null)
            {
                if (model == null)
                {
                    model = newsItem.ToModel<NewsItemModel>();
                    model.SeName = await UrlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, true, false);
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
            await BaseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, false);

            //prepare available stores
            await StoreMappingSupportedModelFactory.PrepareModelStoresAsync(model, newsItem, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare news comment search model
        /// </summary>
        /// <param name="searchModel">News comment search model</param>
        /// <param name="newsItem">News item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the news comment search model
        /// </returns>
        public virtual async Task<NewsCommentSearchModel> PrepareNewsCommentSearchModelAsync(NewsCommentSearchModel searchModel, NewsItem newsItem)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare "approved" property (0 - all; 1 - approved only; 2 - disapproved only)
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await LocalizationService.GetResourceAsync("Admin.ContentManagement.News.Comments.List.SearchApproved.All"),
                Value = "0"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await LocalizationService.GetResourceAsync("Admin.ContentManagement.News.Comments.List.SearchApproved.ApprovedOnly"),
                Value = "1"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await LocalizationService.GetResourceAsync("Admin.ContentManagement.News.Comments.List.SearchApproved.DisapprovedOnly"),
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the news comment list model
        /// </returns>
        public virtual async Task<NewsCommentListModel> PrepareNewsCommentListModelAsync(NewsCommentSearchModel searchModel, int? newsItemId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter comments
            var createdOnFromValue = searchModel.CreatedOnFrom == null ? null
                : (DateTime?)DateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await DateTimeHelper.GetCurrentTimeZoneAsync());
            var createdOnToValue = searchModel.CreatedOnTo == null ? null
                : (DateTime?)DateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
            var isApprovedOnly = searchModel.SearchApprovedId == 0 ? null : searchModel.SearchApprovedId == 1 ? true : (bool?)false;

            //get comments
            var comments = (await NewsService.GetAllCommentsAsync(newsItemId: newsItemId,
                approved: isApprovedOnly,
                fromUtc: createdOnFromValue,
                toUtc: createdOnToValue,
                commentText: searchModel.SearchText)).ToPagedList(searchModel);

            //prepare store names (to avoid loading for each comment)
            var storeNames = (await StoreService.GetAllStoresAsync())
                .ToDictionary(store => store.Id, store => store.Name);

            //prepare list model
            var model = await new NewsCommentListModel().PrepareToGridAsync(searchModel, comments, () =>
            {
                return comments.SelectAwait(async newsComment =>
                {
                    //fill in model values from the entity
                    var commentModel = newsComment.ToModel<NewsCommentModel>();

                    //convert dates to the user time
                    commentModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(newsComment.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    commentModel.NewsItemTitle = (await NewsService.GetNewsByIdAsync(newsComment.NewsItemId))?.Title;

                    if ((await CustomerService.GetCustomerByIdAsync(newsComment.CustomerId)) is Customer customer)
                    {
                        commentModel.CustomerInfo = (await CustomerService.IsRegisteredAsync(customer))
                            ? customer.Email
                            : await LocalizationService.GetResourceAsync("Admin.Customers.Guest");
                    }

                    commentModel.CommentText = NopHtmlHelper.FormatText(newsComment.CommentText, false, true, false, false, false, false);
                    commentModel.StoreName = storeNames.ContainsKey(newsComment.StoreId) ? storeNames[newsComment.StoreId] : "Deleted";

                    return commentModel;
                });
            });

            return model;
        }
        
        /// <summary>
        /// Prepare news item search model
        /// </summary>
        /// <param name="searchModel">News item search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the news item search model
        /// </returns>
        public virtual async Task<NewsItemSearchModel> PrepareNewsItemSearchModelAsync(NewsItemSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            await BaseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = CatalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion
    }
}