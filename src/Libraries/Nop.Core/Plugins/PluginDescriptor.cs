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
        public string Name { get; protected set; }
        public int SortOrder { get; protected set; }

        #region Properties (1)

        public abstract Type PluginType { get; }

        #endregion Properties

        #region Methods (2)

        // Public Methods (2) 

        public abstract T Instance<T>() where T : class, IPlugin;

        public IPlugin Instance()
        {
            return Instance<IPlugin>();
        }

        #endregion Methods

        public int CompareTo(PluginDescriptor other)
        {
            return SortOrder - other.SortOrder;
        }
    }

    public class PluginAttributeDescriptor : PluginDescriptor
    {
        #region Fields (2)

        private IPlugin _pluginInstance = null;
        private Type _pluginType = null;

        #endregion Fields

        #region Constructors (1)

        public PluginAttributeDescriptor(IPlugin pluginInstance)
        {
            _pluginInstance = pluginInstance;
            _pluginType = _pluginInstance.GetType();
            var instance = Instance<IPlugin>();
            Name = instance.Name;
            SortOrder = instance.SortOrder;
        }

        #endregion Constructors

        #region Properties (1)

        public override Type PluginType
        {
            get { return _pluginType; }
        }

        #endregion Properties

        #region Methods (1)

        // Public Methods (1) 

        public override T Instance<T>()
        {
            return _pluginInstance as T;
        }

        #endregion Methods
    }

    public class PluginImplementationDescriptor : PluginDescriptor
    {
        #region Fields (1)

        private Type _pluginType = null;

        #endregion Fields

        #region Constructors (1)

        public PluginImplementationDescriptor(Type pluginType)
        {
            _pluginType = pluginType;
            var instance = Instance<IPlugin>();
            Name = instance.Name;
            SortOrder = instance.SortOrder;
        }

        #endregion Constructors

        #region Properties (1)

        public override Type PluginType
        {
            get { return _pluginType; }
        }

        #endregion Properties

        #region Methods (1)

        // Public Methods (1) 

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

        #endregion Methods
    }
}
