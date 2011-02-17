using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Nop.Core.Web;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the 
    /// various services composing the Nop engine. Edit functionality, modules
    /// and implementations access most Nop functionality through this 
    /// interface.
    /// </summary>
    public interface IEngine
    {
        ContainerManager ContainerManager { get; }

        /// <summary>Contextual data associated with the current request.</summary>
        IWebContext RequestContext { get; }

        /// <summary>
        /// Initialize components and plugins in the nop environment.
        /// </summary>
        void Initialize();

        T Resolve<T>() where T : class;

        object Resolve(Type type);

        Array ResolveAll(Type serviceType);

        T[] ResolveAll<T>();
    }

    public enum ComponentLifeStyle
    {
        Singleton = 0,
        Transient = 1,
        LifetimeScope
    }
}
