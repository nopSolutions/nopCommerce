using System.Linq;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
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
            var mAssemblies = new AppDomainTypeFinder().FindClassesOfType<MigrationBase>()
                .Select(t => t.Assembly)
                .Where(assembly => !assembly.FullName.Contains("FluentMigrator.Runner"))
                .Distinct()
                .ToArray();

            services
                // add common FluentMigrator services
                .AddFluentMigratorCore()
                .AddScoped<IProcessorAccessor, NopProcessorAccessor>()
                // set accessor for the connection string
                .AddScoped<IConnectionStringAccessor>(x => DataSettingsManager.LoadSettings())
                .AddScoped<IMigrationManager, MigrationManager>()
                .AddSingleton<IConventionSet, NopConventionSet>()
                .ConfigureRunner(rb =>
                    rb.WithVersionTable(new MigrationVersionInfo()).AddSqlServer().AddMySql5()
                        // define the assembly containing the migrations
                        .ScanIn(mAssemblies).For.Migrations());
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