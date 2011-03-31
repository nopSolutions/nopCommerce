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

    public class PluginAttributeDescriptor : PluginDescriptor
    {
        private IPlugin _pluginInstance = null;
        private Type _pluginType = null;

        public PluginAttributeDescriptor(IPlugin pluginInstance)
        {
            this._pluginInstance = pluginInstance;
            this._pluginType = _pluginInstance.GetType();
            var instance = Instance<IPlugin>();
            this.FriendlyName = instance.FriendlyName;
            this.SystemName = instance.SystemName;
            this.Version = instance.Version;
            this.Author = instance.Author;
            this.DisplayOrder = instance.DisplayOrder;
        }

        public override Type PluginType
        {
            get { return _pluginType; }
        }

        public override T Instance<T>()
        {
            return _pluginInstance as T;
        }
    }

    public class PluginImplementationDescriptor : PluginDescriptor
    {
        private Type _pluginType = null;

        public PluginImplementationDescriptor(Type pluginType)
        {
            this._pluginType = pluginType;
            var instance = Instance<IPlugin>();
            this.FriendlyName = instance.FriendlyName;
            this.SystemName = instance.SystemName;
            this.Version = instance.Version;
            this.Author = instance.Author;
            this.DisplayOrder = instance.DisplayOrder;
        }

        public override Type PluginType
        {
            get { return _pluginType; }
        }

        public override T Instance<T>()
        {
            object instance;
            if (EngineContext.Current.ContainerManager.Scope().TryResolve(_pluginType, out instance))
            {
                return instance as T;
            }
            try
            {
                return Activator.CreateInstance(_pluginType) as T;
            }
            catch (MissingMethodException ex)
            {
                return EngineContext.Current.ContainerManager.ResolveUnregistered(_pluginType) as T;
            }
        }
    }
}
