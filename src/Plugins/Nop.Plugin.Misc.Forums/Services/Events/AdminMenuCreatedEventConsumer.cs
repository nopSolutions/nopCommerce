using Nop.Services.Cms;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Framework.Events;

namespace Nop.Plugin.Misc.Forums.Services.Events;

/// <summary>
/// Represents the plugin event consumer
/// </summary>
public class AdminMenuCreatedEventConsumer : IConsumer<AdminMenuCreatedEvent>
{
    #region Fields

    private readonly ForumSettings _forumSettings;
    private readonly ILocalizationService _localizationService;
    private readonly IWidgetPluginManager _pluginManager;

    #endregion

    #region Ctor

    public AdminMenuCreatedEventConsumer(ForumSettings forumSettings,
        ILocalizationService localizationService,
        IWidgetPluginManager pluginManager)
    {
        _forumSettings = forumSettings;
        _localizationService = localizationService;
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
        var plugin = await _pluginManager.LoadPluginBySystemNameAsync(ForumDefaults.SystemName);

        //the LoadPluginBySystemNameAsync method returns only plugins that are already fully installed,
        //while the IConsumer<AdminMenuCreatedEvent> event can be called before the installation is complete
        if (plugin == null || !_pluginManager.IsPluginActive(plugin))
            return;

        if (!_forumSettings.ForumsEnabled)
            return;

        eventMessage.RootMenuItem.InsertAfter("Message templates", new()
        {
            SystemName = ForumDefaults.ForumsMenuSystemName,
            Title = await _localizationService.GetResourceAsync("Plugins.Misc.Forums"),
            PermissionNames = new List<string> { ForumDefaults.Permissions.FORUMS_VIEW },
            Url = eventMessage.GetMenuItemUrl("Forum", "List"),
            IconClass = "far fa-dot-circle"
        });
    }

    #endregion
}