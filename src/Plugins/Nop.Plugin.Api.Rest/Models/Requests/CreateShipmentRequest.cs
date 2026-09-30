using System.Collections.Generic;

namespace Nop.Plugin.Api.Rest.Models.Requests;

/// <summary>
/// Request to create a shipment for an order
/// </summary>
public record CreateShipmentRequest
{
    /// <summary>
    /// Gets or sets the order items to ship, with an explicit quantity each
    /// </summary>
    /// <remarks>
    /// Left empty, every order item that can still be shipped is added at its full remaining quantity
    /// </remarks>
    public List<ShipmentItemRequest>? Items { get; set; }

    /// <summary>
    /// Gets or sets the carrier tracking number
    /// </summary>
    public string? TrackingNumber { get; set; }

    /// <summary>
    /// Gets or sets the admin comment
    /// </summary>
    public string? AdminComment { get; set; }
}

/// <summary>
/// An order item and the quantity to ship for it
/// </summary>
public record ShipmentItemRequest
{
    /// <summary>
    /// Gets or sets the order item identifier
    /// </summary>
    public int OrderItemId { get; set; }

    /// <summary>
    /// Gets or sets the quantity to ship, which cannot exceed what is still pending
    /// </summary>
    public int Quantity { get; set; }
}
