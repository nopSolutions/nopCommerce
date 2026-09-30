using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// A shipment belonging to an order
/// </summary>
public record ShipmentDto
{
    /// <summary>
    /// Gets or sets the shipment identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the order the shipment belongs to
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the carrier tracking number
    /// </summary>
    public string? TrackingNumber { get; set; }

    /// <summary>
    /// Gets or sets the total weight
    /// </summary>
    public decimal? TotalWeight { get; set; }

    /// <summary>
    /// Gets or sets the date and time the shipment was shipped (UTC), null while not shipped
    /// </summary>
    public DateTime? ShippedDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time the shipment was delivered (UTC)
    /// </summary>
    public DateTime? DeliveryDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time the shipment was marked ready for pickup (UTC)
    /// </summary>
    public DateTime? ReadyForPickupDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the admin comment
    /// </summary>
    public string? AdminComment { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation (UTC)
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the items contained in the shipment
    /// </summary>
    public List<ShipmentItemDto> Items { get; set; } = new();
}

/// <summary>
/// A single order item inside a shipment
/// </summary>
public record ShipmentItemDto
{
    /// <summary>
    /// Gets or sets the shipment item identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the order item the quantity is taken from
    /// </summary>
    public int OrderItemId { get; set; }

    /// <summary>
    /// Gets or sets the shipped quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the warehouse the item is shipped from
    /// </summary>
    public int WarehouseId { get; set; }
}
