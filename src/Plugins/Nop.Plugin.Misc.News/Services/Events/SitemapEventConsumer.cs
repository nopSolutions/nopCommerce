using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Misc.News.Domain;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Sitemap;

namespace Nop.Plugin.Misc.News.Services.Events;

/// <summary>
/// Represents the plugin event consumer
/// </summary>
public class SitemapEventConsumer : IConsumer<SitemapCreatedEvent>, IConsumer<ModelPreparedEvent<BaseNopModel>>
{
    #region Fields

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILanguageService _languageService;
    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IStoreContext _storeContext;
    private readonly IUrlRecordService _urlRecordService;
    private readonly LocalizationSettings _localizationSettings;
    private readonly NewsService _newsService;
    private readonly NewsSettings _newsSettings;

    #endregion

    #region Ctor

    public SitemapEventConsumer(IHttpContextAccessor httpContextAccessor,
        ILanguageService languageService,
        ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        LocalizationSettings localizationSettings,
        NewsService newsService,
        NewsSettings newsSettings)
    {
        _httpContextAccessor = httpContextAccessor;
        _languageService = languageService;
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _storeContext = storeContext;
        _urlRecordService = urlRecordService;
        _localizationSettings = localizationSettings;
        _newsService = newsService;
        _newsSettings = newsSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets localized URL with SEO code
    /// </summary>
    /// <param name="currentUrl">URL to add SEO code</param>
    /// <param name="lang">Language for localization</param>
    /// <returns>Localized URL with SEO code</returns>
    protected string GetLocalizedUrl(string currentUrl, Language lang)
    {
        if (string.IsNullOrEmpty(currentUrl))
            return null;

        var pathBase = _httpContextAccessor.HttpContext.Request.PathBase;

        //Extract server and path from url
        var scheme = new Uri(currentUrl).GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
        var path = new Uri(currentUrl).PathAndQuery;

        //Replace seo code
        var localizedPath = path
            .RemoveLanguageSeoCodeFromUrl(pathBase, true)
            .AddLanguageSeoCodeToUrl(pathBase, true, lang);

        return new Uri(new Uri(scheme), localizedPath).ToString();
    }

    /// <summary>
    /// Get news item URLs for the sitemap
    /// </summary>
    /// <param name="store">Store</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap URLs
    /// </returns>
    private async Task<IEnumerable<SitemapUrlModel>> GetNewsItemUrlsAsync(Store store)
    {
        var newsList = await _newsService.GetAllNewsAsync(storeId: store.Id);
        var storeProtocol = store.SslEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;

        return await newsList
            .SelectAwait(async news => await PrepareLocalizedSitemapUrlAsync(news, news.CreatedOnUtc, storeProtocol))
            .ToListAsync();
    }

    /// <summary>
    /// Return localized urls
    /// </summary>
    /// <param name="newsItem">A news item</param>
    /// <param name="dateTimeUpdatedOn">A time when URL was updated last time</param>
    /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task<SitemapUrlModel> PrepareLocalizedSitemapUrlAsync(NewsItem newsItem,
        DateTime? dateTimeUpdatedOn = null,
        string protocol = null)
    {
        var url = await _nopUrlHelper
            .RouteGenericUrlAsync(newsItem, protocol, ensureTwoPublishedLanguages: false);

        var store = await _storeContext.GetCurrentStoreAsync();

        var updatedOn = dateTimeUpdatedOn ?? DateTime.UtcNow;
        var languages = _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled
            ? await _languageService.GetAllLanguagesAsync(storeId: store.Id)
            : null;

        if (languages == null || languages.Count == 1)
            return new SitemapUrlModel(url, new List<string>(), UpdateFrequency.Weekly, updatedOn);

        if (await _languageService.GetLanguageByIdAsync(newsItem.LanguageId) is not Language language || !language.Published)
            return new SitemapUrlModel(url, new List<string>(), UpdateFrequency.Weekly, updatedOn);

        var localizedUrl = await _nopUrlHelper
            .RouteGenericUrlAsync(newsItem, protocol, languageId: newsItem.LanguageId, ensureTwoPublishedLanguages: false);
        localizedUrl = GetLocalizedUrl(url, language);
        return new SitemapUrlModel(localizedUrl, new List<string>(), UpdateFrequency.Weekly, updatedOn);
    }

    /// <summary>
    /// Return localized urls
    /// </summary>
    /// <param name="routeName">Route name</param>
    /// <param name="dateTimeUpdatedOn">A time when URL was updated last time</param>
    /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task<SitemapUrlModel> PrepareLocalizedSitemapUrlAsync(string routeName,
        DateTime? dateTimeUpdatedOn = null,
        string protocol = null)
    {
        var url = _nopUrlHelper.RouteUrl(routeName, null, protocol);

        var store = await _storeContext.GetCurrentStoreAsync();

        var updatedOn = dateTimeUpdatedOn ?? DateTime.UtcNow;
        var languages = _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled
            ? await _languageService.GetAllLanguagesAsync(storeId: store.Id)
            : null;

        if (languages == null || languages.Count == 1)
            return new SitemapUrlModel(url, new List<string>(), UpdateFrequency.Weekly, updatedOn);

        //return list of localized urls
        var localizedUrls = await languages
            .Select(lang => GetLocalizedUrl(url, lang))
            .Where(value => !string.IsNullOrEmpty(value))
            .ToListAsync();

        return new SitemapUrlModel(url, localizedUrls, UpdateFrequency.Weekly, updatedOn);
    }

    #endregion

    /// <summary>
    /// Handle sitemap URLs created event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(SitemapCreatedEvent eventMessage)
    {
        if (!_newsSettings.Enabled)
            return;

        var store = await _storeContext.GetCurrentStoreAsync();
        var storeProtocol = store.SslEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;

        eventMessage.SitemapUrls.Add(await PrepareLocalizedSitemapUrlAsync(NewsDefaults.Routes.Public.NewsArchive, protocol: storeProtocol));

        if (!_newsSettings.SitemapXmlIncludeNews)
            return;

        foreach (var smUrl in await GetNewsItemUrlsAsync(store))
            eventMessage.SitemapUrls.Add(smUrl);
    }

    /// <summary>
    /// Handle model prepared event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
    {
        if (eventMessage.Model is not SitemapModel sitemapModel || !_newsSettings.Enabled)
            return;

        var store = await _storeContext.GetCurrentStoreAsync();

        sitemapModel.Items.Add(new()
        {
            GroupTitle = await _localizationService.GetResourceAsync("Sitemap.General"),
            Name = await _localizationService.GetResourceAsync("Plugins.Misc.News.Title"),
            Url = _nopUrlHelper.RouteUrl(NewsDefaults.Routes.Public.NewsArchive)
        });

        if (_newsSettings.SitemapIncludeNews)
        {
            var newsGroupTitle = await _localizationService.GetResourceAsync("Plugins.Misc.News.Sitemap.News");
            var news = await _newsService.GetAllNewsAsync(storeId: store.Id);

            sitemapModel.Items.AddRange(await news.SelectAwait(async newsItem => new SitemapModel.SitemapItemModel
            {
                GroupTitle = newsGroupTitle,
                Name = newsItem.Title,
                Url = _nopUrlHelper.RouteUrl(NewsDefaults.Routes.Public.NewsItemRouteName, new { SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false) })
            }).ToListAsync());
        }
    }
}