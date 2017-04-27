using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Nop.Core.Http;
using Nop.Core.Infrastructure;

namespace Nop.Core.Extensions
{
    /// <summary>
    /// Represents extensions of IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Configure middleware checking whether requested page is keep alive page
        /// </summary>
        /// <param name="application">Builder that provides the mechanisms to configure an application's request pipeline</param>
        /// <returns>Builder that provides the mechanisms to configure an application's request pipeline</returns>
        public static IApplicationBuilder UseKeepAlive(this IApplicationBuilder application)
        {
            return application.UseMiddleware<KeepAliveMiddleware>();
        }

        /// <summary>
        /// Configure middleware checking whether database is installed
        /// </summary>
        /// <param name="application">Builder that provides the mechanisms to configure an application's request pipeline</param>
        /// <returns>Builder that provides the mechanisms to configure an application's request pipeline</returns>
        public static IApplicationBuilder UseInstallUrl(this IApplicationBuilder application)
        {
            return application.UseMiddleware<InstallUrlMiddleware>();
        }

        /// <summary>
        /// Configure access to static HTTP context
        /// </summary>
        /// <param name="application">Builder that provides the mechanisms to configure an application's request pipeline</param>
        /// <returns>Builder that provides the mechanisms to configure an application's request pipeline</returns>
        public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder application)
        {
            //get registered HTTP context accessor
            var httpContextAccessor = EngineContext.Current.Resolve<IHttpContextAccessor>();

            //configure HTTP context
            Nop.Core.Http.HttpContext.Configure(httpContextAccessor);

            return application;
        }
    }
}
