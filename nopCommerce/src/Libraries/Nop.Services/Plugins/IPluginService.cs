using Nop.Core.Domain.Customers;

namespace Nop.Services.Plugins;

/// <summary>
/// Represents a plugin service
/// </summary>
public partial interface IPluginService
{
    /// <summary>
    /// Get plugin descriptors
    /// </summary>
    /// <typeparam name="TPlugin">The type of plugins to get</typeparam>
    /// <param name="loadMode">Filter by load plugins mode</param>
    /// <param name="customer">Filter by  customer; pass null to load all records</param>
    /// <param name="storeId">Filter by store; pass 0 to load all records</param>
    /// <param name="group">Filter by plugin group; pass null to load all records</param>
    /// <param name="friendlyName">Filter by plugin friendly name; pass null to load all records</param>
    /// <param name="author">Filter by plugin author; pass null to load all records</param>
    /// <param name="dependsOnSystemName">System name of the plugin to define dependencies</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the plugin descriptors
    /// </returns>
    Task<IList<PluginDescriptor>> GetPluginDescriptorsAsync<TPlugin>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
        Customer customer = null, int storeId = 0, string group = null, string dependsOnSystemName = "", string friendlyName = null, string author = null) where TPlugin : class, IPlugin;

    /// <summary>
    /// Get a plugin descriptor by the system name
    /// </summary>
    /// <typeparam name="TPlugin">The type of plugin to get</typeparam>
    /// <param name="systemName">Plugin system name</param>
    /// <param name="loadMode">Load plugins mode</param>
    /// <param name="customer">Filter by  customer; pass null to load all records</param>
    /// <param name="storeId">Filter by store; pass 0 to load all records</param>
    /// <param name="group">Filter by plugin group; pass null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the >Plugin descriptor
    /// </returns>
    Task<PluginDescriptor> GetPluginDescriptorBySystemNameAsync<TPlugin>(string systemName,
        LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
        Customer customer = null, int storeId = 0, string group = null) where TPlugin : class, IPlugin;

    /// <summary>
    /// Get plugins
    /// </summary>
    /// <typeparam name="TPlugin">The type of plugins to get</typeparam>
    /// <param name="loadMode">Filter by load plugins mode</param>
    /// <param name="customer">Filter by  customer; pass null to load all records</param>
    /// <param name="storeId">Filter by store; pass 0 to load all records</param>
    /// <param name="group">Filter by plugin group; pass null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the plugins
    /// </returns>
    Task<IList<TPlugin>> GetPluginsAsync<TPlugin>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
        Customer customer = null, int storeId = 0, string group = null) where TPlugin : class, IPlugin;

    /// <summary>
    /// Find a plugin by the type which is located into the same assembly as a plugin
    /// </summary>
    /// <param name="typeInAssembly">Type</param>
    /// <returns>Plugin</returns>
    IPlugin FindPluginByTypeInAssembly(Type typeInAssembly);

    /// <summary>
    /// Get plugin logo URL
    /// </summary>
    /// <param name="pluginDescriptor">Plugin descriptor</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the logo URL
    /// </returns>
    Task<string> GetPluginLogoUrlAsync(PluginDescriptor pluginDescriptor);

    /// <summary>
    /// Prepare plugin to the installation
    /// </summary>
    /// <param name="systemName">Plugin system name</param>
    /// <param name="customer">Customer</param>
    /// <param name="checkDependencies">Specifies whether to check plugin dependencies</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PreparePluginToInstallAsync(string systemName, Customer customer = null, bool checkDependencies = true);

    /// <summary>
    /// Prepare plugin to the uninstallation
    /// </summary>
    /// <param name="systemName">Plugin system name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PreparePluginToUninstallAsync(string systemName);

    /// <summary>
    /// Prepare plugin to the removing
    /// </summary>
    /// <param name="systemName">Plugin system name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PreparePluginToDeleteAsync(string systemName);

    /// <summary>
    /// Reset changes
    /// </summary>
    void ResetChanges();

    /// <summary>
    /// Clear installed plugins list
    /// </summary>
    void ClearInstalledPluginsList();

    /// <summary>
    /// Install plugins
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InstallPluginsAsync();

    /// <summary>
    /// Uninstall plugins
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UninstallPluginsAsync();

    /// <summary>
    /// Delete plugins
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeletePluginsAsync();

    /// <summary>
    /// Update plugins
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdatePluginsAsync();

    /// <summary>
    /// Check whether application restart is required to apply changes to plugins
    /// </summary>
    /// <returns>Result of check</returns>
    bool IsRestartRequired();

    /// <summary>
    /// Get names of incompatible plugins
    /// </summary>
    /// <returns>List of plugin names</returns>
    IDictionary<string, PluginIncompatibleType> GetIncompatiblePlugins();

    /// <summary>
    /// Get all assembly loaded collisions
    /// </summary>
    /// <returns>List of plugin loaded assembly info</returns>
    IList<PluginLoadedAssemblyInfo> GetAssemblyCollisions();
}