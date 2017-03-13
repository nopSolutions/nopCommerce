using System;
using Nop.Core.Domain.Orders;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a reward point history entry
    /// </summary>
    public partial class RewardPointsHistory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the store identifier in which these reward points were awarded or redeemed
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the points redeemed/added
        /// </summary>
        public int Points { get; set; }
        /// <summary>
        /// Gets or sets the purchased points redeemed/added
        /// </summary>
        public int PointsPurchased { get; set; }
        /// <summary>
        /// Gets or sets the points balance
        /// </summary>
        public int? PointsBalance { get; set; }
        /// <summary>
        /// Gets or sets the purchased points balance
        /// </summary>
        public int? PointsBalancePurchased { get; set; }
        /// <summary>
        /// Gets or sets the used amount. Opposite sign as respective points!
        /// </summary>
        public decimal UsedAmount { get; set; }
        /// <summary>
        /// Gets or sets the used purchased amount. Opposite sign as respective points!
        /// </summary>
        public decimal UsedAmountPurchased { get; set; }
        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the order for which points were redeemed as a payment (spent by a customer when placing this order)
        /// </summary>
        public virtual Order UsedWithOrder { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }
        /// <summary>
        /// Gets or sets the associated order item identifier
        /// </summary>
        public int? PurchasedWithOrderItemId { get; set; }
        /// <summary>
        /// Gets or sets the associated order item
        /// </summary>
        public virtual OrderItem PurchasedWithOrderItem { get; set; }
    }
}
