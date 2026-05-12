using Nop.Core;

namespace Nop.Plugin.Misc.OmnichannelCore.Domains;

/// <summary>
/// Represents a durable inbound integration message
/// </summary>
public class OmniInboxMessage : BaseEntity
{
    /// <summary>
    /// Gets or sets the external message identifier
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
    /// Gets or sets the inbox status identifier
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Gets or sets the last processing error
    /// </summary>
    public string LastError { get; set; }

    /// <summary>
    /// Gets or sets the received date and time
    /// </summary>
    public DateTime ReceivedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the processed date and time
    /// </summary>
    public DateTime? ProcessedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the updated date and time
    /// </summary>
    public DateTime? UpdatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the inbox status
    /// </summary>
    public OmniInboxMessageStatus Status
    {
        get => (OmniInboxMessageStatus)StatusId;
        set => StatusId = (int)value;
    }
}
