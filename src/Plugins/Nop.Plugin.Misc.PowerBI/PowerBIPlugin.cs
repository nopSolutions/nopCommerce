﻿using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.PowerBI;

/// <summary>
/// Represents the Power BI helper plugin
/// </summary>
public class PowerBIPlugin : BasePlugin, IMiscPlugin
{
    #region Fields

    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public PowerBIPlugin(IWebHelper webHelper)
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
        return $"{_webHelper.GetStoreLocation()}Admin/PowerBI/Configure";
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
