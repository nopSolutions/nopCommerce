using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Misc.RFQ.Infrastructure;

/// <summary>
/// Represents plugin route provider
/// </summary>
public class RouteProvider : BaseRouteProvider, IRouteProvider
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
        var lang = GetLanguageRoutePattern();

        endpointRouteBuilder.MapControllerRoute(name: RfqDefaults.CreateCustomerRequestRouteName,
            pattern: $"{lang}/rfq/requestforquote/{{requestId?}}",
            defaults: new { controller = "RfqCustomer", action = "CustomerRequest" });

        endpointRouteBuilder.MapControllerRoute(name: RfqDefaults.ClearCustomerRequestRouteName,
            pattern: $"{lang}/rfq/exitquotemode",
            defaults: new { controller = "RfqCustomer", action = "ExitQuoteMode" });

        endpointRouteBuilder.MapControllerRoute(name: RfqDefaults.CustomerRequestsRouteName,
            pattern: $"{lang}/rfq/requestsforquote/",
            defaults: new { controller = "RfqCustomer", action = "CustomerRequests" });

        endpointRouteBuilder.MapControllerRoute(name: RfqDefaults.CustomerQuotesRouteName,
            pattern: $"{lang}/rfq/quotes/",
            defaults: new { controller = "RfqCustomer", action = "CustomerQuotes" });

        endpointRouteBuilder.MapControllerRoute(name: RfqDefaults.CustomerQuoteRouteName,
            pattern: $"{lang}/rfq/quote/{{quoteId}}",
            defaults: new { controller = "RfqCustomer", action = "CustomerQuote" });
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}