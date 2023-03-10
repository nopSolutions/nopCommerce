using Nop.Core.Domain.Customers;

namespace Nop.Services.Plugins
{
    /// <summary>
    /// Represents a plugin manager
    /// </summary>
    /// <typeparam name="TPlugin">Type of plugin</typeparam>
    public partial interface IPluginManager<TPlugin> where TPlugin : class, IPlugin
    {
        /// <summary>
        /// Load all plugins
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of plugins
        /// </returns>
        Task<IList<TPlugin>> LoadAllPluginsAsync(Customer customer = null, int storeId = 0);

        /// <summary>
        /// Load plugin by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the plugin
        /// </returns>
        Task<TPlugin> LoadPluginBySystemNameAsync(string systemName, Customer customer = null, int storeId = 0);

        /// <summary>
        /// Load active plugins
        /// </summary>
        /// <param name="systemNames">System names of active plugins</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of active plugins
        /// </returns>
        Task<IList<TPlugin>> LoadActivePluginsAsync(List<string> systemNames, Customer customer = null, int storeId = 0);

        /// <summary>
        /// Check whether the passed plugin is active
        /// </summary>
        /// <param name="plugin">Plugin to check</param>
        /// <param name="systemNames">System names of active plugins</param>
        /// <returns>Result</returns>
        bool IsPluginActive(TPlugin plugin, List<string> systemNames);

        /// <summary>
        /// Get plugin logo URL
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the logo URL
        /// </returns>
        Task<string> GetPluginLogoUrlAsync(TPlugin plugin);
    }
}