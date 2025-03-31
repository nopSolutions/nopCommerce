using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Messages;

/// <summary>
/// Represents newsletter subscription type entity
/// </summary>
public partial class NewsLetterSubscriptionType : BaseEntity, ILocalizedEntity, IStoreMappingSupported
{
    /// <summary>
    /// Gets or sets the name of subscription type
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

    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public bool LimitedToStores { get; set; }
}
