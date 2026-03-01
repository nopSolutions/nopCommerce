using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.Paystack.Infrastructure;

/// <summary>
/// Registers Paystack callback and webhook routes (public, no admin area).
/// </summary>
public class RouteProvider : IRouteProvider
{
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute(name: PaystackDefaults.CallbackRouteName,
            pattern: "paystack/callback",
            defaults: new { controller = "PaystackCallback", action = "Callback" });

        endpointRouteBuilder.MapControllerRoute(name: PaystackDefaults.WebhookRouteName,
            pattern: "paystack/webhook",
            defaults: new { controller = "PaystackCallback", action = "Webhook" });

        endpointRouteBuilder.MapControllerRoute(name: PaystackDefaults.CompleteRouteName,
            pattern: "paystack/complete",
            defaults: new { controller = "PaystackCallback", action = "Complete" });
    }

    public int Priority => 0;
}
