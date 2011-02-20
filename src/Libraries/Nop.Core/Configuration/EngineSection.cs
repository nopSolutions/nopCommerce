
using System.Configuration;
using Nop.Core.Configuration;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// Configuration related to inversion of control and the dynamic aspects
    /// </summary>
    public class EngineSection : ConfigurationSectionBase
    {
        /// <summary>A custom <see cref="IEngine"/> to manage the application instead of the default.</summary>
        [ConfigurationProperty("engineType")]
        public string EngineType
        {
            get { return (string)base["engineType"]; }
            set { base["engineType"] = value; }
        }

        /// <summary>In addition to configured assemblies examine and load assemblies in the bin directory.</summary>
        [ConfigurationProperty("dynamicDiscovery", DefaultValue = true)]
        public bool DynamicDiscovery
        {
            get { return (bool)base["dynamicDiscovery"]; }
            set { base["dynamicDiscovery"] = value; }
        }

        /// <summary>Additional assemblies assemblies investigated while investigating the environemnt.</summary>
        [ConfigurationProperty("assemblies")]
        public AssemblyCollection Assemblies
        {
            get { return (AssemblyCollection)base["assemblies"]; }
            set { base["assemblies"] = value; }
        }

        /// <summary>Scheduler related configuration.</summary>
        [ConfigurationProperty("scheduler")]
        public SchedulerElement Scheduler
        {
            get { return (SchedulerElement)base["scheduler"]; }
            set { base["scheduler"] = value; }
        }

        /// <summary>A collection of services that are registered in the container before the default ones. This is a place through which core services can be replaced.</summary>
        [ConfigurationProperty("components")]
        public ComponentCollection Components
        {
            get { return (ComponentCollection)base["components"]; }
            set { base["components"] = value; }
        }

        /// <summary>Add or remove plugin initializers. This is most commonly used to remove automatic plugin initializers in an external assembly.</summary>
        [ConfigurationProperty("pluginInitializers")]
        public PluginInitializerCollection PluginInitializers
        {
            get { return (PluginInitializerCollection)base["pluginInitializers"]; }
            set { base["pluginInitializers"] = value; }
        }

        /// <summary>Add or remove UI plugins. This is most commonly used to remove unwanted toolbar buttons.</summary>
        [ConfigurationProperty("interfacePlugins")]
        public InterfacePluginCollection InterfacePlugins
        {
            get { return (InterfacePluginCollection)base["interfacePlugins"]; }
            set { base["interfacePlugins"] = value; }
        }
    }
}
