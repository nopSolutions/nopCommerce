using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Data;
using Nop.Core.Domain;
using Nop.Core.Infrastructure;
using Nop.Services.Security;
using Nop.Web.Framework.Globalization;
using Nop.Web.Framework.Mvc.Routing;
using StackExchange.Profiling;
using StackExchange.Profiling.Storage;

namespace Nop.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void ConfigureRequestPipeline(this IApplicationBuilder application)
        {
            EngineContext.Current.ConfigureRequestPipeline(application);
        }

        /// <summary>
        /// Set current culture info
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseCulture(this IApplicationBuilder application)
        {
            application.UseMiddleware<CultureMiddleware>();
        }

        /// <summary>
        /// Configure MVC routing
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseNopMvc(this IApplicationBuilder application)
        {
            application.UseMvc(routeBuilder =>
            {
                //register all routes
                EngineContext.Current.Resolve<IRoutePublisher>().RegisterRoutes(routeBuilder);
            });
        }

        /// <summary>
        /// Create and configure MiniProfiler service
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseMiniProfiler(this IApplicationBuilder application)
        {
            //whether database is already installed
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            //whether MiniProfiler should be displayed
            if (EngineContext.Current.Resolve<StoreInformationSettings>().DisplayMiniProfilerInPublicStore)
            {
                application.UseMiniProfiler(new MiniProfilerOptions
                {
                    //use memory cache provider for storing each result
                    Storage = new MemoryCacheStorage(TimeSpan.FromMinutes(60)),

                    //determine who can access the MiniProfiler results
                    ResultsAuthorize = request =>
                        !EngineContext.Current.Resolve<StoreInformationSettings>().DisplayMiniProfilerForAdminOnly ||
                        EngineContext.Current.Resolve<IPermissionService>().Authorize(StandardPermissionProvider.AccessAdminPanel)
                });
            }
        }
    }
}
