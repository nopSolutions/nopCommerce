using Nop.Services.Common;
using Nop.Services.Helpers;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.OmnichannelCore;

/// <summary>
/// Represents the omnichannel core plugin
/// </summary>
public class OmnichannelCorePlugin : BasePlugin, IMiscPlugin
{
    #region Fields

    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public OmnichannelCorePlugin(IWebHelper webHelper)
    {
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}{OmnichannelCoreDefaults.ConfigurationRoute}";
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
