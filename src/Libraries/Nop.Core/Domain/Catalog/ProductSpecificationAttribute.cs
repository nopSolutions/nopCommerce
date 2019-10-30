using LinqToDB.Mapping;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product specification attribute
    /// </summary>
    [Table(NopMappingDefaults.ProductSpecificationAttributeTable)]
    public partial class ProductSpecificationAttribute : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [Column]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the attribute type ID
        /// </summary>
        [Column]
        public int AttributeTypeId { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute identifier
        /// </summary>
        [Column]
        public int SpecificationAttributeOptionId { get; set; }

        /// <summary>
        /// Gets or sets the custom value
        /// </summary>
        [Column]
        public string CustomValue { get; set; }

        /// <summary>
        /// Gets or sets whether the attribute can be filtered by
        /// </summary>
        [Column]
        public bool AllowFiltering { get; set; }

        /// <summary>
        /// Gets or sets whether the attribute will be shown on the product page
        /// </summary>
        [Column]
        public bool ShowOnProductPage { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [Column]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        [NotColumn]
        public SpecificationAttributeType AttributeType
        {
            get => (SpecificationAttributeType)AttributeTypeId;
            set => AttributeTypeId = (int)value;
        }
    }
}
