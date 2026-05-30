namespace Nop.Core.Caching;

public partial interface ILocker
{
    /// <summary>
    /// Performs some asynchronous task with exclusive lock
    /// </summary>
    /// <param name="resource">The key we are locking on</param>
    /// <param name="expirationTime">The time after which the lock will automatically be expired</param>
    /// <param name="action">Asynchronous task to be performed with locking</param>
    /// <returns>A task that resolves true if lock was acquired and action was performed; otherwise false</returns>
    Task<bool> PerformActionWithLockAsync(string resource, TimeSpan expirationTime, Func<Task> action);

    /// <summary>
    /// Starts a background task with "heartbeat": a status flag that will be periodically updated to signal to
    /// others that the task is running and stop them from starting the same task.
    /// </summary>
    /// <param name="key">The key of the background task</param>
    /// <param name="expirationTime">The time after which the heartbeat key will automatically be expired. Should be longer than <paramref name="heartbeatInterval"/></param>
    /// <param name="heartbeatInterval">The interval at which to update the heartbeat, if required by the implementation</param>
    /// <param name="action">Asynchronous background task to be performed</param>
    /// <param name="cancellationTokenSource">A CancellationTokenSource for manually canceling the task</param>
    /// <returns>A task that resolves true if lock was acquired and action was performed; otherwise false</returns>
    Task RunWithHeartbeatAsync(string key, TimeSpan expirationTime, TimeSpan heartbeatInterval, Func<CancellationToken, Task> action, CancellationTokenSource cancellationTokenSource = default);

    /// <summary>
    /// Tries to cancel a background task by flagging it for cancellation on the next heartbeat.
    /// </summary>
    /// <param name="key">The task's key</param>
    /// <param name="expirationTime">The time after which the task will be considered stopped due to system shutdown or other causes,
    /// even if not explicitly canceled.</param>
    /// <returns>A task that represents requesting cancellation of the task. Note that the completion of this task does not
    /// necessarily imply that the task has been canceled, only that cancellation has been requested.</returns>

    Task CancelTaskAsync(string key, TimeSpan expirationTime);

    /// <summary>
    /// Check if a background task is running.
    /// </summary>
    /// <param name="key">The task's key</param>
    /// <returns>A task that resolves to true if the background task is running; otherwise false</returns>
    Task<bool> IsTaskRunningAsync(string key);
}