using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Shipping.ShipStation
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //Webhook
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.ShipStation.WebhookHandler", "Plugins/ShipStation/Webhook",
                new { controller = "ShipStation", action = "Webhook" });
        }

        public int Priority => 0;
    }
}
