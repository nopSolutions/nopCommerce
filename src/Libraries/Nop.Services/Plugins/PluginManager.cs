using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Plugins
{
    /// <summary>
    /// Represents a plugin manager implementation
    /// </summary>
    /// <typeparam name="TPlugin">Type of plugin</typeparam>
    public partial class PluginManager<TPlugin> : IPluginManager<TPlugin> where TPlugin : class, IPlugin
    {
        #region Constants

        private const string KEY_FORMAT = "{0}-{1}-{2}";

        #endregion

        #region Fields

        private readonly IPluginService _pluginService;
        private readonly Dictionary<string, IList<TPlugin>> _plugins = new Dictionary<string, IList<TPlugin>>();

        #endregion

        #region Ctor

        public PluginManager(IPluginService pluginService)
        {
            _pluginService = pluginService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load all plugins
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>List of plugins</returns>
        public virtual IList<TPlugin> LoadAllPlugins(Customer customer = null, int storeId = 0)
        {
            //get plugins and put them into the dictionary to avoid further loading
            var key = string.Format(KEY_FORMAT, null, customer?.CustomerGuid ?? default(Guid), storeId);
            if (!_plugins.ContainsKey(key))
                _plugins.Add(key, _pluginService.GetPlugins<TPlugin>(customer: customer, storeId: storeId).ToList());

            return _plugins[key];
        }

        /// <summary>
        /// Load plugin by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Plugin</returns>
        public virtual TPlugin LoadPluginBySystemName(string systemName, Customer customer = null, int storeId = 0)
        {
            if (string.IsNullOrEmpty(systemName))
                return null;

            //try to get already loaded plugin
            var key = string.Format(KEY_FORMAT, systemName, customer?.CustomerGuid ?? default(Guid), storeId);
            if (_plugins.ContainsKey(key))
                return _plugins[key].FirstOrDefault();

            //or get it from list of all loaded plugins, or load it for the first time
            var pluginBySystemName = LoadAllPlugins(customer, storeId)
                .FirstOrDefault(plugin => plugin.PluginDescriptor.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase))
                ?? _pluginService.GetPluginDescriptorBySystemName<TPlugin>(systemName, customer: customer, storeId: storeId)?.Instance<TPlugin>();
            _plugins.Add(key, new List<TPlugin> { pluginBySystemName });

            return pluginBySystemName;
        }

        /// <summary>
        /// Load primary active plugin
        /// </summary>
        /// <param name="systemName">System name of primary active plugin</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Plugin</returns>
        public virtual TPlugin LoadPrimaryPlugin(string systemName, Customer customer = null, int storeId = 0)
        {
            //try to get a plugin by system name or return the first loaded one (it's necessary to have a primary active plugin)
            var plugin = LoadPluginBySystemName(systemName, customer, storeId)
                ?? LoadAllPlugins(customer, storeId).FirstOrDefault();

            return plugin;
        }

        /// <summary>
        /// Load active plugins
        /// </summary>
        /// <param name="systemNames">System names of active plugins</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>List of active plugins</returns>
        public virtual IList<TPlugin> LoadActivePlugins(List<string> systemNames, Customer customer = null, int storeId = 0)
        {
            if (systemNames == null)
                return new List<TPlugin>();

            //get loaded plugins according to passed system names
            return LoadAllPlugins(customer, storeId)
                .Where(plugin => systemNames.Contains(plugin.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Check whether the passed plugin is active
        /// </summary>
        /// <param name="plugin">Plugin to check</param>
        /// <param name="systemNames">System names of active plugins</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(TPlugin plugin, List<string> systemNames)
        {
            if (plugin == null)
                return false;

            return systemNames
                ?.Any(systemName => plugin.PluginDescriptor.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase))
                ?? false;
        }

        /// <summary>
        /// Get plugin logo URL
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <returns>Logo URL</returns>
        public virtual string GetPluginLogoUrl(TPlugin plugin)
        {
            return _pluginService.GetPluginLogoUrl(plugin.PluginDescriptor);
        }

        #endregion
    }
}