using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Infrastructure;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Base class for attributes which can create an instance of the plugin 
    /// initializer and execute it.
    /// </summary>
    public abstract class InitializerCreatingAttribute : Attribute, IPluginDefinition
    {
        private Type initializerType = null;

        public Type InitializerType
        {
            get { return initializerType; }
            set { initializerType = value; }
        }

        public virtual void Initialize(IEngine engine)
        {
            if (InitializerType == null) throw new ArgumentNullException("InitializerType");

            CreateInitializer().Initialize(engine);
        }

        /// <summary>Creates an instance of the initializer defined by this attribute.</summary>
        /// <returns>A new initializer instance.</returns>
        protected virtual IPluginInitializer CreateInitializer()
        {
            return (IPluginInitializer)Activator.CreateInstance(InitializerType);
        }

    }
}
