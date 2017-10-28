using System;
using Autofac;
using Nop.Core.Data;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;

namespace Nop.Web.Framework.Infrastructure
{
    /// <summary>
    /// Extensions for DependencyRegistrar
    /// </summary>
    public static class DependencyRegistrarExtensions
    {
        /// <summary>
        /// Register custom DataContext for a plugin
        /// </summary>
        /// <typeparam name="T">Class implementing IDbContext</typeparam>
        /// <param name="dependencyRegistrar">Dependency registrar</param>
        /// <param name="builder">Builder</param>
        /// <param name="contextName">Context name</param>
        /// <param name="filePath">File path to load settings (connection string); pass null to use default settings file path</param>
        /// <param name="reloadSettings">Indicates whether to reload data, if they already loaded (connection string)</param>
        public static void RegisterPluginDataContext<T>(this IDependencyRegistrar dependencyRegistrar,
            ContainerBuilder builder, string contextName, string filePath = null, bool reloadSettings = false) where T : IDbContext
        {
            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings(filePath, reloadSettings);

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                //register named context
                builder.Register(c => (IDbContext)Activator.CreateInstance(typeof(T), new object[] { dataProviderSettings.DataConnectionString }))
                    .Named<IDbContext>(contextName)
                    .InstancePerLifetimeScope();

                builder.Register(c => (T)Activator.CreateInstance(typeof(T), new object[] { dataProviderSettings.DataConnectionString }))
                    .InstancePerLifetimeScope();
            }
            else
            {
                //register named context
                builder.Register(c => (T)Activator.CreateInstance(typeof(T), new object[] { c.Resolve<DataSettings>().DataConnectionString }))
                    .Named<IDbContext>(contextName)
                    .InstancePerLifetimeScope();

                builder.Register(c => (T)Activator.CreateInstance(typeof(T), new object[] { c.Resolve<DataSettings>().DataConnectionString }))
                    .InstancePerLifetimeScope();
            }
        }
    }
}
