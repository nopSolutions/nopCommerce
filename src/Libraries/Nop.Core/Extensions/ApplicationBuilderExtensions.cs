using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Nop.Core.Infrastructure;

namespace Nop.Core.Extensions
{
    /// <summary>
    /// Represents extensions of IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
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
            HttpContext.Configure(httpContextAccessor);

            return application;
        }
    }
}
