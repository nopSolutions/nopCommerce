using System;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a gift card usage history entry
    /// </summary>
    public partial class GiftCardUsageHistory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the gift card identifier
        /// </summary>
        public virtual int GiftCardId { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public virtual int UsedWithOrderId { get; set; }

        /// <summary>
        /// Gets or sets the used value (amount)
        /// </summary>
        public virtual decimal UsedValue { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }
        
        /// <summary>
        /// Gets the gift card
        /// </summary>
        public virtual GiftCard GiftCard { get; set; }

        /// <summary>
        /// Gets the gift card
        /// </summary>
        public virtual Order UsedWithOrder { get; set; }
    }
}
