using LinqToDB.Mapping;
using Nop.Core.Data;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product category mapping
    /// </summary>
    [Table(NopMappingDefaults.ProductCategoryTable)]
    public partial class ProductCategory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [Column]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        [Column]
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is featured
        /// </summary>
        [Column]
        public bool IsFeaturedProduct { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [Column]
        public int DisplayOrder { get; set; }
    }
}
