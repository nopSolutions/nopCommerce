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
        public virtual int ProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the attributes
        /// </summary>
        public virtual string AttributesXml { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity
        /// </summary>
        public virtual int StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow orders when out of stock
        /// </summary>
        public virtual bool AllowOutOfStockOrders { get; set; }
        
        /// <summary>
        /// Gets the product variant
        /// </summary>
        public virtual ProductVariant ProductVariant { get; set; }

    }
}
