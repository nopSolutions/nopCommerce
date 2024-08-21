using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.Brevo.Services;

/// <summary>
/// Represents a schedule task to synchronize contacts
/// </summary>
public class SynchronizationTask : IScheduleTask
{
    #region Fields

    protected readonly BrevoManager _brevoEmailManager;

    #endregion

    #region Ctor

    public SynchronizationTask(BrevoManager brevoEmailManager)
    {
        _brevoEmailManager = brevoEmailManager;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Execute task
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ExecuteAsync()
    {
        await _brevoEmailManager.SynchronizeAsync();
    }

    #endregion
}