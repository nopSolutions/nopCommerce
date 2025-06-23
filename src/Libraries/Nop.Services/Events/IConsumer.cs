namespace Nop.Services.Events;

/// <summary>
/// Consumer interface
/// </summary>
/// <typeparam name="T">Type</typeparam>
public partial interface IConsumer<T>
{
    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task HandleEventAsync(T eventMessage);

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    void HandleEvent(T eventMessage)
    {
        HandleEventAsync(eventMessage).Wait();
    }
}