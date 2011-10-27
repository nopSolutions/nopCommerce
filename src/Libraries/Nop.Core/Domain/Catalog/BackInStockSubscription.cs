using System;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a back in stock subscription
    /// </summary>
    public partial class BackInStockSubscription : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product variant identifier
        /// </summary>
        public virtual int ProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public virtual int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets the product variant
        /// </summary>
        public virtual ProductVariant ProductVariant { get; set; }

        /// <summary>
        /// Gets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }

    }

}
