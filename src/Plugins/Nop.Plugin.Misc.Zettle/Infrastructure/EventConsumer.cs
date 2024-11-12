using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Plugins;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.Zettle.Infrastructure;

/// <summary>
/// Represents plugin event consumer
/// </summary>
public class EventConsumer: IConsumer<AdminMenuCreatedEvent>
{
    private readonly IPluginManager<IMiscPlugin> _pluginManager;

    public EventConsumer(IPluginManager<IMiscPlugin> pluginManager)
    {
        _pluginManager = pluginManager;
    }

    public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        var description = (await _pluginManager.LoadPluginBySystemNameAsync(ZettleDefaults.SystemName))?.PluginDescriptor;
        
        eventMessage.RootMenuItem.InsertAfter("Shipping",
            new AdminMenuItem
            {
                Visible = true,
                SystemName = description?.SystemName ?? string.Empty,
                Title = description?.FriendlyName ?? string.Empty,
                IconClass = "far fa-dot-circle",
                Url = eventMessage.GetMenuItemUrl("ZettleAdmin", "Configure"),
            });
    }
}
