<<<<<<< HEAD
﻿using System;

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
        public int GiftCardId { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public int UsedWithOrderId { get; set; }

        /// <summary>
        /// Gets or sets the used value (amount)
        /// </summary>
        public decimal UsedValue { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}
=======
﻿using System;

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
        public int GiftCardId { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public int UsedWithOrderId { get; set; }

        /// <summary>
        /// Gets or sets the used value (amount)
        /// </summary>
        public decimal UsedValue { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
