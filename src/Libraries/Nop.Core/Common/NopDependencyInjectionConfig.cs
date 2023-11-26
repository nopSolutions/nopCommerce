using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Nop.Core.Common
{
    /// <summary>
    /// Containing extension methods for configuring dependency injection in generic way
    /// </summary>
    public static partial class NopDependencyInjectionConfig
    {
        #region Methods
        /// <summary>
        /// Registers all implementations of a specified interface with the provided service lifetime.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="interfaceType">The interface type whose implementations need to be registered.</param>
        /// <param name="lifetime">The service lifetime (Transient, Scoped, Singleton).</param>
        /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection RegisterImplementations(this IServiceCollection services, Type interfaceType, ServiceLifetime lifetime)
        {
            var implementationTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => interfaceType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .Select(type => new { Interface = type.GetInterfaces().OrderByDescending(i => i.GetInterfaces().Length).FirstOrDefault(), Implementation = type })
                .Where(type => type.Interface != null && interfaceType.IsAssignableFrom(type.Interface));

            foreach (var implementationType in implementationTypes)
                services.RegisterService(implementationType.Interface!, implementationType.Implementation, lifetime);

            return services;
        }
        /// <summary>
        /// Registers a service with the specified interface, implementation, and service lifetime.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="interfaceType">The interface type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="lifetime">The service lifetime (Transient, Scoped, Singleton).</param>
        /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection RegisterService(this IServiceCollection services, Type interfaceType, Type implementationType, ServiceLifetime lifetime)
        {
            return lifetime switch
            {
                ServiceLifetime.Transient => services.AddTransient(interfaceType, implementationType),
                ServiceLifetime.Scoped => services.AddScoped(interfaceType, implementationType),
                ServiceLifetime.Singleton => services.AddSingleton(interfaceType, implementationType),
                _ => throw new ArgumentException("Invalid lifetime specified", nameof(lifetime))
            };
        }
        #endregion
    }
}
