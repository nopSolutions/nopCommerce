namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a specification attribute option filter
    /// </summary>
    public class SpecificationAttributeOptionFilter
    {
        /// <summary>
        /// Gets or sets the specification attribute identifier
        /// </summary>
        public virtual int SpecificationAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the SpecificationAttributeName
        /// </summary>
        public virtual string SpecificationAttributeName { get; set; }

        /// <summary>
        /// Gets or sets the DisplayOrder
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option identifier
        /// </summary>
        public virtual int SpecificationAttributeOptionId { get; set; }

        /// <summary>
        /// Gets or sets the SpecificationAttributeOptionName
        /// </summary>
        public virtual string SpecificationAttributeOptionName { get; set; }
    }
}
