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
        /// Gets or sets the specification attribute name
        /// </summary>
        public string SpecificationAttributeName { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute display order
        /// </summary>
        public int SpecificationAttributeDisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option identifier
        /// </summary>
        public int SpecificationAttributeOptionId { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option name
        /// </summary>
        public string SpecificationAttributeOptionName { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option color (RGB)
        /// </summary>
        public string SpecificationAttributeOptionColorRgb { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option display order
        /// </summary>
        public int SpecificationAttributeOptionDisplayOrder { get; set; }
    }
}
