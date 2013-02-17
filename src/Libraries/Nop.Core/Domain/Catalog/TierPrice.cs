using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a tier price
    /// </summary>
    public partial class TierPrice : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product variant identifier
        /// </summary>
        public int ProductVariantId { get; set; }
        /// <summary>
        /// Gets or sets the customer role identifier
        /// </summary>
        public int? CustomerRoleId { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the product variant
        /// </summary>
        public virtual ProductVariant ProductVariant { get; set; }

        /// <summary>
        /// Gets or sets the customer role
        /// </summary>
        public virtual CustomerRole CustomerRole { get; set; }
    }
}
