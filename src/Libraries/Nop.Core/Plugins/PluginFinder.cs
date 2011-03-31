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

        /// <summary>
        /// Gets plugins found in the environment sorted and filtered by the given user.
        /// </summary>
        /// <typeparam name="T">
        /// The type of plugin to get.
        /// </typeparam>
        /// <param name="user">
        /// The user that should be authorized for the plugin.
        /// </param>
        /// <returns>
        /// An enumeration of plugins.</returns>
        public IEnumerable<T> GetPlugins<T>(IPrincipal user) where T : class, IPlugin
        {
            foreach (T plugin in GetPlugins<T>())
                if (plugin.IsAuthorized(user))
                    yield return plugin;
        }

        public IEnumerable<T> GetPlugins<T>() where T : class, IPlugin
        {
            EnsurePluginsAreLoaded();

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
        protected virtual IList<PluginDescriptor> FindPlugins()
        {
            var foundPlugins = new List<PluginDescriptor>();
            foreach (Assembly assembly in _typeFinder.GetAssemblies())
            {
                foreach (PluginDescriptor plugin in FindPluginsIn(assembly))
                {
                    if (plugin.Instance().SystemName == null)
                        throw new Exception(string.Format("A plugin in the assembly '{0}' has no system name. The plugin is likely defined on the assembly ([assembly:...]). Try assigning the plugin a unique name and recompiling.", assembly.FullName));
                    if (foundPlugins.Contains(plugin))
                        throw new Exception(string.Format("A plugin of the type '{0}' named '{1}' is already defined, assembly: {2}", plugin.PluginType.FullName, plugin.Instance().SystemName, assembly.FullName));
                    
                    foundPlugins.Add(plugin);
                }
            }
            foundPlugins.Sort();
            return foundPlugins;
        }

        protected virtual void EnsurePluginsAreLoaded()
        {
            if (!_arePluginsLoaded)
            {
                _plugins = FindPlugins();
                _arePluginsLoaded = true;
            }
        }

        private IEnumerable<PluginDescriptor> FindPluginsIn(Assembly a)
        {
            
            // return plugin attributes
            foreach (IPlugin attribute in a.GetCustomAttributes(typeof(IPlugin), false))
            {
                yield return new PluginAttributeDescriptor(attribute);
            }
            foreach (Type t in a.GetTypes())
            {
                foreach (IPlugin attribute in t.GetCustomAttributes(typeof(IPlugin), false))
                {
                    yield return new PluginAttributeDescriptor(attribute);
                }
            }
            
            //return plugin implementations
            foreach (var plugin in _typeFinder.FindClassesOfType<IPlugin>(new List<Assembly> { a }))
            {
                //Make sure that the IPlugin found is not implemented on a attribute (not an actual implementation)
                if (!typeof(System.Attribute).IsAssignableFrom(plugin))
                {
                    yield return new PluginImplementationDescriptor(plugin);
                }
            }
        }
    }
}
