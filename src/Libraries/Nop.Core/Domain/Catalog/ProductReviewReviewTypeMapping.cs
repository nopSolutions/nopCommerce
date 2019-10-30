using LinqToDB.Mapping;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product review and review type mapping
    /// </summary>
    [Table(NopMappingDefaults.ProductReviewReviewTypeTable)]
    public partial class ProductReviewReviewTypeMapping : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the product review identifier
        /// </summary>
        [Column]
        public int ProductReviewId { get; set; }

        /// <summary>
        /// Gets or sets the review type identifier
        /// </summary>
        [Column]
        public int ReviewTypeId { get; set; }

        /// <summary>
        /// Gets or sets the rating
        /// </summary>
        [Column]
        public int Rating { get; set; }
    }
}
