using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Web
{
    /// <summary>
    /// Represents startup class of application
    /// </summary>
    public class Startup
    {
        #region Properties

        /// <summary>
        /// Get Configuration of the application
        /// </summary>
        public IConfiguration Configuration { get; }

        #endregion

        #region Ctor

        public Startup(IConfiguration configuration)
        {
            //set configuration
            Configuration = configuration;
        }

        #endregion

        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.ConfigureApplicationServices(Configuration);
        }

        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            application.ConfigureRequestPipeline();
            
            //install plugins
            var engine = EngineContext.Current;
            var logger = engine.Resolve<ILogger>();
            var customerActivityService = engine.Resolve<ICustomerActivityService>();
            var customerService = engine.Resolve<ICustomerService>();
            var workContext = engine.Resolve<IWorkContext>();
            var localizationService = engine.Resolve<ILocalizationService>();
           
            foreach (var installedPlugin in PluginManager.GetLastInstalledPlugins)
            {
                var customer = (installedPlugin.CustomerGuid.HasValue ? customerService.GetCustomerByGuid(installedPlugin.CustomerGuid.Value) : null) ?? workContext.CurrentCustomer;
                customerActivityService.InsertActivity(customer, "InstallNewPlugin", string.Format(localizationService.GetResource("ActivityLog.InstallNewPlugin"), installedPlugin.SystemName));
            }

            //log plugin installation errors
            foreach (var descriptor in PluginManager.ReferencedPlugins.Where(pluginDescriptor=>pluginDescriptor.Error != null))
            {
                logger.Error(string.Format(localizationService.GetResource("ActivityLog.NotInstalledNewPluginError"), descriptor.SystemName), descriptor.Error);
                descriptor.Error = null;
            }
        }
    }
}
