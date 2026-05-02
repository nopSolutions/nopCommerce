namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the shipment status
/// </summary>
public enum ShipmentStatusType
{
    /// <summary>
    /// The shipment was cancelled and the tracking number no longer applies.
    /// </summary>
    CANCELLED,

    /// <summary>
    /// The merchant has assigned a tracking number to the items being shipped from the Order. This does not correspond to the carrier's actual status for the shipment. The latest status of the parcel must be retrieved from the carrier.
    /// </summary>
    SHIPPED
}