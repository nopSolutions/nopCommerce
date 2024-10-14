using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.Zettle.Infrastructure;

/// <summary>
/// Represents plugin event consumer
/// </summary>
public class EventConsumer: IConsumer<AdminMenuCreatedEvent>
{
    private readonly ILocalizationService _localizationService;
    private readonly IPluginManager<IMiscPlugin> _pluginManager;

    public EventConsumer(ILocalizationService localizationService, 
        IPluginManager<IMiscPlugin> pluginManager)
    {
        _localizationService = localizationService;
        _pluginManager = pluginManager;
    }

    public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        var description = await _pluginManager.LoadPluginBySystemNameAsync(ZettleDefaults.SystemName);
        
        eventMessage.RootMenuItem.InsertAfter("Shipping",
            new AdminMenuItem
            {
                Visible = true,
                SystemName = "POS plugins",
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.Zettle.Menu.Pos"),
                IconClass = "far fa-dot-circle",
                ChildNodes = new List<AdminMenuItem>
                {
                    new()
                    {
                        Visible = true,
                        SystemName = ZettleDefaults.SystemName,
                        Title = description?.PluginDescriptor.FriendlyName ?? "PayPal Zettle",
                        Url = eventMessage.GetMenuItemUrl("ZettleAdmin", "Configure"),
                        IconClass = "far fa-circle"
                    }
                }
            });
    }
}
