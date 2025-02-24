using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.RFQ.Infrastructure;

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
        endpointRouteBuilder.MapControllerRoute(name: RfqDefaults.ConfigurationRouteName,
            pattern: "Admin/RFQ/Configure",
            defaults: new { controller = "RfqAdmin", action = "Configure", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: RfqDefaults.CustomerRequestRouteName,
            pattern: "Customer/RFQ/RequestForQuote/{requestId?}",
            defaults: new { controller = "RfqCustomer", action = "CustomerRequest" });

        endpointRouteBuilder.MapControllerRoute(name: RfqDefaults.CustomerRequestsRouteName,
            pattern: "Customer/RFQ/RequestsForQuote/",
            defaults: new { controller = "RfqCustomer", action = "CustomerRequests" });

        endpointRouteBuilder.MapControllerRoute(name: RfqDefaults.CustomerQuotesRouteName,
            pattern: "Customer/RFQ/Quotes/",
            defaults: new { controller = "RfqCustomer", action = "CustomerQuotes" });

        endpointRouteBuilder.MapControllerRoute(name: RfqDefaults.CustomerQuoteRouteName,
            pattern: "Customer/RFQ/Quote/{quoteId}",
            defaults: new { controller = "RfqCustomer", action = "CustomerQuote" });
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}