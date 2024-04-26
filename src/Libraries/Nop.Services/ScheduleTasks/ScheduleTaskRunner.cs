using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.ScheduleTasks;

/// <summary>
/// Schedule task runner
/// </summary>
public partial class ScheduleTaskRunner : IScheduleTaskRunner
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly ILocker _locker;
    protected readonly ILogger _logger;
    protected readonly IScheduleTaskService _scheduleTaskService;
    protected readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public ScheduleTaskRunner(ILocalizationService localizationService,
        ILocker locker,
        ILogger logger,
        IScheduleTaskService scheduleTaskService,
        IStoreContext storeContext)
    {
        _localizationService = localizationService;
        _locker = locker;
        _logger = logger;
        _scheduleTaskService = scheduleTaskService;
        _storeContext = storeContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Initialize and execute task
    /// </summary>
    protected virtual async Task PerformTaskAsync(ScheduleTask scheduleTask)
    {
        var type = (Type.GetType(scheduleTask.Type) ??
                    //ensure that it works fine when only the type name is specified (do not require fully qualified names)
                    AppDomain.CurrentDomain.GetAssemblies()
                        .Select(a => a.GetType(scheduleTask.Type))
                        .FirstOrDefault(t => t != null)) ?? throw new Exception($"Schedule task ({scheduleTask.Type}) cannot by instantiated");

        object instance = null;

        try
        {
            instance = EngineContext.Current.Resolve(type);
        }
        catch
        {
            // ignored
        }

        instance ??= EngineContext.Current.ResolveUnregistered(type);

        if (instance is not IScheduleTask task)
            return;

        scheduleTask.LastStartUtc = DateTime.UtcNow;
        //update appropriate datetime properties
        await _scheduleTaskService.UpdateTaskAsync(scheduleTask);
        await task.ExecuteAsync();
        scheduleTask.LastEndUtc = scheduleTask.LastSuccessUtc = DateTime.UtcNow;
        //update appropriate datetime properties
        await _scheduleTaskService.UpdateTaskAsync(scheduleTask);
    }

    /// <summary>
    /// Is task already running?
    /// </summary>
    /// <param name="scheduleTask">Schedule task</param>
    /// <returns>Result</returns>
    protected virtual bool IsTaskAlreadyRunning(ScheduleTask scheduleTask)
    {
        //task run for the first time
        if (!scheduleTask.LastStartUtc.HasValue && !scheduleTask.LastEndUtc.HasValue)
            return false;

        var lastStartUtc = scheduleTask.LastStartUtc ?? DateTime.UtcNow;

        //task already finished
        if (scheduleTask.LastEndUtc.HasValue && lastStartUtc < scheduleTask.LastEndUtc)
            return false;

        //task wasn't finished last time
        if (lastStartUtc.AddSeconds(scheduleTask.Seconds) <= DateTime.UtcNow)
            return false;

        return true;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes the task
    /// </summary>
    /// <param name="scheduleTask">Schedule task</param>
    /// <param name="forceRun">Force run</param>
    /// <param name="throwException">A value indicating whether exception should be thrown if some error happens</param>
    /// <param name="ensureRunOncePerPeriod">A value indicating whether we should ensure this task is run once per run period</param>
    public async Task ExecuteAsync(ScheduleTask scheduleTask, bool forceRun = false, bool throwException = false, bool ensureRunOncePerPeriod = true)
    {
        var enabled = forceRun || (scheduleTask?.Enabled ?? false);

        if (scheduleTask == null || !enabled)
            return;

        if (ensureRunOncePerPeriod)
        {
            //task already running
            if (IsTaskAlreadyRunning(scheduleTask))
                return;

            //validation (so nobody else can invoke this method when he wants)
            if (scheduleTask.LastStartUtc.HasValue && (DateTime.UtcNow - scheduleTask.LastStartUtc).Value.TotalSeconds < scheduleTask.Seconds)
                //too early
                return;
        }

        try
        {
            //get expiration time
            var expirationInSeconds = Math.Min(scheduleTask.Seconds, 300) - 1;
            var expiration = TimeSpan.FromSeconds(expirationInSeconds);

            //execute task with lock
            await _locker.PerformActionWithLockAsync(scheduleTask.Type, expiration, () => PerformTaskAsync(scheduleTask));
        }
        catch (Exception exc)
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            var scheduleTaskUrl = $"{store.Url}{NopTaskDefaults.ScheduleTaskPath}";

            scheduleTask.Enabled = scheduleTask.Enabled && !scheduleTask.StopOnError;
            scheduleTask.LastEndUtc = DateTime.UtcNow;
            await _scheduleTaskService.UpdateTaskAsync(scheduleTask);

            var message = string.Format(await _localizationService.GetResourceAsync("ScheduleTasks.Error"), scheduleTask.Name,
                exc.Message, scheduleTask.Type, store.Name, scheduleTaskUrl);

            //log error
            await _logger.ErrorAsync(message, exc);
            if (throwException)
                throw;
        }
    }

    #endregion
}