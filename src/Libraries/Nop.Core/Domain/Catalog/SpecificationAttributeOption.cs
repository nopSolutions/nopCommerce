using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Represents a specification attribute option
/// </summary>
public partial class SpecificationAttributeOption : BaseEntity, ILocalizedEntity
{
    /// <summary>
    /// Gets or sets the specification attribute identifier
    /// </summary>
    public int SpecificationAttributeId { get; set; }

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