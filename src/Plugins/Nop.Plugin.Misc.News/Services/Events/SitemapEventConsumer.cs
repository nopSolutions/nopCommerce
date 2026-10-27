using Nop.Core;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Factories;
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

    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly ISitemapModelFactory _sitemapModelFactory;
    private readonly IStoreContext _storeContext;
    private readonly NewsService _newsService;
    private readonly NewsSettings _newsSettings;

    #endregion

    #region Ctor

    public SitemapEventConsumer(ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        ISitemapModelFactory sitemapModelFactory,
        IStoreContext storeContext,
        NewsService newsService,
        NewsSettings newsSettings)
    {
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _sitemapModelFactory = sitemapModelFactory;
        _storeContext = storeContext;
        _newsService = newsService;
        _newsSettings = newsSettings;
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

        eventMessage.SitemapUrls.Add(await _sitemapModelFactory.PrepareLocalizedSitemapUrlAsync(NewsDefaults.Routes.Public.NewsArchive));

        if (!_newsSettings.SitemapXmlIncludeNews)
            return;

        var store = await _storeContext.GetCurrentStoreAsync();
        var newsList = await _newsService.GetAllNewsAsync(storeId: store.Id);

        var newsSitemapUrls = await newsList
            .SelectAwait(async news => await _sitemapModelFactory.PrepareLocalizedSitemapUrlAsync(news, news.CreatedOnUtc, languageId: news.LanguageId))
            .ToListAsync();

        foreach (var smUrl in newsSitemapUrls)
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
                Url = await _nopUrlHelper.RouteGenericUrlAsync(newsItem, languageId: newsItem.LanguageId, ensureTwoPublishedLanguages: false)
            }).ToListAsync());
        }
    }
}