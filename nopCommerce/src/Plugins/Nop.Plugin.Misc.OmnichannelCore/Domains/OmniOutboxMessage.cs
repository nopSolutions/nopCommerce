using Nop.Core;

namespace Nop.Plugin.Misc.OmnichannelCore.Domains;

/// <summary>
/// Represents a durable outbound integration message
/// </summary>
public class OmniOutboxMessage : BaseEntity
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
    /// Gets or sets the related order GUID
    /// </summary>
    public Guid? OrderGuid { get; set; }

    /// <summary>
    /// Gets or sets the related order identifier
    /// </summary>
    public int? OrderId { get; set; }

    /// <summary>
    /// Gets or sets the serialized integration event payload
    /// </summary>
    public string Payload { get; set; }

    /// <summary>
    /// Gets or sets the outbox status identifier
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Gets or sets the retry count
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Gets or sets the last publishing error
    /// </summary>
    public string LastError { get; set; }

    /// <summary>
    /// Gets or sets the published date and time
    /// </summary>
    public DateTime? PublishedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the next publishing attempt date and time
    /// </summary>
    public DateTime? NextAttemptOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the created date and time
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the updated date and time
    /// </summary>
    public DateTime? UpdatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the outbox status
    /// </summary>
    public OmniOutboxMessageStatus Status
    {
        get => (OmniOutboxMessageStatus)StatusId;
        set => StatusId = (int)value;
    }
}
