namespace Nop.Core.Events;

/// <summary>
/// Represents an event for which processing may be stopped by the consumer
/// </summary>
public partial interface IStopProcessingEvent
{
    /// <summary>
    /// Gets or sets a value whether processing of event publishing should be stopped
    /// </summary>
    bool StopProcessing { get; set; }
}
