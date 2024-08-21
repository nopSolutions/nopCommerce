namespace Nop.Services.ScheduleTasks;

/// <summary>
/// Task manager interface
/// </summary>
public partial interface ITaskScheduler
{
    /// <summary>
    /// Initializes task scheduler
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InitializeAsync();

    /// <summary>
    /// Starts the task scheduler
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task StartSchedulerAsync();

    /// <summary>
    /// Stops the task scheduler
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task StopSchedulerAsync();
}