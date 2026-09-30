namespace Nop.Plugin.Api.Rest.Models.Requests;

/// <summary>
/// Request to mark a shipment as shipped
/// </summary>
public record ShipShipmentRequest
{
    /// <summary>
    /// Gets or sets a value indicating whether the customer is notified by email
    /// </summary>
    public bool NotifyCustomer { get; set; }
}
