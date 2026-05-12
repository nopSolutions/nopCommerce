using Nop.Core;

namespace Nop.Plugin.Messaging.RabbitMq.Domain;

public class OutboxMessage : BaseEntity
{
    public string AggregateId { get; set; }
    public string EventType { get; set; }
    public string Payload { get; set; }
    public int Status { get; set; }
    public int AttemptCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? SentAtUtc { get; set; }
}

public enum OutboxMessageStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2
}
