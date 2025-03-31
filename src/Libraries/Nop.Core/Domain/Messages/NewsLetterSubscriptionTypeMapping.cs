namespace Nop.Core.Domain.Messages;

/// <summary>
/// Represents a NewsLetterSubscription and NewsLetterSubscriptionType mapping
/// </summary>
public partial class NewsLetterSubscriptionTypeMapping : BaseEntity
{
    /// <summary>
    /// Gets or sets the news letter subscription identifier
    /// </summary>
    public int NewsLetterSubscriptionId { get; set; }

    /// <summary>
    /// Gets or sets the news letter subscription type identifier
    /// </summary>
    public int NewsLetterSubscriptionTypeId { get; set; }
}
