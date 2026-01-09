using Nop.Core;

namespace Nop.Plugin.DropShipping.AliExpress.Domain;

/// <summary>
/// Represents an AliExpress product mapping to a NopCommerce product
/// </summary>
public class AliExpressProductMapping : BaseEntity
{
    /// <summary>
    /// Gets or sets the NopCommerce product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the AliExpress product identifier
    /// </summary>
    public long AliExpressProductId { get; set; }

    /// <summary>
    /// Gets or sets the AliExpress product URL
    /// </summary>
    public string? AliExpressProductUrl { get; set; }

    /// <summary>
    /// Gets or sets the selected SKU attributes (e.g., "14:350853#Black;5:361386")
    /// </summary>
    public string? SkuAttributes { get; set; }

    /// <summary>
    /// Gets or sets the AliExpress product price (base price from AliExpress)
    /// </summary>
    public decimal AliExpressPrice { get; set; }

    /// <summary>
    /// Gets or sets the shipping cost
    /// </summary>
    public decimal ShippingCost { get; set; }

    /// <summary>
    /// Gets or sets the VAT amount
    /// </summary>
    public decimal VatAmount { get; set; }

    /// <summary>
    /// Gets or sets the customs duty amount (10% as per requirements)
    /// </summary>
    public decimal CustomsDuty { get; set; }

    /// <summary>
    /// Gets or sets the margin percentage applied
    /// </summary>
    public decimal MarginPercentage { get; set; }

    /// <summary>
    /// Gets or sets the calculated selling price
    /// </summary>
    public decimal CalculatedPrice { get; set; }

    /// <summary>
    /// Gets or sets the shipping service name
    /// </summary>
    public string? ShippingServiceName { get; set; }

    /// <summary>
    /// Gets or sets the estimated delivery time in days
    /// </summary>
    public int? EstimatedDeliveryDays { get; set; }

    /// <summary>
    /// Gets or sets when this mapping was created
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets when this mapping was last synchronized
    /// </summary>
    public DateTime LastSyncOnUtc { get; set; }

    /// <summary>
    /// Gets or sets whether this product is available on AliExpress
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Gets or sets the last sync status message
    /// </summary>
    public string? LastSyncMessage { get; set; }

    /// <summary>
    /// Gets or sets JSON data with additional product details from AliExpress
    /// </summary>
    public string? ProductDetailsJson { get; set; }
}
