using Nop.Services.Plugins;

namespace Nop.Web.Framework.Menu;

public static partial class PluginAdminMenuExtensions
{
    public static AdminMenuItem GetAdminMenuItem(this IPlugin plugin, string iconClass = "far fa-dot-circle")
    {
        var description = plugin?.PluginDescriptor;

        if (description == null)
            return new AdminMenuItem { Visible = false };
        
        return new AdminMenuItem
        {
            SystemName = description.SystemName,
            Title = description.FriendlyName,
            IconClass = iconClass,
            Url = plugin.GetConfigurationPageUrl()
        };
    }
}
