using System;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
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
        /// <param name="services">Services</param>
        public static void RegisterPluginDataContext<T>(this IDependencyRegistrar dependencyRegistrar,
            IServiceCollection services) where T: IDbContext
        {
            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                //register named context
                services.AddScoped(typeof(T), c => (IDbContext)Activator.CreateInstance(typeof(T), new object[] { dataProviderSettings.DataConnectionString }));
            }
            else
            {
                //register named context
                services.AddScoped(typeof(T), c => (T)Activator.CreateInstance(typeof(T), new object[] { c.GetService<DataSettings>().DataConnectionString }));
            }
        }
        /// <summary>
        /// Register custom repository for a plugin
        /// </summary>
        /// <typeparam name="T">Entity class</typeparam>
        /// <typeparam name="K">Class implementing IDbContext</typeparam>
        /// <param name="dependencyRegistrar">Dependency registrar</param>
        /// <param name="services">Services</param>
        public static void RegisterPluginRepository<T, K>(this IDependencyRegistrar dependencyRegistrar,
            IServiceCollection services) where T : BaseEntity where K: IDbContext
        {
            services.AddScoped<IRepository<T>, EfRepository<T>>(c => (EfRepository<T>)Activator.CreateInstance(typeof(EfRepository<T>), new object[] { c.GetService<K>() }));
        }
    }
}
