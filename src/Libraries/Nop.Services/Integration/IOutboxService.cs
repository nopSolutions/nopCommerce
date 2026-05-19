namespace Nop.Services.Integration;

/// <summary>
/// Writes integration events to the outbox table for reliable publishing
/// </summary>
public partial interface IOutboxService
{
    /// <summary>
    /// Writes an integration event to the outbox table
    /// </summary>
    /// <param name="eventType">Event type / routing key (e.g. "order.placed")</param>
    /// <param name="data">Event payload — serialized to JSON</param>
    Task WriteEventAsync(string eventType, object data);
}
