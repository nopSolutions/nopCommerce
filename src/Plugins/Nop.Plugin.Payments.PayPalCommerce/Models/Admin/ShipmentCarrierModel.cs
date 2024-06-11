using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.PayPalCommerce.Models.Admin;

/// <summary>
/// Represents the shipment carrier model
/// </summary>
public record ShipmentCarrierModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Shipment.Carrier")]
    public string PayPalCommerceShipmentCarrier { get; set; }

    public string TrackingNumber { get; set; }

    #endregion
}