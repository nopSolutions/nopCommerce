using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Factories;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Sitemap;

namespace Nop.Plugin.Misc.Forums.Services.Events;

/// <summary>
/// Represents the plugin event consumer
/// </summary>
public class SitemapEventConsumer : IConsumer<SitemapCreatedEvent>, IConsumer<ModelPreparedEvent<BaseNopModel>>
{
    #region Fields

    private readonly ForumSettings _forumSettings;
    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly ISitemapModelFactory _sitemapModelFactory;

    #endregion

    #region Ctor

    public SitemapEventConsumer(
        ForumSettings forumSettings,
        ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        ISitemapModelFactory sitemapModelFactory)
    {
        _forumSettings = forumSettings;
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _sitemapModelFactory = sitemapModelFactory;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle sitemap URLs created event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(SitemapCreatedEvent eventMessage)
    {
        if (!_forumSettings.ForumsEnabled)
            return;

        eventMessage.SitemapUrls.Add(await _sitemapModelFactory.PrepareLocalizedSitemapUrlAsync(ForumDefaults.Routes.Public.BOARDS));
    }

    /// <summary>
    /// Handle model prepared event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
    {
        if (eventMessage.Model is not SitemapModel sitemapModel || !_forumSettings.ForumsEnabled)
            return;

        sitemapModel.Items.Add(new()
        {
            GroupTitle = await _localizationService.GetResourceAsync("Sitemap.General"),
            Name = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Forums"),
            Url = _nopUrlHelper.RouteUrl(ForumDefaults.Routes.Public.BOARDS)
        });

    }

    #endregion
}