using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Services.Plugins;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.Paystack.Infrastructure;

/// <summary>
/// Registers Paystack callback and webhook routes (public, no admin area).
/// </summary>
public class RouteProvider : IRouteProvider
{
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute(name: PaystackDefaults.CALLBACK_ROUTE_NAME,
            pattern: "paystack/callback",
            defaults: new { controller = "PaystackCallback", action = "Callback" });

        endpointRouteBuilder.MapControllerRoute(name: PaystackDefaults.WEBHOOK_ROUTE_NAME,
            pattern: "paystack/webhook",
            defaults: new { controller = "PaystackCallback", action = "Webhook" });

        endpointRouteBuilder.MapControllerRoute(name: PaystackDefaults.COMPLETE_ROUTE_NAME,
            pattern: "paystack/complete",
            defaults: new { controller = "PaystackCallback", action = "Complete" });

        endpointRouteBuilder.MapControllerRoute(name: "PaystackCallback.Complete",
            pattern: "paystack/callback/complete",
            defaults: new { controller = "PaystackCallback", action = "Complete" });

        endpointRouteBuilder.MapControllerRoute(name: "Plugin.Payments.Paystack.ShowPopup",
            pattern: "paystack/show-popup",
            defaults: new { controller = "Paystack", action = "ShowPopup" });

        endpointRouteBuilder.MapControllerRoute(name: PaystackDefaults.CANCEL_PAYMENT,
            pattern: "paystack/cancel-payment",
            defaults: new { controller = "Paystack", action = "CancelPayment" });
    }

    public int Priority => 0;
}
