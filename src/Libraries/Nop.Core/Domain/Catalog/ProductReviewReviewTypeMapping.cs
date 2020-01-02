using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product review and review type mapping
    /// </summary>
    public partial class ProductReviewReviewTypeMapping : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the product review identifier
        /// </summary>
        public int ProductReviewId { get; set; }

        /// <summary>
        /// Gets or sets the review type identifier
        /// </summary>
        public int ReviewTypeId { get; set; }

        /// <summary>
        /// Gets or sets the rating
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Gets the product review
        /// </summary>
        public virtual ProductReview ProductReview { get; set; }

        /// <summary>
        /// Gets the review type
        /// </summary>
        public virtual ReviewType ReviewType { get; set; }
    }
}
