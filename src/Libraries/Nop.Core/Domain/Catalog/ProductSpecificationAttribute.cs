namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product specification attribute
    /// </summary>
    public partial class ProductSpecificationAttribute : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public virtual int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute identifier
        /// </summary>
        public virtual int SpecificationAttributeOptionId { get; set; }

        /// <summary>
        /// Gets or sets whether the attribute can be filtered by
        /// </summary>
        public virtual bool AllowFiltering { get; set; }

        /// <summary>
        /// Gets or sets whether the attrbiute will be shown on the product page
        /// </summary>
        public virtual bool ShowOnProductPage { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }
        
        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option
        /// </summary>
        public virtual SpecificationAttributeOption SpecificationAttributeOption { get; set; }
    }
}
