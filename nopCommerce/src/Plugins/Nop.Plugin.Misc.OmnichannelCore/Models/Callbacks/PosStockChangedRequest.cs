namespace Nop.Plugin.Misc.OmnichannelCore.Models.Callbacks;

/// <summary>
/// Represents a POS-originated stock changed event
/// </summary>
public record PosStockChangedRequest
{
    /// <summary>
    /// Gets or sets the message identifier
    /// </summary>
    public Guid MessageId { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier
    /// </summary>
    public string CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the event type
    /// </summary>
    public string EventType { get; set; }

    /// <summary>
    /// Gets or sets the event occurrence date and time
    /// </summary>
    public DateTime OccurredOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the source system
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the source version
    /// </summary>
    public long SourceVersion { get; set; }

    /// <summary>
    /// Gets or sets the nopCommerce product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product SKU
    /// </summary>
    public string Sku { get; set; }

    /// <summary>
    /// Gets or sets the warehouse identifier
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// Gets or sets the quantity on hand
    /// </summary>
    public int QuantityOnHand { get; set; }
}
