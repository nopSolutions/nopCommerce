namespace Nop.Plugin.Misc.OmnichannelCore.Models.Callbacks;

/// <summary>
/// Represents the common inbound message envelope stored in the inbox
/// </summary>
public record InboxMessageContext
{
    /// <summary>
    /// Gets or sets the message identifier
    /// </summary>
    public Guid MessageId { get; set; }

    /// <summary>
    /// Gets or sets the event type
    /// </summary>
    public string EventType { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier
    /// </summary>
    public string CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the source system
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the related nopCommerce order GUID, when the message is order-related
    /// </summary>
    public Guid? OrderGuid { get; set; }
}
