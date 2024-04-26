namespace Nop.Services.Plugins;

/// <summary>
/// Interface denoting plug-in attributes that are displayed throughout 
/// the editing interface.
/// </summary>
public partial interface IPlugin
{
    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    string GetConfigurationPageUrl();

    /// <summary>
    /// Gets or sets the plugin descriptor
    /// </summary>
    PluginDescriptor PluginDescriptor { get; set; }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InstallAsync();

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UninstallAsync();

    /// <summary>
    /// Update plugin
    /// </summary>
    /// <param name="currentVersion">Current version of plugin</param>
    /// <param name="targetVersion">New version of plugin</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateAsync(string currentVersion, string targetVersion);

    /// <summary>
    /// Prepare plugin to the uninstallation
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PreparePluginToUninstallAsync();
}