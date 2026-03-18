using Nop.Services.Cms;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Framework.Events;

namespace Nop.Plugin.Misc.Polls.Services.Events;

/// <summary>
/// Represents the plugin event consumer
/// </summary>
public class AdminMenuCreatedEventConsumer : IConsumer<AdminMenuCreatedEvent>
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly IWidgetPluginManager _pluginManager;

    #endregion

    #region Ctor

    public AdminMenuCreatedEventConsumer(ILocalizationService localizationService,
        IWidgetPluginManager pluginManager)
    {
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
        var plugin = await _pluginManager.LoadPluginBySystemNameAsync(PollsDefaults.SystemName);

        //the LoadPluginBySystemNameAsync method returns only plugins that are already fully installed,
        //while the IConsumer<AdminMenuCreatedEvent> event can be called before the installation is complete
        if (plugin == null || !_pluginManager.IsPluginActive(plugin))
            return;

        eventMessage.RootMenuItem.InsertAfter("Blog comments", new()
        {
            SystemName = "Polls",
            Title = await _localizationService.GetResourceAsync("Plugins.Misc.Polls"),
            PermissionNames = new List<string> { PollsDefaults.Permissions.POLLS_VIEW },
            Url = eventMessage.GetMenuItemUrl("PollAdmin", "List"),
            IconClass = "far fa-dot-circle"
        });
    }

    #endregion
}