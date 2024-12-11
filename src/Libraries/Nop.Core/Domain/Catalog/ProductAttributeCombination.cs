using System.ComponentModel;

namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Represents a product attribute combination
/// </summary>
public partial class ProductAttributeCombination : BaseEntity
{
    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the attributes
    /// </summary>
    public string AttributesXml { get; set; }

    /// <summary>
    /// Gets or sets the stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to allow orders when out of stock
    /// </summary>
    public bool AllowOutOfStockOrders { get; set; }

    /// <summary>
    /// Gets or sets the SKU
    /// </summary>
    public string Sku { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer part number
    /// </summary>
    public string ManufacturerPartNumber { get; set; }

    /// <summary>
    /// Gets or sets the Global Trade Item Number (GTIN). These identifiers include UPC (in North America), EAN (in Europe), JAN (in Japan), and ISBN (for books).
    /// </summary>
    public string Gtin { get; set; }

    /// <summary>
    /// Gets or sets the attribute combination price. This way a store owner can override the default product price when this attribute combination is added to the cart. For example, you can give a discount this way.
    /// </summary>
    public decimal? OverriddenPrice { get; set; }

    /// <summary>
    /// Gets or sets the quantity when admin should be notified
    /// </summary>
    public int NotifyAdminForQuantityBelow { get; set; }

    /// <summary>
    /// Gets or sets the minimum stock quantity
    /// </summary>
    public int MinStockQuantity { get; set; }

    /// <summary>
    /// The field is not used since 4.70 and is left only for the update process
    /// use the <see cref="ProductAttributeCombinationPicture"/> instead
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [Obsolete("The field is not used since 4.70 and is left only for the update process use the ProductAttributeCombinationPicture instead")]
    public int? PictureId { get; set; }
}