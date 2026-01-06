using ClosedXML.Excel;
using Nop.Core;

namespace Nop.Plugin.DropShipping.AliExpress.Domain;

/// <summary>
/// Represents an order placed on AliExpress for drop shipping
/// </summary>
public class AliExpressOrder : BaseEntity
{
    /// <summary>
    /// Gets or sets the NopCommerce order identifier
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the AliExpress order identifier
    /// </summary>
    public long? AliExpressOrderId { get; set; }

    /// <summary>
    /// Gets or sets the AliExpress product identifier
    /// </summary>
    public long AliExpressProductId { get; set; }

    /// <summary>
    /// Gets or sets the order status on AliExpress
    /// </summary>
    public string? AliExpressOrderStatus { get; set; }

    /// <summary>
    /// Gets or sets the tracking number from AliExpress
    /// </summary>
    public string? AliExpressTrackingNumber { get; set; }

    /// <summary>
    /// Gets or sets the logistics service used
    /// </summary>
    public string? LogisticsServiceName { get; set; }

    /// <summary>
    /// Gets or sets when the order was placed on AliExpress
    /// </summary>
    public DateTime? PlacedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets when the order was marked as shipped on AliExpress
    /// </summary>
    public DateTime? ShippedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets when the order was delivered (by AliExpress)
    /// </summary>
    public DateTime? DeliveredOnUtc { get; set; }

    /// <summary>
    /// Gets or sets whether local shipping was created with Courier Guy
    /// </summary>
    public bool LocalShippingCreated { get; set; }

    /// <summary>
    /// Gets or sets the Courier Guy tracking number (if applicable)
    /// </summary>
    public string? CourierGuyTrackingNumber { get; set; }

    /// <summary>
    /// Gets or sets when this record was created
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets when this record was last updated
    /// </summary>
    public DateTime UpdatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the last error message (if any)
    /// </summary>
    public string? LastErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets JSON data with full order details
    /// </summary>
    public string? OrderDetailsJson { get; set; }
}
