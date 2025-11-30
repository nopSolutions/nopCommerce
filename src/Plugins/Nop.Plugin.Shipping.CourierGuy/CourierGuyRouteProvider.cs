using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Shipping.CourierGuy;

public class CourierGuyRouteProvider : IRouteProvider
{
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute("Plugin.Shipping.CourierGuy.Webhooks.TrackingEvent",
            "Plugins/CourierGuy/Webhook/TrackingEvents",
            new { controller = "CourierGuyWebhook", action = "HandleTrackingEvent" });

        endpointRouteBuilder.MapControllerRoute("Plugin.Shipping.CourierGuy.Webhooks.ShipmentNote",
            "Plugins/CourierGuy/Webhook/ShipmentNotes",
            new { controller = "CourierGuyWebhook", action = "HandleShipmentNote" });

    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}