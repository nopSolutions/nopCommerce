using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Represents a shipping method (used by offline shipping rate computation methods)
/// </summary>
public partial class ShippingMethod : BaseEntity, ILocalizedEntity
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}