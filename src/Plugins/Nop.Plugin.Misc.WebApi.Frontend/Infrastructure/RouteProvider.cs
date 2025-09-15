using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.WebApi.Frontend.Infrastructure;

/// <summary>
/// Represents the plugin route provider
/// </summary>
public class RouteProvider : IRouteProvider
{
    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        // API routes
        endpointRouteBuilder.MapControllerRoute(
            name: WebApiFrontendDefaults.Route.Orders,
            pattern: "api/orders/{id?}",
            defaults: new { controller = "Orders", action = "GetOrders" });

        endpointRouteBuilder.MapControllerRoute(
            name: WebApiFrontendDefaults.Route.Customers,
            pattern: "api/customers/{id?}",
            defaults: new { controller = "Customers", action = "GetCustomers" });

        endpointRouteBuilder.MapControllerRoute(
            name: WebApiFrontendDefaults.Route.Products,
            pattern: "api/products/{id?}",
            defaults: new { controller = "Products", action = "GetProducts" });

        endpointRouteBuilder.MapControllerRoute(
            name: WebApiFrontendDefaults.Route.Authentication,
            pattern: "api/auth/{{action}}",
            defaults: new { controller = "Authentication" });
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}