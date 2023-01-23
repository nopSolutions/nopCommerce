using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Zettle.Infrastructure
{
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
            endpointRouteBuilder.MapControllerRoute(name: ZettleDefaults.ConfigurationRouteName,
                pattern: "Admin/Zettle/Configure",
                defaults: new { controller = "ZettleAdmin", action = "Configure", area = AreaNames.Admin });

            endpointRouteBuilder.MapControllerRoute(name: ZettleDefaults.WebhookRouteName,
                pattern: "zettle/webhook",
                defaults: new { controller = "ZettleWebhook", action = "Webhook" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}