using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.Dynamics365;

/// <summary>
/// Represents the Dynamics 365 helper plugin
/// </summary>
public class Dynamics365Plugin(IWebHelper webHelper) : BasePlugin, IMiscPlugin
{
    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return $"{webHelper.GetStoreLocation()}Admin/Dynamics365/Configure";
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        await base.UninstallAsync();
    }

    #endregion
}
