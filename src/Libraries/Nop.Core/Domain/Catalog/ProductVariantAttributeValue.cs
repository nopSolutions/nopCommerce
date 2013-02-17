
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product variant attribute value
    /// </summary>
    public partial class ProductVariantAttributeValue : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the product variant attribute mapping identifier
        /// </summary>
        public int ProductVariantAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the product variant attribute name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the color RGB value (used with "Color squares" attribute type)
        /// </summary>
        public string ColorSquaresRgb { get; set; }

        /// <summary>
        /// Gets or sets the price adjustment
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        /// Gets or sets the weight adjustment
        /// </summary>
        public decimal WeightAdjustment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
        
        /// <summary>
        /// Gets the product variant attribute
        /// </summary>
        public virtual ProductVariantAttribute ProductVariantAttribute { get; set; }
    }

}
