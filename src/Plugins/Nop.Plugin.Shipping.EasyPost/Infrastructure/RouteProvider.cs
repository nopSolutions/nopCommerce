using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Shipping.EasyPost.Infrastructure
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
            endpointRouteBuilder.MapControllerRoute(name: EasyPostDefaults.ConfigurationRouteName,
                pattern: "Admin/EasyPost/Configure",
                defaults: new { controller = "EasyPost", action = "Configure" });

            endpointRouteBuilder.MapControllerRoute(name: EasyPostDefaults.UpdateShipmentRouteName,
                pattern: "Admin/EasyPost/UpdateShipment",
                defaults: new { controller = "EasyPost", action = "UpdateShipment" });

            endpointRouteBuilder.MapControllerRoute(name: EasyPostDefaults.BuyLabelRouteName,
                pattern: "Admin/EasyPost/BuyLabel",
                defaults: new { controller = "EasyPost", action = "BuyLabel" });

            endpointRouteBuilder.MapControllerRoute(name: EasyPostDefaults.DownloadLabelRouteName,
                pattern: "Admin/EasyPost/DownloadLabel",
                defaults: new { controller = "EasyPost", action = "DownloadLabel" });

            endpointRouteBuilder.MapControllerRoute(name: EasyPostDefaults.DownloadInvoiceRouteName,
                pattern: "Admin/EasyPost/DownloadInvoice",
                defaults: new { controller = "EasyPost", action = "DownloadInvoice" });

            endpointRouteBuilder.MapControllerRoute(name: EasyPostDefaults.CreatePickupRouteName,
                pattern: "Admin/EasyPost/CreatePickup",
                defaults: new { controller = "EasyPost", action = "CreatePickup" });

            endpointRouteBuilder.MapControllerRoute(name: EasyPostDefaults.BuyPickupRouteName,
                pattern: "Admin/EasyPost/BuyPickup",
                defaults: new { controller = "EasyPost", action = "BuyPickup" });

            endpointRouteBuilder.MapControllerRoute(name: EasyPostDefaults.CancelPickupRouteName,
                pattern: "Admin/EasyPost/CancelPickup",
                defaults: new { controller = "EasyPost", action = "CancelPickup" });

            endpointRouteBuilder.MapControllerRoute(name: EasyPostDefaults.WebhookRouteName,
                pattern: "easypost/webhook",
                defaults: new { controller = "EasyPostPublic", action = "Webhook" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}