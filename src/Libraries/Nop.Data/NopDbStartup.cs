using System;
using System.Linq;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

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
            var mappingBuilder = new FluentMappingBuilder(NopDataConnection.AdditionalSchema);
            
            //find database mapping configuration by other assemblies
            var typeFinder = new AppDomainTypeFinder();
            var typeConfigurations = typeFinder.FindClassesOfType<IMappingConfiguration>().ToList();

            foreach (var typeConfiguration in typeConfigurations)
            {
                var mappingConfiguration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration);
                mappingConfiguration.ApplyConfiguration(mappingBuilder);
            }

            services
                // add common FluentMigrator services
                .AddFluentMigratorCore()
                .AddScoped<IProcessorAccessor, NopProcessorAccessor>()
                // set accessor for the connection string
                .AddScoped<IConnectionStringAccessor>(x => DataSettingsManager.LoadSettings())
                .AddScoped<IMigrationManager, MigrationManager>()
                .AddSingleton<IConventionSet, NopConventionSet>()
                .ConfigureRunner(rb => 
                    rb.SetServers()
                    .WithVersionTable(new MigrationVersionInfo())
                    // define the assembly containing the migrations
                    .ScanIn(typeConfigurations.Select(p => p.Assembly).Distinct().ToArray()).For.Migrations());

            //further actions are performed only when the database is installed
            if (!DataSettingsManager.DatabaseIsInstalled)
                return;

            DataConnection.DefaultSettings = Singleton<DataSettings>.Instance;

            //TODO: 
            MappingSchema.Default.SetConvertExpression<string, Guid>(strGuid => new Guid(strGuid));

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