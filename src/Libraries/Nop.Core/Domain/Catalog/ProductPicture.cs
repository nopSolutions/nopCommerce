using LinqToDB.Mapping;
using Nop.Core.Data;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product picture mapping
    /// </summary>
    [Table(NopMappingDefaults.ProductPictureTable)]
    public partial class ProductPicture : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [Column]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        [Column]
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [Column]
        public int DisplayOrder { get; set; }
    }
}
