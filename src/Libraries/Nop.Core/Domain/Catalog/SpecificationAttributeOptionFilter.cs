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
        public int SpecificationAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the SpecificationAttributeName
        /// </summary>
        public string SpecificationAttributeName { get; set; }

        /// <summary>
        /// Gets or sets the DisplayOrder
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option identifier
        /// </summary>
        public int SpecificationAttributeOptionId { get; set; }

        /// <summary>
        /// Gets or sets the SpecificationAttributeOptionName
        /// </summary>
        public string SpecificationAttributeOptionName { get; set; }
    }
}
