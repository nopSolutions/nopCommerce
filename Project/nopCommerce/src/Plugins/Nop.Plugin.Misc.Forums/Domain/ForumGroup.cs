using Nop.Core;

namespace Nop.Plugin.Misc.Forums.Domain;

/// <summary>
/// Represents a forum group
/// </summary>
public class ForumGroup : BaseEntity
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
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance update
    /// </summary>
    public DateTime UpdatedOnUtc { get; set; }
}