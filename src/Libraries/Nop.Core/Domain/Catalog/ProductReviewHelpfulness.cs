<<<<<<< HEAD
﻿namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product review helpfulness
    /// </summary>
    public partial class ProductReviewHelpfulness : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product review identifier
        /// </summary>
        public int ProductReviewId { get; set; }

        /// <summary>
        /// A value indicating whether a review a helpful
        /// </summary>
        public bool WasHelpful { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }
    }
}
=======
﻿namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product review helpfulness
    /// </summary>
    public partial class ProductReviewHelpfulness : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product review identifier
        /// </summary>
        public int ProductReviewId { get; set; }

        /// <summary>
        /// A value indicating whether a review a helpful
        /// </summary>
        public bool WasHelpful { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }
    }
}
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
