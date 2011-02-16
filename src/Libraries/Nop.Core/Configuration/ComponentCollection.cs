using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// Services to register instead of/in addition to existing Nop services.
    /// </summary>
    [ConfigurationCollection(typeof(ComponentElement))]
    public class ComponentCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ComponentElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            ComponentElement se = element as ComponentElement;

            if (!string.IsNullOrEmpty(se.Service))
                return se.Service;

            return se.Implementation;
        }
    }
}
