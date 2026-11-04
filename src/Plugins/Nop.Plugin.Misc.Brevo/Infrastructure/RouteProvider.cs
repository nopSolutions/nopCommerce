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
        endpointRouteBuilder.MapControllerRoute(name: BrevoDefaults.ImportContactsRoute,
            pattern: "Plugins/Brevo/ImportContacts",
            defaults: new { controller = "Brevo", action = "ImportContacts" });

        endpointRouteBuilder.MapControllerRoute(name: BrevoDefaults.UnsubscribeContactRoute,
            pattern: $"Plugins/Brevo/UnsubscribeWebHook",
            defaults: new { controller = "BrevoWebhook", action = "UnsubscribeWebHook" });

        //leave it for compatibility
        endpointRouteBuilder.MapControllerRoute(name: "Plugin.Misc.Sendinblue.Unsubscribe",
            pattern: "Plugins/Sendinblue/UnsubscribeWebHook",
            defaults: new { controller = "BrevoWebhook", action = "UnsubscribeWebHook" });
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}