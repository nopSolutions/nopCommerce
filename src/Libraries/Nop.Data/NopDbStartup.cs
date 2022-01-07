using System.Linq;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data.Mapping;
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
            var typeFinder = Singleton<ITypeFinder>.Instance;
            var mAssemblies = typeFinder.FindClassesOfType<MigrationBase>()
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
                .AddSingleton<IMigrationManager, MigrationManager>()
                .AddSingleton<IConventionSet, NopConventionSet>()
                .AddTransient<IMappingEntityAccessor>(x => x.GetRequiredService<IDataProviderManager>().DataProvider)
                .ConfigureRunner(rb =>
                {
                    var migrationRunnerBuilder = rb.WithVersionTable(new MigrationVersionInfo());
                    var dataSettings = DataSettingsManager.LoadSettings();
                    switch (dataSettings.DataProvider)
                    {
                        case DataProviderType.SqlServer:
                            migrationRunnerBuilder.AddSqlServer();
                            break;
                        case DataProviderType.MySql:
                            migrationRunnerBuilder.AddMySql5();
                            break;
                        case DataProviderType.PostgreSQL:
                            migrationRunnerBuilder.AddPostgres();
                            break;
                        default:
                            if (!DataSettingsManager.IsDatabaseInstalled())
                            {
                                // This is for the first installation
                                migrationRunnerBuilder.AddSqlServer().AddMySql5().AddPostgres();
                            }
                            else
                            {
                                throw new NopException($"Not supported data provider name: '{dataSettings.DataProvider}'");
                            }
                            break;
                    }

                    // define the assembly containing the migrations
                    migrationRunnerBuilder.ScanIn(mAssemblies).For.Migrations();
                });
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
