using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Nop.Core.Http;

namespace Nop.Core.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Add exception handling
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        /// <param name="useDetailedExceptionPage">Whether to use detailed exception page</param>
        public static void UseExceptionHandler(this IApplicationBuilder application, bool useDetailedExceptionPage)
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
        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 404 status code that do not have a body
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UsePageNotFound(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(async context =>
            {
                //handle 404 Not Found
                if (context.HttpContext.Response.StatusCode == 404)
                {
                    //get original path and query
                    var originalPath = context.HttpContext.Request.Path;
                    var originalQueryString = context.HttpContext.Request.QueryString;

                    //store the original paths in special feature, so we can use it later
                    context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(new StatusCodeReExecuteFeature()
                    {
                        OriginalPathBase = context.HttpContext.Request.PathBase.Value,
                        OriginalPath = originalPath.Value,
                        OriginalQueryString = originalQueryString.HasValue ? originalQueryString.Value : null,
                    });

                    //get new path
                    context.HttpContext.Request.Path = "/page-not-found";
                    context.HttpContext.Request.QueryString = QueryString.Empty;

                    try
                    {
                        //re-execute request with new path
                        await context.Next(context.HttpContext);
                    }
                    finally
                    {
                        //return original path to request
                        context.HttpContext.Request.QueryString = originalQueryString;
                        context.HttpContext.Request.Path = originalPath;
                        context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(null);
                    }
                }
            });
        }

        /// <summary>
        /// Configure middleware checking whether requested page is keep alive page
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseKeepAlive(this IApplicationBuilder application)
        {
            application.UseMiddleware<KeepAliveMiddleware>();
        }

        /// <summary>
        /// Configure middleware checking whether database is installed
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseInstallUrl(this IApplicationBuilder application)
        {
            application.UseMiddleware<InstallUrlMiddleware>();
        }

        /// <summary>
        /// Configure access to static HTTP context
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseStaticHttpContext(this IApplicationBuilder application)
        {
            //get registered HTTP context accessor
            var httpContextAccessor = EngineContext.Current.Resolve<IHttpContextAccessor>();

            //configure HTTP context
            Nop.Core.Http.HttpContext.Configure(httpContextAccessor);
        }
    }
}
