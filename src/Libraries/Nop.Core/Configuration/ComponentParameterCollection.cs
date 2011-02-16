using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Nop.Core.Configuration
{
    public class ComponentParameterCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ComponentParameterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var pe = (ComponentParameterElement)element;

            return pe.Name;
        }

        public IDictionary<string, string> ToDictionary()
        {
            var result = new Dictionary<string, string>();

            foreach (ComponentParameterElement value in this)
            {
                result.Add(value.Name, value.Value);
            }
            return result;
        }
    }
}
