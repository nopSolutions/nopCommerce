using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Represents a specification attribute
/// </summary>
public partial class SpecificationAttribute : BaseEntity, ILocalizedEntity
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
    /// Gets or sets the specification attribute group identifier
    /// </summary>
    public int? SpecificationAttributeGroupId { get; set; }
}