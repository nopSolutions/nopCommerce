using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Web;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the 
    /// various services composing the N2 engine. Edit functionality, modules
    /// and implementations access most N2 functionality through this 
    /// interface.
    /// </summary>
    public interface IEngine
    {
        IServiceContainer Container { get; }

        /// <summary>Contextual data associated with the current request.</summary>
        IWebContext RequestContext { get; }

        /// <summary>
        /// Initialize components and plugins in the nop environment.
        /// </summary>
        void Initialize();

        T Resolve<T>();

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
