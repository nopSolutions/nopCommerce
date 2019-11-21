using System;
using System.Data.SqlClient;
using System.Linq;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Exceptions;
using LinqToDB.Data;
using LinqToDB.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Data.Extensions;

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
            var typeConfigurations = typeFinder.FindClassesOfType<IMappingConfiguration>().ToList();

            foreach (var typeConfiguration in typeConfigurations)
            {
                var mappingConfiguration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration);
                mappingConfiguration.ApplyConfiguration(mappingBuilder);
            }

            //further actions are performed only when the database is installed
            if (!DataSettingsManager.DatabaseIsInstalled)
                return;

            var dataSettings = Singleton<DataSettings>.Instance;

            var connSettings = new Linq2DbSettingsProvider(dataSettings);
            DataConnection.DefaultSettings = connSettings;

            foreach (var typeConfiguration in typeConfigurations)
            {
                var mappingConfiguration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration);
                mappingConfiguration.CreateTableIfNotExists(new NopDataConnection());
            }

            services
                // add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb.SetServer(dataSettings)
                // define the assembly containing the migrations
                .ScanIn(typeConfigurations.Select(p => p.Assembly).Distinct().ToArray()).For.Migrations())
                // enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // build the service provider
                .BuildServiceProvider(false);
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //further actions are performed only when the database is installed
            if (!DataSettingsManager.DatabaseIsInstalled)
                return;

            var runner = EngineContext.Current.Resolve<IMigrationRunner>();

            try
            {
                // execute the migrations
                if (runner.HasMigrationsToApplyUp())
                    runner.MigrateUp();
            }
            catch (MissingMigrationsException)
            {
            }
            catch (Exception ex)
            {
                if (!(ex.InnerException is SqlException))
                    throw;
            }
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 10;
    }
}