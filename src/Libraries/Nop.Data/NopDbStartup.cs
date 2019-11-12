using System;
using LinqToDB.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Nop.Data
{
    /// <summary>
    /// Represents object for the configuring DB context on application startup
    /// </summary>
    public class NopDbStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            Singleton<MappingSchema>.Instance = new MappingSchema();

            var mappingBuilder = new FluentMappingBuilder(Singleton<MappingSchema>.Instance);

            //find database mapping configuration by other assemblies
            var typeFinder = new AppDomainTypeFinder();
            var typeConfigurations = typeFinder.FindClassesOfType<IMappingConfiguration>();

            foreach (var typeConfiguration in typeConfigurations)
            {
                var mappingConfiguration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration);
                mappingConfiguration.ApplyConfiguration(mappingBuilder);
            }
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 10;
    }
}