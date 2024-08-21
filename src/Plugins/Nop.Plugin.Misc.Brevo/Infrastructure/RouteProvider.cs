using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Brevo.Infrastructure;

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
        endpointRouteBuilder.MapControllerRoute(BrevoDefaults.ImportContactsRoute, "Plugins/Brevo/ImportContacts",
            new { controller = "Brevo", action = "ImportContacts" });

        endpointRouteBuilder.MapControllerRoute(BrevoDefaults.UnsubscribeContactRoute, "Plugins/Sendinblue/UnsubscribeWebHook",
            new { controller = "BrevoWebhook", action = "UnsubscribeWebHook" });

        endpointRouteBuilder.MapControllerRoute(BrevoDefaults.UnsubscribeContactRoute, "Plugins/Brevo/UnsubscribeWebHook",
            new { controller = "BrevoWebhook", action = "UnsubscribeWebHook" });
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}