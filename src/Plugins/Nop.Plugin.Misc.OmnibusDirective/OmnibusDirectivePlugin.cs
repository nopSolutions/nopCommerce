using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.OmnibusDirective;

/// <summary>
/// Represents EU Omnibus Directive plugin
/// </summary>
public class OmnibusDirectivePlugin(IPermissionService permissionService,
        IWebHelper webHelper) 
    : BasePlugin, IMiscPlugin
{
    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return $"{webHelper.GetStoreLocation()}Admin/OmnibusDirective/Configure";
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
