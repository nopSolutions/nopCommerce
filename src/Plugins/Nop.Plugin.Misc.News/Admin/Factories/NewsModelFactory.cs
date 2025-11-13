using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.News.Admin.Models;
using Nop.Plugin.Misc.News.Domain;
using Nop.Plugin.Misc.News.Services;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.News.Admin.Factories;

/// <summary>
/// Represents the news model factory implementation
/// </summary>
public class NewsModelFactory
{
    #region Fields

    private readonly CatalogSettings _catalogSettings;
    private readonly IBaseAdminModelFactory _baseAdminModelFactory;
    private readonly ICustomerService _customerService;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IHtmlFormatter _htmlFormatter;
    private readonly ILanguageService _languageService;
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
    private readonly IStoreContext _storeContext;
    private readonly IStoreService _storeService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly NewsService _newsService;

    #endregion

    #region Ctor

    public NewsModelFactory(CatalogSettings catalogSettings,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IHtmlFormatter htmlFormatter,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ISettingService settingService,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
        IStoreContext storeContext,
        IStoreService storeService,
        IUrlRecordService urlRecordService,
        NewsService newsService)
    {
        _catalogSettings = catalogSettings;
        _customerService = customerService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _dateTimeHelper = dateTimeHelper;
        _htmlFormatter = htmlFormatter;
        _languageService = languageService;
        _localizationService = localizationService;
        _settingService = settingService;
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
        _storeContext = storeContext;
        _storeService = storeService;
        _urlRecordService = urlRecordService;
        _newsService = newsService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare news configuration model
    /// </summary>
    /// <param name="model">News configuration model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news configuration model
    /// </returns>
    public async Task<ConfigurationModel> PrepareNewsConfigurationModelAsync(ConfigurationModel model = null)
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var newsSettings = await _settingService.LoadSettingAsync<NewsSettings>(storeId);

        //fill in model values from the entity
        model ??= newsSettings.ToSettingsModel<ConfigurationModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.Enabled, storeId);
        model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeId);
        model.NotifyAboutNewNewsComments_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.NotifyAboutNewNewsComments, storeId);
        model.ShowNewsOnMainPage_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.ShowNewsOnMainPage, storeId);
        model.MainPageNewsCount_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.MainPageNewsCount, storeId);
        model.NewsArchivePageSize_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.NewsArchivePageSize, storeId);
        model.ShowHeaderRssUrl_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.ShowHeaderRssUrl, storeId);
        model.NewsCommentsMustBeApproved_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.NewsCommentsMustBeApproved, storeId);
        //model.DisplayNewsFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.DisplayNewsFooterItem, storeId);
        model.ShowCaptchaOnNewsCommentPage_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.ShowCaptchaOnNewsCommentPage, storeId);
        model.SitemapIncludeNews_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.SitemapIncludeNews, storeId);

        return model;
    }

    /// <summary>
    /// Prepare news content model
    /// </summary>
    /// <param name="newsContentModel">News content model</param>
    /// <param name="filterByNewsItemId">Filter by news item ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news content model
    /// </returns>
    public async Task<NewsContentModel> PrepareNewsContentModelAsync(NewsContentModel newsContentModel, int? filterByNewsItemId)
    {
        ArgumentNullException.ThrowIfNull(newsContentModel);

        //prepare nested search models
        await PrepareNewsItemSearchModelAsync(newsContentModel.NewsItems);
        var newsItem = await _newsService.GetNewsByIdAsync(filterByNewsItemId ?? 0);
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
    public async Task<NewsItemListModel> PrepareNewsItemListModelAsync(NewsItemSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get news items
        var newsItems = await _newsService.GetAllNewsAsync(showHidden: true,
            storeId: searchModel.SearchStoreId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
            title: searchModel.SearchTitle);

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

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
                    newsItemModel.StartDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(newsItem.StartDateUtc.Value, DateTimeKind.Utc);
                if (newsItem.EndDateUtc.HasValue)
                    newsItemModel.EndDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(newsItem.EndDateUtc.Value, DateTimeKind.Utc);
                newsItemModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(newsItem.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                newsItemModel.SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, true, false);
                newsItemModel.LanguageName = languages.FirstOrDefault(language => language.Id == newsItem.LanguageId)?.Name;
                newsItemModel.ApprovedComments = await _newsService.GetNewsCommentsCountAsync(newsItem, isApproved: true);
                newsItemModel.NotApprovedComments = await _newsService.GetNewsCommentsCountAsync(newsItem, isApproved: false);

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
    public async Task<NewsItemModel> PrepareNewsItemModelAsync(NewsItemModel model, NewsItem newsItem, bool excludeProperties = false)
    {
        //fill in model values from the entity
        if (newsItem != null)
        {
            if (model == null)
            {
                model = newsItem.ToModel<NewsItemModel>();
                model.SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, true, false);
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
        await _baseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, false);

        //prepare available stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, newsItem, excludeProperties);

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
    public async Task<NewsCommentSearchModel> PrepareNewsCommentSearchModelAsync(NewsCommentSearchModel searchModel, NewsItem newsItem)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare "approved" property (0 - all; 1 - approved only; 2 - disapproved only)
        searchModel.AvailableApprovedOptions.Add(new()
        {
            Text = await _localizationService.GetResourceAsync("Plugins.Misc.News.Comments.List.SearchApproved.All"),
            Value = "0"
        });
        searchModel.AvailableApprovedOptions.Add(new()
        {
            Text = await _localizationService.GetResourceAsync("Plugins.Misc.News.Comments.List.SearchApproved.ApprovedOnly"),
            Value = "1"
        });
        searchModel.AvailableApprovedOptions.Add(new()
        {
            Text = await _localizationService.GetResourceAsync("Plugins.Misc.News.Comments.List.SearchApproved.DisapprovedOnly"),
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
    public async Task<NewsCommentListModel> PrepareNewsCommentListModelAsync(NewsCommentSearchModel searchModel, int? newsItemId)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter comments
        var createdOnFromValue = searchModel.CreatedOnFrom == null
            ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var createdOnToValue = searchModel.CreatedOnTo == null
            ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var isApprovedOnly = searchModel.SearchApprovedId == 0 ? null : searchModel.SearchApprovedId == 1 ? true : (bool?)false;

        //get comments
        var comments = (await _newsService.GetAllCommentsAsync(newsItemId: newsItemId,
            approved: isApprovedOnly,
            fromUtc: createdOnFromValue,
            toUtc: createdOnToValue,
            commentText: searchModel.SearchText)).ToPagedList(searchModel);

        //prepare store names (to avoid loading for each comment)
        var storeNames = (await _storeService.GetAllStoresAsync())
            .ToDictionary(store => store.Id, store => store.Name);

        var news = await _newsService.GetNewsByIdsAsync(comments.Select(comment => comment.NewsItemId).Distinct().ToArray());
        var customers = await _customerService.GetCustomersByIdsAsync(comments.Select(comment => comment.CustomerId).Distinct().ToArray());

        //prepare list model
        var model = await new NewsCommentListModel().PrepareToGridAsync(searchModel, comments, () =>
        {
            return comments.SelectAwait(async newsComment =>
            {
                //fill in model values from the entity
                var commentModel = newsComment.ToModel<NewsCommentModel>();

                //convert dates to the user time
                commentModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(newsComment.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                commentModel.NewsItemTitle = news.FirstOrDefault(item => item.Id == newsComment.NewsItemId)?.Title;

                if (customers.FirstOrDefault(customer => customer.Id == newsComment.CustomerId) is Customer author)
                {
                    commentModel.CustomerInfo = await _customerService.IsRegisteredAsync(author)
                        ? author.Email
                        : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                }

                commentModel.CommentText = _htmlFormatter.FormatText(newsComment.CommentText, false, true, false, false, false, false);
                commentModel.StoreName = storeNames.TryGetValue(newsComment.StoreId, out var value) ? value : "Deleted";

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
    public async Task<NewsItemSearchModel> PrepareNewsItemSearchModelAsync(NewsItemSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    #endregion
}