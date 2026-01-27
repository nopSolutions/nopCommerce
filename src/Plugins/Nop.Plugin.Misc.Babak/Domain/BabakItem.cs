using Nop.Core;

namespace Nop.Plugin.Misc.Babak.Domain;

/// <summary>
/// Represents a Babak item.
/// </summary>
public class BabakItem : BaseEntity
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets whether the item is active.
    /// </summary>
    public bool IsActive { get; set; }
}
