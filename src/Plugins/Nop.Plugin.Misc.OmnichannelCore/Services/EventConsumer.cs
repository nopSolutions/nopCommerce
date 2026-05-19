using Nop.Services.Plugins;
using Nop.Web.Framework.Events;

namespace Nop.Plugin.Misc.OmnichannelCore.Services;

/// <summary>
/// Represents plugin event consumer
/// </summary>
public class EventConsumer : BaseAdminMenuCreatedEventConsumer
{
    #region Ctor

    public EventConsumer(IPluginManager<IPlugin> pluginManager) :
        base(pluginManager)
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the plugin system name
    /// </summary>
    protected override string PluginSystemName => OmnichannelCoreDefaults.SystemName;

    /// <summary>
    /// The system name of the menu item before which the current one is inserted
    /// </summary>
    protected override string BeforeMenuSystemName => "Local plugins";

    #endregion
}
