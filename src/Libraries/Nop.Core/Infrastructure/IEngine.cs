using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;

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
        /// <summary>
        /// Gets or sets service provider
        /// </summary>
        IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Initialize components and plugins in the nop environment.
        /// </summary>
        /// <param name="nopConfiguration">Startup Nop configuration parameters</param>
        /// <param name="services">The contract for a collection of service descriptors</param>
        void Initialize(NopConfig config, IServiceCollection services);

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">Type of resolved service</typeparam>
        /// <returns>Resolved service</returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <param name="type">Type of resolved service</param>
        /// <returns>Resolved service</returns>
        object Resolve(Type type);

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">Type of resolved services</typeparam>
        /// <returns>Collection of resolved services</returns>
        IEnumerable<T> ResolveAll<T>();

        /// <summary>
        /// Resolve unregistered service
        /// </summary>
        /// <param name="type">Type of service</param>
        /// <returns>Resolved service</returns>
        object ResolveUnregistered(Type type);
    }
}
