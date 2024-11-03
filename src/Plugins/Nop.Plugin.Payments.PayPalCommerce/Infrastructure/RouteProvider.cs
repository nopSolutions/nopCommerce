using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Payments.PayPalCommerce.Infrastructure;

/// <summary>
/// Represents the plugin route provider
/// </summary>
public class RouteProvider : BaseRouteProvider, IRouteProvider
{
    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var lang = GetLanguageRoutePattern();

        endpointRouteBuilder.MapControllerRoute(name: PayPalCommerceDefaults.Route.Configuration,
            pattern: "Admin/PayPalCommerce/Configure",
            defaults: new { controller = "PayPalCommerce", action = "Configure", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: PayPalCommerceDefaults.Route.OnboardingCallback,
            pattern: "Admin/PayPalCommerce/Onboarding/{storeId:int}",
            defaults: new { controller = "PayPalCommerce", action = "OnboardingCallback", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: PayPalCommerceDefaults.Route.Webhook,
            pattern: "Plugins/PayPalCommerce/Webhook",
            defaults: new { controller = "PayPalCommerceWebhook", action = "WebhookHandler" });

        endpointRouteBuilder.MapControllerRoute(name: PayPalCommerceDefaults.Route.PaymentInfo,
            pattern: $"{lang}/paypal/payment-info",
            defaults: new { controller = "PayPalCommercePublic", action = "PluginPaymentInfo" });

        endpointRouteBuilder.MapControllerRoute(name: PayPalCommerceDefaults.Route.ConfirmOrder,
            pattern: $"{lang}/paypal/confirm-order",
            defaults: new { controller = "PayPalCommercePublic", action = "ConfirmOrder" });

        endpointRouteBuilder.MapControllerRoute(name: PayPalCommerceDefaults.Route.PaymentTokens,
            pattern: $"{lang}/customer/paypal-payment-methods",
            defaults: new { controller = "PayPalCommercePublic", action = "PaymentTokens" });

    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}