using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Infrastructure;
using Autofac;

namespace Nop.Core.Plugins
{
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
