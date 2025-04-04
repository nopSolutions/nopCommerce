using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;
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

    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly ILanguageService _languageService;
    private readonly ILocalizationService _localizationService;
    private readonly IStoreContext _storeContext;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly IUrlRecordService _urlRecordService;
    private readonly LocalizationSettings _localizationSettings;
    private readonly NewsService _newsService;
    private readonly NewsSettings _newsSettings;

    #endregion

    #region Ctor

    public SitemapEventConsumer(IActionContextAccessor actionContextAccessor,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IStoreContext storeContext,
        IUrlHelperFactory urlHelperFactory,
        IUrlRecordService urlRecordService,
        LocalizationSettings localizationSettings,
        NewsService newsService,
        NewsSettings newsSettings)
    {
        _actionContextAccessor = actionContextAccessor;
        _languageService = languageService;
        _localizationService = localizationService;
        _storeContext = storeContext;
        _urlHelperFactory = urlHelperFactory;
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
    private string GetLocalizedUrl(string currentUrl, Language lang)
    {
        if (string.IsNullOrEmpty(currentUrl))
            return null;

        if (_actionContextAccessor.ActionContext == null)
            return null;

        var pathBase = _actionContextAccessor.ActionContext.HttpContext.Request.PathBase;

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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap URLs
    /// </returns>
    private async Task<IEnumerable<SitemapUrlModel>> GetNewsItemUrlsAsync(Store store)
    {
        var protocol = store.SslEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
        var allNews = await _newsService.GetAllNewsAsync(storeId: store.Id);

        return await allNews.SelectAwait(async newsItem =>
        {
            var routeValues = new { SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false) };
            return await PrepareLocalizedSitemapUrlAsync(NewsDefaults.Routes.Public.NewsItemRouteName, routeValues, protocol, store);
        }).ToListAsync();
    }

    private async Task<SitemapUrlModel> PrepareLocalizedSitemapUrlAsync(string routeName, object values, string protocol,
        Store store, DateTime? dateTimeUpdatedOn = null)
    {
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

        var url = urlHelper.RouteUrl(routeName, values, protocol);

        var updatedOn = dateTimeUpdatedOn ?? DateTime.UtcNow;
        var languages = _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled
            ? await _languageService.GetAllLanguagesAsync(storeId: store.Id)
            : null;

        if (languages == null || languages.Count == 1)
            return new SitemapUrlModel(url, [], UpdateFrequency.Weekly, updatedOn);

        //return list of localized urls
        var localizedUrls = await languages
            .Select(lang =>
            {
                var currentUrl = urlHelper.RouteUrl(routeName, values, protocol);
                return GetLocalizedUrl(currentUrl, lang);
            })
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
        var protocol = store.SslEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;

        eventMessage.SitemapUrls.Add(await PrepareLocalizedSitemapUrlAsync(NewsDefaults.Routes.Public.NewsArchive, null, protocol, store));

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

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

        sitemapModel.Items.Add(new SitemapModel.SitemapItemModel
        {
            GroupTitle = await _localizationService.GetResourceAsync("Sitemap.General"),
            Name = await _localizationService.GetResourceAsync("Plugins.Misc.News.Title"),
            Url = urlHelper.RouteUrl(NewsDefaults.Routes.Public.NewsArchive)
        });

        if (_newsSettings.SitemapIncludeNews)
        {
            var newsGroupTitle = await _localizationService.GetResourceAsync("Plugins.Misc.News.Sitemap.News");
            var news = await _newsService.GetAllNewsAsync(storeId: store.Id);

            sitemapModel.Items.AddRange(await news.SelectAwait(async newsItem => new SitemapModel.SitemapItemModel
            {
                GroupTitle = newsGroupTitle,
                Name = newsItem.Title,
                Url = urlHelper.RouteUrl(NewsDefaults.Routes.Public.NewsItemRouteName, new { SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false) })
            }).ToListAsync());
        }
    }
}