namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product variant attribute combination
    /// </summary>
    public partial class ProductVariantAttributeCombination : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product variant identifier
        /// </summary>
        public int ProductVariantId { get; set; }

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
        /// Gets the product variant
        /// </summary>
        public virtual ProductVariant ProductVariant { get; set; }

    }
}
