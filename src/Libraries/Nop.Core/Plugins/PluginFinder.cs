using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using Nop.Core.Configuration;
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
        
        public IEnumerable<T> GetPlugins<T>() where T : class, IPlugin
        {
            EnsurePluginsAreLoaded();

            //TODO performance optimization: do nto return instances, return a list of PluginDescriptor
            foreach (var plugin in _plugins)
                if (typeof(T).IsAssignableFrom(plugin.PluginType))
                    yield return plugin.Instance<T>();
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
            
            //find plugin type implementations
            foreach (var plugin in foundPlugins)
            {
                var types = _typeFinder.FindClassesOfType<IPlugin>(new List<Assembly> { plugin.ReferencedAssembly }).ToList();
                if (types.Count > 0)
                    plugin.PluginType = types.FirstOrDefault();
            }

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
