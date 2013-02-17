
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product review helpfulness
    /// </summary>
    public partial class ProductReviewHelpfulness : CustomerContent
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
        /// Gets the product
        /// </summary>
        public virtual ProductReview ProductReview { get; set; }
    }
}
