using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Plugin finder
    /// </summary>
    public class PluginFinder : IPluginFinder
    {
        #region Fields

        private IList<PluginDescriptor> _plugins;

        private bool _arePluginsLoaded = false;

        #endregion

        #region Utilities

        /// <summary>
        /// Ensure plugins are loaded
        /// </summary>
        protected virtual void EnsurePluginsAreLoaded()
        {
            if (!_arePluginsLoaded)
            {
                var foundPlugins = PluginManager.ReferencedPlugins.ToList();
                foundPlugins.Sort(); //sort
                _plugins = foundPlugins.ToList();

                _arePluginsLoaded = true;
            }
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Check whether the plugin is available in a certain store
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="storeId">Store identifier to check</param>
        /// <returns>true - available; false - no</returns>
        public virtual bool AuthenticateStore(PluginDescriptor pluginDescriptor, int storeId)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException("pluginDescriptor");

            //no validation required
            if (storeId == 0)
                return true;

            if (pluginDescriptor.LimitedToStores.Count == 0)
                return true;

            return pluginDescriptor.LimitedToStores.Contains(storeId);
        }

        /// <summary>
        /// Gets plugins
        /// </summary>
        /// <typeparam name="T">The type of plugins to get.</typeparam>
        /// <param name="installedOnly">A value indicating whether to load only installed plugins</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Plugins</returns>
        public virtual IEnumerable<T> GetPlugins<T>(bool installedOnly = true, int storeId = 0) where T : class, IPlugin
        {
            EnsurePluginsAreLoaded();

            foreach (var plugin in _plugins)
                if (typeof(T).IsAssignableFrom(plugin.PluginType))
                    if (!installedOnly || plugin.Installed)
                        if (AuthenticateStore(plugin, storeId))
                            yield return plugin.Instance<T>();
        }

        /// <summary>
        /// Get plugin descriptors
        /// </summary>
        /// <param name="installedOnly">A value indicating whether to load only installed plugins</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Plugin descriptors</returns>
        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptors(bool installedOnly = true, int storeId = 0)
        {
            EnsurePluginsAreLoaded();

            foreach (var plugin in _plugins)
                if (!installedOnly || plugin.Installed)
                    if (AuthenticateStore(plugin, storeId))
                        yield return plugin;
        }

        /// <summary>
        /// Get plugin descriptors
        /// </summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <param name="installedOnly">A value indicating whether to load only installed plugins</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Plugin descriptors</returns>
        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptors<T>(bool installedOnly = true, int storeId = 0) 
            where T : class, IPlugin
        {
            EnsurePluginsAreLoaded();

            foreach (var plugin in _plugins)
                if (typeof(T).IsAssignableFrom(plugin.PluginType))
                    if (!installedOnly || plugin.Installed)
                        if (AuthenticateStore(plugin, storeId))
                            yield return plugin;
        }

        /// <summary>
        /// Get a plugin descriptor by its system name
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        /// <param name="installedOnly">A value indicating whether to load only installed plugins</param>
        /// <returns>>Plugin descriptor</returns>
        public virtual PluginDescriptor GetPluginDescriptorBySystemName(string systemName, bool installedOnly = true)
        {
            return GetPluginDescriptors(installedOnly)
                .SingleOrDefault(p => p.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Get a plugin descriptor by its system name
        /// </summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <param name="systemName">Plugin system name</param>
        /// <param name="installedOnly">A value indicating whether to load only installed plugins</param>
        /// <returns>>Plugin descriptor</returns>
        public virtual PluginDescriptor GetPluginDescriptorBySystemName<T>(string systemName, bool installedOnly = true) where T : class, IPlugin
        {
            return GetPluginDescriptors<T>(installedOnly)
                .SingleOrDefault(p => p.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }
        
        /// <summary>
        /// Reload plugins
        /// </summary>
        public virtual void ReloadPlugins()
        {
            _arePluginsLoaded = false;
            EnsurePluginsAreLoaded();
        }

        #endregion
    }
}
