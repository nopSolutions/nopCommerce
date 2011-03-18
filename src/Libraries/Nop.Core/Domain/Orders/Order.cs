
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Payments;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order
    /// </summary>
    public partial class Order : BaseEntity
    {
        public Order()
        {
            this.DiscountUsageHistory = new List<DiscountUsageHistory>();
            this.GiftCardUsageHistory = new List<GiftCardUsageHistory>();
        }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public Guid OrderGuid { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }
        
        /// <summary>
        /// Gets or sets the payment status identifier
        /// </summary>
        public int PaymentStatusId { get; set; }
        
        /// <summary>
        /// Gets or sets the payment method system name
        /// </summary>
        public string PaymentMethodSystemName { get; set; }
        
        /// <summary>
        /// Gets or sets the order total
        /// </summary>
        public decimal OrderTotal { get; set; }

        //UNDONE add other properties

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of order creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }
        
        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual RewardPointsHistory RedeemedRewardPointsEntry { get; set; }

        /// <summary>
        /// Gets or sets discount usage history
        /// </summary>
        public virtual ICollection<DiscountUsageHistory> DiscountUsageHistory { get; set; }

        /// <summary>
        /// Gets or sets gift card usage history (gift card that were used with this order)
        /// </summary>
        public virtual ICollection<GiftCardUsageHistory> GiftCardUsageHistory { get; set; }
        
        /// <summary>
        /// Gets or sets the payment status
        /// </summary>
        public PaymentStatus PaymentStatus
        {
            get
            {
                return (PaymentStatus)this.PaymentStatusId;
            }
            set
            {
                this.PaymentStatusId = (int)value;
            }
        }
    }
}
