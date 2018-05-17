using Nop.Core.Domain.Catalog;

namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount-product mapping class
    /// </summary>
    public partial class DiscountProductMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the discount identifier
        /// </summary>
        public int DiscountId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the discount
        /// </summary>
        public virtual Discount Discount { get; set; }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }
    }
}