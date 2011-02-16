using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Investigates the execution environment to find plugins.
    /// </summary>
    [Service(typeof(IPluginFinder))]
    public class PluginFinder : IPluginFinder
    {
        private IList<IPlugin> plugins = null;
        private readonly ITypeFinder typeFinder;
        public IEnumerable<InterfacePluginElement> addedPlugins = new InterfacePluginElement[0];
        public IEnumerable<InterfacePluginElement> removedPlugins = new InterfacePluginElement[0];

        public PluginFinder(ITypeFinder typeFinder, EngineSection config)
        {
            addedPlugins = config.InterfacePlugins.AllElements;
            removedPlugins = config.InterfacePlugins.RemovedElements;
            this.typeFinder = typeFinder;
            this.plugins = FindPlugins();
        }

        public PluginFinder(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
            this.plugins = FindPlugins();
        }

        /// <summary>Gets plugins found in the environment sorted and filtered by the given user.</summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <param name="user">The user that should be authorized for the plugin.</param>
        /// <returns>An enumeration of plugins.</returns>
        public IEnumerable<T> GetPlugins<T>(IPrincipal user) where T : class, IPlugin
        {
            foreach (T plugin in GetPlugins<T>())
                if (plugin.IsAuthorized(user))
                    yield return plugin;
        }

        public IEnumerable<T> GetPlugins<T>() where T : class, IPlugin
        {
            foreach (IPlugin plugin in plugins)
                if (plugin is T)
                    yield return plugin as T;
        }

        /// <summary>Finds and sorts plugin defined in known assemblies.</summary>
        /// <returns>A sorted list of plugins.</returns>
        protected virtual IList<IPlugin> FindPlugins()
        {
            List<IPlugin> foundPlugins = new List<IPlugin>();
            foreach (Assembly assembly in typeFinder.GetAssemblies())
            {
                foreach (IPlugin plugin in FindPluginsIn(assembly))
                {
                    if (plugin.Name == null)
                        throw new Exception(string.Format("A plugin in the assembly '{0}' has no name. The plugin is likely defined on the assembly ([assembly:...]). Try assigning the plugin a unique name and recompiling.", assembly.FullName));
                    if (foundPlugins.Contains(plugin))
                        throw new Exception(string.Format("A plugin of the type '{0}' named '{1}' is already defined, assembly: {2}", plugin.GetType().FullName, plugin.Name, assembly.FullName));

                    if (!IsRemoved(plugin))
                        foundPlugins.Add(plugin);
                }
            }
            foundPlugins.Sort();
            return foundPlugins;
        }

        private bool IsRemoved(IPlugin plugin)
        {
            foreach (InterfacePluginElement configElement in removedPlugins)
            {
                if (plugin.Name == configElement.Name)
                    return true;
            }
            return false;
        }

        private IEnumerable<IPlugin> FindPluginsIn(Assembly a)
        {
            foreach (IPlugin attribute in a.GetCustomAttributes(typeof(IPlugin), false))
            {
                yield return attribute;
            }
            foreach (Type t in a.GetTypes())
            {
                foreach (IPlugin attribute in t.GetCustomAttributes(typeof(IPlugin), false))
                {
                    if (attribute.Name == null)
                        attribute.Name = t.Name;
                    attribute.Decorates = t;

                    yield return attribute;
                }
            }
        }
    }
}
