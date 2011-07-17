using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Investigates the execution environment to find plugins.
    /// </summary>
    [Dependency(typeof(IPluginFinder))]
    public class PluginFinder : IPluginFinder
    {
        private IList<PluginDescriptor> _plugins;
        private bool _arePluginsLoaded = false;
        private readonly ITypeFinder _typeFinder;

        public PluginFinder(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }
        
        public virtual IEnumerable<T> GetPlugins<T>(bool installedOnly = true) where T : class, IPlugin
        {
            EnsurePluginsAreLoaded();

            foreach (var plugin in _plugins)
                if (typeof(T).IsAssignableFrom(plugin.PluginType))
                    if (!installedOnly || plugin.Installed)
                        yield return plugin.Instance<T>();
        }

        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptors(bool installedOnly = true)
        {
            EnsurePluginsAreLoaded();

            foreach (var plugin in _plugins)
                if (!installedOnly || plugin.Installed)
                    yield return plugin;
        }

        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptors<T>(bool installedOnly = true) where T : class, IPlugin
        {
            EnsurePluginsAreLoaded();

            foreach (var plugin in _plugins)
                if (typeof(T).IsAssignableFrom(plugin.PluginType))
                    if (!installedOnly || plugin.Installed)
                        yield return plugin;
        }

        public virtual PluginDescriptor GetPluginDescriptorBySystemName(string systemName, bool installedOnly = true)
        {
            return GetPluginDescriptors(installedOnly)
                .SingleOrDefault(p => p.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        public virtual PluginDescriptor GetPluginDescriptorBySystemName<T>(string systemName, bool installedOnly = true) where T : class, IPlugin
        {
            return GetPluginDescriptors<T>(installedOnly)
                .SingleOrDefault(p => p.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Finds and sorts plugin defined in known assemblies.
        /// </summary>
        /// <returns>
        /// A sorted list of plugins.
        /// </returns>
        protected virtual IList<PluginDescriptor> FindAllPlugins()
        {
            var foundPlugins = PluginManager.ReferencedPlugins.ToList();
            //sort
            foundPlugins.Sort();
            return foundPlugins.ToList();
        }

        protected virtual void EnsurePluginsAreLoaded()
        {
            if (!_arePluginsLoaded)
            {
                _plugins = FindAllPlugins();
                _arePluginsLoaded = true;
            }
        }
    }
}
