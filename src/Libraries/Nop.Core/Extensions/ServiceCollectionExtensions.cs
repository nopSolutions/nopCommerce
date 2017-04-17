using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.Mapper;
using Nop.Core.Plugins;

namespace Nop.Core.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Initialize an instance of the Nop engine
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors</param>
        /// <param name="nopConfiguration">Startup Nop configuration parameters</param>
        /// <returns>Engine instance</returns>
        public static IEngine InitializeNopEngine(this IServiceCollection services, NopConfig nopConfiguration)
        {
            //create engine
            var engine = EngineContext.Create();

            //and initialize it
            engine.Initialize(nopConfiguration, services);

            return engine;
        }

        /// <summary>
        /// Create, bind and register as service the specified configuration parameters 
        /// </summary>
        /// <typeparam name="TConfig">Configuration parameters</typeparam>
        /// <param name="services">The contract for a collection of service descriptors</param>
        /// <param name="configuration">Set of key/value application configuration properties</param>
        /// <returns>Instance of configuration parameters</returns>
        public static TConfig ConfigureStartupConfig<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            //create instance of config
            var config = new TConfig();

            //bind it to the appropriate section of configuration
            configuration.Bind(config);

            //and register it as a service
            services.AddSingleton(config);

            return config;
        }

        /// <summary>
        /// Register HttpContextAccessor
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors</param>
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// Register and configure AutoMapper
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors</param>
        public static void AddAutoMapper(this IServiceCollection services)
        {
            //find mapper configurations provided by other assemblies
            var mapperConfigurations = EngineContext.Current.Resolve<ITypeFinder>().FindClassesOfType<IMapperConfiguration>();

            //create and sort instances of mapper configurations
            var instances = mapperConfigurations
                .Where(mapperConfiguration => PluginManager.PluginInstalled(mapperConfiguration)) //ignore not installed plugins
                .Select(mapperConfiguration => (IMapperConfiguration)Activator.CreateInstance(mapperConfiguration))
                .OrderBy(mapperConfiguration => mapperConfiguration.Order);

            //get all configuration actions
            var configurationActions = instances.Select(mapperConfiguration => mapperConfiguration.GetConfiguration());

            //register AutoMapper with provided configurations
            services.AddAutoMapper(mapperConfigurationExpression =>
            {
                foreach (var action in configurationActions)
                    action(mapperConfigurationExpression);
            });
        }
    }
}
