using Nop.Services.Cms;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.News.Services.Events;

/// <summary>
/// Represents the plugin event consumer
/// </summary>
public class AdminMenuCreatedEventConsumer : IConsumer<AdminMenuCreatedEvent>
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IWidgetPluginManager _pluginManager;

    #endregion

    #region Ctor

    public AdminMenuCreatedEventConsumer(ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        IWidgetPluginManager pluginManager)
    {
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _pluginManager = pluginManager;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle admin menu created event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        var plugin = await _pluginManager.LoadPluginBySystemNameAsync(NewsDefaults.SystemName);

        //the LoadPluginBySystemNameAsync method returns only plugins that are already fully installed,
        //while the IConsumer<AdminMenuCreatedEvent> event can be called before the installation is complete
        if (plugin == null || !_pluginManager.IsPluginActive(plugin))
            return;

        eventMessage.RootMenuItem.InsertAfter("Message templates", new()
        {
            SystemName = NewsDefaults.NewsItemsMenuSystemName,
            Title = await _localizationService.GetResourceAsync("Plugins.Misc.News.NewsItems"),
            IconClass = "far fa-dot-circle",
            Url = _nopUrlHelper.RouteUrl(NewsDefaults.Routes.Admin.NewsItemsRouteName),
            PermissionNames = new List<string> { NewsDefaults.Permissions.NEWS_VIEW }
        });

        eventMessage.RootMenuItem.InsertAfter(NewsDefaults.NewsItemsMenuSystemName, new()
        {
            SystemName = NewsDefaults.NewsCommentsMenuSystemName,
            Title = await _localizationService.GetResourceAsync("Plugins.Misc.News.Comments"),
            IconClass = "far fa-dot-circle",
            Url = _nopUrlHelper.RouteUrl(NewsDefaults.Routes.Admin.NewsCommentsRouteName),
            PermissionNames = new List<string> { NewsDefaults.Permissions.NEWS_COMMENTS_VIEW }
        });
    }

    #endregion
}