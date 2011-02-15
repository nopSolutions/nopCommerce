using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Nop.Core.Configuration
{
    public class HostSection : ConfigurationSectionBase
    {
        [ConfigurationProperty("serverAddress")]
        public string ServerAddress
        {
            get { return (string)base["serverAddress"]; }
            set { base["serverAddress"] = value; }
        }

        /// <summary>Use wildcard matching for hostnames, e.g. both nopcommerce.com and www.nopcommerce.com.</summary>
        [ConfigurationProperty("wildcards", DefaultValue = false)]
        public bool Wildcards
        {
            get { return (bool)base["wildcards"]; }
            set { base["wildcards"] = value; }
        }

        /// <summary>Whether the current application is running in a web context. This affects how database sessions are stored during a request.</summary>
        [ConfigurationProperty("isWeb", DefaultValue = true)]
        public bool IsWeb
        {
            get { return (bool)base["isWeb"]; }
            set { base["isWeb"] = value; }
        }
    }
}
