namespace Nop.Core.Domain.Events;

/// <summary>
/// Represents an integration event stored in the outbox pattern
/// </summary>
public partial class IntegrationEvent : BaseEntity
{
    /// <summary>
    /// Gets or sets the event type (e.g., "OrderPlaced", "ProductUpdated")
    /// </summary>
    public string EventType { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized event data
    /// </summary>
    public string EventData { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the event was created
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets whether the event has been published to the message broker
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the event was published (null if not yet published)
    /// </summary>
    public DateTime? PublishedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the number of publish attempts (for retry tracking)
    /// </summary>
    public int PublishAttempts { get; set; }

    /// <summary>
    /// Gets or sets the last error message (if publish failed)
    /// </summary>
    public string LastError { get; set; }
}
