using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Messages;

/// <summary>
/// Represents newsletter subscription type entity
/// </summary>
public partial class NewsLetterSubscriptionType : BaseEntity, ILocalizedEntity
{
    /// <summary>
    /// Gets or sets the name of subscribtion type
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether subscription type is active
    /// </summary>
    public bool TickedByDefault { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}
