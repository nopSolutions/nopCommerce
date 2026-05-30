using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.ExternalAuth.Facebook.Infrastructure;

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
        endpointRouteBuilder.MapControllerRoute(FacebookAuthenticationDefaults.DataDeletionCallbackRoute, $"facebook/data-deletion-callback/",
            new { controller = "FacebookDataDeletion", action = "DataDeletionCallback" });

        endpointRouteBuilder.MapControllerRoute(FacebookAuthenticationDefaults.DataDeletionStatusCheckRoute, $"facebook/data-deletion-status-check/{{earId:min(0)}}",
            new { controller = "FacebookAuthentication", action = "DataDeletionStatusCheck" });
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}