namespace Nop.Services.Installation.SampleData;

/// <summary>
/// Represents a sample specification attributes and groups
/// </summary>
public partial class SampleSpecificationAttributes
{
    /// <summary>
    /// Sample specification attributes
    /// </summary>
    public List<SampleSpecificationAttribute> Attributes { get; set; }

    /// <summary>
    /// Sample specification attribute groups
    /// </summary>
    public List<SampleSpecificationAttributeGroup> AttributeGroups { get; set; }

    #region Nested classes

    /// <summary>
    /// Represents a sample specification attribute
    /// </summary>
    public partial class SampleSpecificationAttribute
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// sample specification attribute options
        /// </summary>
        public List<SampleSpecificationAttributeOption> AttributeOptions { get; set; } = new();
    }

    /// <summary>
    /// Represents a sample specification attribute option
    /// </summary>
    public partial class SampleSpecificationAttributeOption
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the color RGB value (used when you want to display "Color squares" instead of text)
        /// </summary>
        public string ColorSquaresRgb { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// Represents a sample specification attribute group
    /// </summary>
    public partial class SampleSpecificationAttributeGroup
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sample specification attributes
        /// </summary>
        public List<SampleSpecificationAttribute> Attributes { get; set; } = new();
    }

    #endregion
}
