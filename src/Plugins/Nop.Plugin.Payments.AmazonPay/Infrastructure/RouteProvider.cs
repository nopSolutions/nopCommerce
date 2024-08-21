using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Payments.AmazonPay.Infrastructure;

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
        //get language pattern
        //it's not needed to use language pattern in AJAX requests and for actions returning the result directly (e.g. file to download),
        //use it only for URLs of pages that the user can go to
        var lang = GetLanguageRoutePattern();

        //confirm order
        endpointRouteBuilder.MapControllerRoute(name: AmazonPayDefaults.ConfirmRouteName,
            pattern: $"{lang}/amazon-pay/confirm",
            defaults: new { controller = "AmazonPayCheckout", action = "Confirm" });

        //completed page
        endpointRouteBuilder.MapControllerRoute(name: AmazonPayDefaults.CheckoutResultHandlerRouteName,
            pattern: $"{lang}/amazon-pay/completed",
            defaults: new { controller = "AmazonPayCheckout", action = "Completed" });

        //set sign in method
        endpointRouteBuilder.MapControllerRoute(name: AmazonPayDefaults.SignInHandlerRouteName,
            pattern: $"{lang}/amazon-pay/sign-in",
            defaults: new { controller = "AmazonPayCustomer", action = "SignIn" });

        //IPN
        endpointRouteBuilder.MapControllerRoute(name: AmazonPayDefaults.IPNHandlerRouteName,
            pattern: "amazon-pay/ipn",
            defaults: new { controller = "AmazonPayIpn", action = "IPNHandler" });

        //onboarding
        endpointRouteBuilder.MapControllerRoute(name: AmazonPayDefaults.Onboarding.KeyShareRouteName,
            pattern: "amazon-pay/key-share",
            defaults: new { controller = "AmazonPayOnboarding", action = "KeyShare" });
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}