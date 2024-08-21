using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Represents a product availability range
/// </summary>
public partial class ProductAvailabilityRange : BaseEntity, ILocalizedEntity
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}