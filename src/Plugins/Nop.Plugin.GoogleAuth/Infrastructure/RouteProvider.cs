using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Plugin.GoogleAuth;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.GoogleAuth.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(GoogleAuthenticationDefaults.DataDeletionCallbackRoute, $"facebook/data-deletion-callback/",
                new { controller = "GoogleDataDeletion", action = "DataDeletionCallback" });

            endpointRouteBuilder.MapControllerRoute(GoogleAuthenticationDefaults.DataDeletionStatusCheckRoute, $"facebook/data-deletion-status-check/{{earId:min(0)}}",
                new { controller = "GoogleAuthentication", action = "DataDeletionStatusCheck" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}
