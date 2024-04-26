namespace Nop.Services.ScheduleTasks;

/// <summary>
/// Interface that should be implemented by each task
/// </summary>
public partial interface IScheduleTask
{
    /// <summary>
    /// Executes a task
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task ExecuteAsync();
}