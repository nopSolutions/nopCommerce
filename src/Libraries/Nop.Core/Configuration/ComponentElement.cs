using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// A service definition to add to the N2 container. This can be used to replace core services.
    /// </summary>
    public class ComponentElement : ConfigurationElement
    {
        /// <summary>Optional name of the component</summary>
        [ConfigurationProperty("key")]
        public string Key
        {
            get { return (string)base["key"]; }
            set { base["key"] = value; }
        }

        /// <summary>Class and name and assembly of the service interface, e.g. "MyNamespace.MyInterface, MyAssembly"</summary>
        [ConfigurationProperty("service")]
        public string Service
        {
            get { return (string)base["service"]; }
            set { base["service"] = value; }
        }

        /// <summary>Class and name and assembly of the implementation to add, e.g. "MyNamespace.MyClass, MyAssembly". If no service is defined the class itself will represent the service.</summary>
        [ConfigurationProperty("implementation", IsRequired = true)]
        public string Implementation
        {
            get { return (string)base["implementation"]; }
            set { base["implementation"] = value; }
        }

        /// <summary>A collection of properties (eg configuration values) that should be registered with the service and used for its instantiation</summary>
        [ConfigurationProperty("parameters")]
        public ComponentParameterCollection Parameters
        {
            get { return (ComponentParameterCollection)base["parameters"]; }
            set { base["parameters"] = value; }
        }
    }
}
