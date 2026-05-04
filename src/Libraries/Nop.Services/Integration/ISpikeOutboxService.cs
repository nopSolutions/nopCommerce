namespace Nop.Services.Integration;

/// <summary>
/// Represents a spike outbox service for testing the outbox pattern
/// </summary>
public partial interface ISpikeOutboxService
{
    /// <summary>
    /// Writes a spike event to the outbox table on application startup
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task WriteStartupEventAsync();
}
