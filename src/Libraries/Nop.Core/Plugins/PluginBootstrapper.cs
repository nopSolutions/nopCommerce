using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Finds plugins and calls their initializer.
    /// </summary>
    [Dependency(typeof(IPluginBootstrapper))]
    public class PluginBootstrapper : IPluginBootstrapper
    {
        private readonly ITypeFinder typeFinder;
        //public IEnumerable<PluginInitializerElement> addedInitializers = new PluginInitializerElement[0];
        //public IEnumerable<PluginInitializerElement> removedInitializers = new PluginInitializerElement[0];

        public PluginBootstrapper(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
        }
        public PluginBootstrapper(ITypeFinder typeFinder, NopConfig config)
            : this(typeFinder)
        {
            //addedInitializers = config.PluginInitializers.AllElements;
            //removedInitializers = config.PluginInitializers.RemovedElements;
        }

        /// <summary>Gets plugins in the current app domain using the type finder.</summary>
        /// <returns>An enumeration of available plugins.</returns>
        public IEnumerable<IPluginDefinition> GetPluginDefinitions()
        {
            List<IPluginDefinition> pluginDefinitions = new List<IPluginDefinition>();

            // assembly defined plugins
            foreach (ICustomAttributeProvider assembly in typeFinder.GetAssemblies())
            {
                foreach (PluginInitializerAttribute plugin in assembly.GetCustomAttributes(typeof(PluginInitializerAttribute), false))
                {
                    if (!IsRemoved(plugin.InitializerType))
                        pluginDefinitions.Add(plugin);
                }
            }

            // autoinitialize plugins
            foreach (Type type in typeFinder.FindClassesOfType<IPluginInitializer>())
            {
                foreach (AutoInitializeAttribute plugin in type.GetCustomAttributes(typeof(AutoInitializeAttribute), true))
                {
                    plugin.InitializerType = type;

                    if (!IsRemoved(type))
                        pluginDefinitions.Add(plugin);
                }
            }

            // configured plugins
            //foreach (PluginInitializerElement configElement in addedInitializers)
            //{
            //    Type pluginType = Type.GetType(configElement.Type);
            //    if (pluginType == null)
            //        throw new Exception(string.Format("Could not find the configured plugin initializer type '{0}'", configElement.Type));
            //    if (typeof(IPluginDefinition).IsAssignableFrom(pluginType))
            //        throw new Exception(string.Format("The configured plugin initializer type '{0}' is not a valid plugin initializer since it doesn't implement the IPluginDefinition interface.", configElement.Type));

            //    PluginAttribute plugin = new PluginAttribute();
            //    plugin.Name = configElement.Name;
            //    plugin.InitializerType = pluginType;
            //    plugin.Title = "Configured plugin " + configElement.Name;
            //    pluginDefinitions.Add(plugin);
            //}

            return pluginDefinitions;
        }



        /// <summary>Invokes the initialize method on the supplied plugins.</summary>
        public void InitializePlugins(IEngine engine, IEnumerable<IPluginDefinition> plugins)
        {
            List<Exception> exceptions = new List<Exception>();
            foreach (IPluginDefinition plugin in plugins)
            {
                try
                {
                    plugin.Initialize(engine);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
            {
                string message = "While initializing {0} plugin(s) threw an exception. Please review the stack trace to find out what went wrong.";
                message = string.Format(message, exceptions.Count);

                foreach (Exception ex in exceptions)
                    message += Environment.NewLine + Environment.NewLine + "- " + ex.Message;

                throw new PluginInitializationException(message, exceptions.ToArray());
            }
        }

        private bool IsRemoved(Type pluginType)
        {
            //foreach (PluginInitializerElement configElement in removedInitializers)
            //{
            //    if (configElement.Name == pluginType.Name)
            //        return true;
            //    if (!string.IsNullOrEmpty(configElement.Type) && Type.GetType(configElement.Type) == pluginType)
            //        return true;
            //}
            return false;
        }
    }
}
