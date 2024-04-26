namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Represents a shipment
/// </summary>
public partial class Shipment : BaseEntity
{
    /// <summary>
    /// Gets or sets the order identifier
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the tracking number of this shipment
    /// </summary>
    public string TrackingNumber { get; set; }

    /// <summary>
    /// Gets or sets the total weight of this shipment
    /// It's nullable for compatibility with the previous version of nopCommerce where was no such property
    /// </summary>
    public decimal? TotalWeight { get; set; }

    /// <summary>
    /// Gets or sets the shipped date and time
    /// </summary>
    public DateTime? ShippedDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the delivery date and time
    /// </summary>
    public DateTime? DeliveryDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the ready for pickup date and time
    /// </summary>
    public DateTime? ReadyForPickupDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the admin comment
    /// </summary>
    public string AdminComment { get; set; }

    /// <summary>
    /// Gets or sets the entity creation date
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}