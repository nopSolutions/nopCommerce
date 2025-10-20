namespace Nop.Core.Events;

/// <summary>
/// Event that is published when the application has started.
/// This event can be consumed by plugins or services that need to perform actions
/// once the application is fully initialized.
/// </summary>
public partial class AppStartedEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppStartedEvent"/> class.
    /// Sets the timestamp of when the application was started in UTC.
    /// </summary>
    public AppStartedEvent()
    {
        StartedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the UTC date and time when the application started.
    /// </summary>
    public DateTime StartedAtUtc { get; }
}