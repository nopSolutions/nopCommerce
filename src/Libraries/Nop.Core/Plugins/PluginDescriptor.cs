using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Infrastructure;
using Autofac;

namespace Nop.Core.Plugins
{
    public abstract class PluginDescriptor : IComparable<PluginDescriptor>
    {
        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public virtual string FriendlyName { get; protected set; }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public virtual string SystemName { get; protected set; }

        /// <summary>
        /// Gets or sets the version
        /// </summary>
        public virtual string Version { get; protected set; }

        /// <summary>
        /// Gets or sets the author
        /// </summary>
        public virtual string Author { get; protected set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; protected set; }

        public abstract Type PluginType { get; }

        public abstract T Instance<T>() where T : class, IPlugin;

        public IPlugin Instance()
        {
            return Instance<IPlugin>();
        }
        
        public int CompareTo(PluginDescriptor other)
        {
            return DisplayOrder - other.DisplayOrder;
        }
    }
}
