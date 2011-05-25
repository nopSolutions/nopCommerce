
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a checkout attribute value
    /// </summary>
    public partial class CheckoutAttributeValue : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the checkout attribute mapping identifier
        /// </summary>
        public virtual int CheckoutAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the checkout attribute name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the price adjustment
        /// </summary>
        public virtual decimal PriceAdjustment { get; set; }

        /// <summary>
        /// Gets or sets the weight adjustment
        /// </summary>
        public virtual decimal WeightAdjustment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public virtual bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the checkout attribute
        /// </summary>
        public virtual CheckoutAttribute CheckoutAttribute { get; set; }
    }

}
