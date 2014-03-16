using System;
using Autofac;
using Autofac.Integration.Mvc;
using Nop.Core.Data;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;

namespace Nop.Web.Framework.Mvc
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
        public static void RegisterPluginDataContext<T>(this IDependencyRegistrar dependencyRegistrar,
            ContainerBuilder builder, string contextName)
             where T: IDbContext
        {
            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                //register named context
                builder.Register<IDbContext>(c => (IDbContext)Activator.CreateInstance(typeof(T), new object[] { dataProviderSettings.DataConnectionString }))
                    .Named<IDbContext>(contextName)
                    .InstancePerHttpRequest();

                builder.Register<T>(c => (T)Activator.CreateInstance(typeof(T), new object[] { dataProviderSettings.DataConnectionString }))
                    .InstancePerHttpRequest();
            }
            else
            {
                //register named context
                builder.Register<IDbContext>(c => (T)Activator.CreateInstance(typeof(T), new object[] { c.Resolve<DataSettings>().DataConnectionString }))
                    .Named<IDbContext>(contextName)
                    .InstancePerHttpRequest();

                builder.Register<T>(c => (T)Activator.CreateInstance(typeof(T), new object[] { c.Resolve<DataSettings>().DataConnectionString }))
                    .InstancePerHttpRequest();
            }
        }
    }
}
