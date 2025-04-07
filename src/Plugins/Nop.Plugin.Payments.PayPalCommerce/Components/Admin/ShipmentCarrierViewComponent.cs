using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.PayPalCommerce.Models.Admin;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Services.Common;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.PayPalCommerce.Components.Admin;

/// <summary>
/// Represents the view component to render an additional input on the shipment details page in the admin area
/// </summary>
public class ShipmentCarrierViewComponent(IGenericAttributeService genericAttributeService,
        IShipmentService shipmentService,
        PayPalCommerceServiceManager serviceManager,
        PayPalCommerceSettings settings) : NopViewComponent
{
    #region Methods

    /// <summary>
    /// Invoke the widget view component
    /// </summary>
    /// <param name="widgetZone">Widget zone</param>
    /// <param name="additionalData">Additional parameters</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        var (active, _) = await serviceManager.IsActiveAsync(settings);
        if (!active)
            return Content(string.Empty);

        if (!settings.UseShipmentTracking)
            return Content(string.Empty);

        if (!widgetZone.Equals(AdminWidgetZones.OrderShipmentDetailsButtons) && !widgetZone.Equals(AdminWidgetZones.OrderShipmentAddButtons))
            return Content(string.Empty);

        if (additionalData is not ShipmentModel shipmentModel)
            return Content(string.Empty);

        var shipment = await shipmentService.GetShipmentByIdAsync(shipmentModel.Id);
        var model = new ShipmentCarrierModel
        {
            TrackingNumber = shipmentModel.TrackingNumber,
            PayPalCommerceShipmentCarrier = shipment is not null
                ? await genericAttributeService.GetAttributeAsync<string>(shipment, PayPalCommerceDefaults.ShipmentCarrierAttribute)
                : null
        };

        return View("~/Plugins/Payments.PayPalCommerce/Views/Admin/_ShipmentCarrier.cshtml", model);
    }

    #endregion
}