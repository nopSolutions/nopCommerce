using LinqToDB.Mapping;
using Nop.Core.Data;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product manufacturer mapping
    /// </summary>
    [Table(NopMappingDefaults.ProductManufacturerTable)]
    public partial class ProductManufacturer : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [Column]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer identifier
        /// </summary>
        [Column]
        public int ManufacturerId { get; set; }

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
