using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Nop.Core.Plugins
{
    public abstract class BasePlugin : IPlugin
    {
        protected BasePlugin()
        {
        }
        
        /// <summary>
        /// Gets or sets the plugin descriptor
        /// </summary>
        public virtual PluginDescriptor PluginDescriptor { get; set; }

        /// <summary>
        /// Install plugin
        /// </summary>
        public virtual void Install() { }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public virtual void Uninstall() { }

    }
}
