using LinqToDB.Mapping;
using Nop.Core.Data;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product-product tag mapping class
    /// </summary>
    [Table(NopMappingDefaults.ProductProductTagTable)]
    public partial class ProductProductTagMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        [NotColumn]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [Column("Product_Id")]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product tag identifier
        /// </summary>
        [Column("ProductTag_Id")]
        public int ProductTagId { get; set; }
    }
}