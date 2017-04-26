using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Data;
using Nop.Core.Domain;
using Nop.Core.Infrastructure;
using Nop.Services.Security;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Mvc.Routing;
using StackExchange.Profiling;
using StackExchange.Profiling.Storage;

namespace Nop.Web.Framework.Extensions
{
    /// <summary>
    /// Represents extensions of IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Add exception handling
        /// </summary>
        /// <param name="application">Builder that provides the mechanisms to configure an application's request pipeline</param>
        /// <param name="useDetailedExceptionPage">Whether to use detailed exception page</param>
        /// <returns>Builder that provides the mechanisms to configure an application's request pipeline</returns>
        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder application, bool useDetailedExceptionPage)
        {
            if (useDetailedExceptionPage)
            {
                //get detailed exceptions for developing and testing purposes
                application.UseDeveloperExceptionPage();
            }
            else
            {
                //or use special exception handler
                application.UseExceptionHandler("/error");
            }

            return application;
        }

        /// <summary>
        /// Configure MVC routing
        /// </summary>
        /// <param name="application">Builder that provides the mechanisms to configure an application's request pipeline</param>
        /// <returns>Builder that provides the mechanisms to configure an application's request pipeline</returns>
        public static IApplicationBuilder UseNopMvc(this IApplicationBuilder application)
        {
            application.UseMvc(routeBuilder =>
            {
                //register all routes
                EngineContext.Current.Resolve<IRoutePublisher>().RegisterRoutes(routeBuilder);

                //and default one
                routeBuilder.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");
            });

            return application;
        }

        /// <summary>
        /// Create and configure MiniProfiler service
        /// </summary>
        /// <param name="application">Builder that provides the mechanisms to configure an application's request pipeline</param>
        /// <returns>Builder that provides the mechanisms to configure an application's request pipeline</returns>
        public static IApplicationBuilder UseMiniProfiler(this IApplicationBuilder application)
        {
            //whether database is already installed
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return application;

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

            return application;
        }
    }
}
