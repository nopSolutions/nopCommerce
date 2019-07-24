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
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets the product
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }
    }
}
