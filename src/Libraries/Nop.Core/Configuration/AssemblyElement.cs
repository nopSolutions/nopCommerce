using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Nop.Core.Configuration
{
    public class AssemblyElement : ConfigurationElement, IIdentifiable
    {
        static ConfigurationPropertyCollection properties;
        static ConfigurationProperty propAssembly;

        static AssemblyElement()
        {
            propAssembly = new ConfigurationProperty("assembly", typeof(string), null, null, new StringValidator(1, 9999), ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
            properties = new ConfigurationPropertyCollection();
            properties.Add(propAssembly);
        }

        public AssemblyElement()
        {
        }

        public AssemblyElement(string assemblyName)
        {
            Assembly = assemblyName;
        }

        [StringValidator(MinLength = 1), ConfigurationProperty("assembly", IsRequired = true, IsKey = true, DefaultValue = "")]
        public string Assembly
        {
            get { return (string)base[propAssembly]; }
            set { base[propAssembly] = value; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return properties; }
        }

        #region IIdentifiable Members

        object IIdentifiable.ElementKey
        {
            get { return Assembly; }
            set { Assembly = (string)value; }
        }

        #endregion
    }
}
