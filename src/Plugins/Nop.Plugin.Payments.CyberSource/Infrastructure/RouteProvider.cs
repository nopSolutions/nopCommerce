using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Payments.CyberSource.Infrastructure
{
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
            var lang = GetLanguageRoutePattern();

            endpointRouteBuilder.MapControllerRoute(name: CyberSourceDefaults.ConfigurationRouteName,
                pattern: "Admin/CyberSource/Configure",
                defaults: new { controller = "CyberSource", action = "Configure" });

            endpointRouteBuilder.MapControllerRoute(name: CyberSourceDefaults.CustomerTokensRouteName,
                pattern: $"{lang}/customer/cybersource-tokens",
                defaults: new { controller = "CyberSourceCustomerToken", action = "CustomerTokens" });

            endpointRouteBuilder.MapControllerRoute(name: CyberSourceDefaults.CustomerTokenAddRouteName,
                pattern: $"{lang}/customer/cybersource-token-add",
                defaults: new { controller = "CyberSourceCustomerToken", action = "TokenAdd" });

            endpointRouteBuilder.MapControllerRoute(name: CyberSourceDefaults.CustomerTokenEditRouteName,
                pattern: $"{lang}/customer/cybersource-token-edit/{{tokenId:min(0)}}",
                defaults: new { controller = "CyberSourceCustomerToken", action = "TokenEdit" });

            endpointRouteBuilder.MapControllerRoute(name: CyberSourceDefaults.PayerRedirectRouteName,
                pattern: "cybersource-payer-redirect",
                defaults: new { controller = "CyberSourceWebhook", action = "PayerRedirect" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}