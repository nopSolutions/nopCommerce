
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
        /// Gets or sets the specification attribute name
        /// </summary>
        public virtual string SpecificationAttributeName { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute display order
        /// </summary>
        public virtual int SpecificationAttributeDisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option identifier
        /// </summary>
        public virtual int SpecificationAttributeOptionId { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option name
        /// </summary>
        public virtual string SpecificationAttributeOptionName { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option display order
        /// </summary>
        public virtual int SpecificationAttributeOptionDisplayOrder { get; set; }
    }
}
