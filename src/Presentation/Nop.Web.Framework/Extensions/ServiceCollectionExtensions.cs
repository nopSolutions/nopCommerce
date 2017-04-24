using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using Nop.Web.Framework.FluentValidation;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Framework.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add and configure MVC for the application
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public static IMvcBuilder AddNopMvc(this IServiceCollection services)
        {
            //add basic MVC feature
            var mvcBuilder = services.AddMvc();

            //add custom display metadata provider
            mvcBuilder.AddMvcOptions(options => options.ModelMetadataDetailsProviders.Add(new NopMetadataProvider()));

            //add custom model binder provider (to the top of the provider list)
            mvcBuilder.AddMvcOptions(options => options.ModelBinderProviders.Insert(0, new NopModelBinderProvider()));

#if NET451
            //whether database is already installed
            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                //add themeable view engine
                mvcBuilder.AddViewOptions(options =>
                {
                    options.ViewEngines.Clear();
                    options.ViewEngines.Add(new ThemeableRazorViewEngine());
                });
            }
#endif

            //add fluent validation
            mvcBuilder.AddFluentValidation(configuration => configuration.ValidatorFactoryType = typeof(NopValidatorFactory));

            return mvcBuilder;
        }

        /// <summary>
        /// Create and configure manager of scheduled tasks
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors</param>
        /// <returns>The contract for a collection of service descriptors</returns>
        public static IServiceCollection AddScheduledTasks(this IServiceCollection services)
        {
            //whether database is already installed
            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                //start scheduled tasks
                TaskManager.Instance.Initialize();
                TaskManager.Instance.Start();
            }

            return services;
        }

        /// <summary>
        /// Create 'Application start' record in log
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors</param>
        /// <returns>The contract for a collection of service descriptors</returns>
        public static IServiceCollection LogApplicationStart(this IServiceCollection services)
        {
            //whether database is already installed
            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                try
                {
                    //log application start
                    EngineContext.Current.Resolve<ILogger>().Information("Application started", null, null);
                }
                catch { }
            }

            return services;
        }
    }
}
