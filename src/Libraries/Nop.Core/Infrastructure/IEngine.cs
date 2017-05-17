using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the various services composing the Nop engine. 
    /// Edit functionality, modules and implementations access most Nop functionality through this interface.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Initialize engine
        /// </summary>
        /// <param name="hostingEnvironment">Web hosting environment an application is running in</param>
        /// <param name="applicationPartManager">Application part manager</param>
        void Initialize(IHostingEnvironment hostingEnvironment, ApplicationPartManager applicationPartManager);

        /// <summary>
        /// Add and configure services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        /// <returns>Service provider</returns>
        IServiceProvider ConfigureServices(IServiceCollection services, IConfigurationRoot configuration);

        /// <summary>
        /// Configure HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        void ConfigureRequestPipeline(IApplicationBuilder application);

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
