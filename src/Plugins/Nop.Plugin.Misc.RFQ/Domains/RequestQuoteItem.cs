using Nop.Core;

namespace Nop.Plugin.Misc.RFQ.Domains;

/// <summary>
/// Represents a request a quote item entity
/// </summary>
public class RequestQuoteItem : BaseEntity, IAdminNote
{
    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product attributes in XML format
    /// </summary>
    public string ProductAttributesXml { get; set; }

    /// <summary>
    /// Gets or sets the original cost of this item, qty 1
    /// </summary>
    public decimal OriginalProductPrice { get; set; }

    /// <summary>
    /// Gets or sets the unit price in primary store currency (include tax)
    /// </summary>
    public decimal RequestedUnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the quantity
    /// </summary>
    public int RequestedQty { get; set; }

    /// <summary>
    /// Gets or sets the administration notes
    /// </summary>
    public string AdminNotes { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the request quote identifier
    /// </summary>
    public int RequestQuoteId { get; set; }
}