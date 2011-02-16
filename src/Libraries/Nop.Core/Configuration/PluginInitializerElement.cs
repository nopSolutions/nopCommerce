using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Nop.Core.Configuration
{
    public class PluginInitializerElement : NamedElement
    {
        public PluginInitializerElement()
        {
        }

        public PluginInitializerElement(string name, string type)
        {
            Name = name;
            Type = type;
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }
    }
}
