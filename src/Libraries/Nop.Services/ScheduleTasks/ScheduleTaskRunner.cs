using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.ScheduleTasks
{
    /// <summary>
    /// Schedule task runner
    /// </summary>
    public partial class ScheduleTaskRunner : IScheduleTaskRunner
    {
        #region Fields

        protected ILocalizationService LocalizationService { get; }
        protected ILocker Locker { get; }
        protected ILogger Logger { get; }
        protected IScheduleTaskService ScheduleTaskService { get; }
        protected IStoreContext StoreContext { get; }

        #endregion

        #region Ctor

        public ScheduleTaskRunner(ILocalizationService localizationService,
            ILocker locker,
            ILogger logger,
            IScheduleTaskService scheduleTaskService,
            IStoreContext storeContext)
        {
            LocalizationService = localizationService;
            Locker = locker;
            Logger = logger;
            ScheduleTaskService = scheduleTaskService;
            StoreContext = storeContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Initialize and execute task
        /// </summary>
        protected void ExecuteTask(ScheduleTask scheduleTask)
        {
            var type = Type.GetType(scheduleTask.Type) ??
                       //ensure that it works fine when only the type name is specified (do not require fully qualified names)
                       AppDomain.CurrentDomain.GetAssemblies()
                           .Select(a => a.GetType(scheduleTask.Type))
                           .FirstOrDefault(t => t != null);
            if (type == null)
                throw new Exception($"Schedule task ({scheduleTask.Type}) cannot by instantiated");

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
            ScheduleTaskService.UpdateTaskAsync(scheduleTask).Wait();
            task.ExecuteAsync().Wait();
            scheduleTask.LastEndUtc = scheduleTask.LastSuccessUtc = DateTime.UtcNow;
            //update appropriate datetime properties
            ScheduleTaskService.UpdateTaskAsync(scheduleTask).Wait();
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
                Locker.PerformActionWithLock(scheduleTask.Type, expiration, () => ExecuteTask(scheduleTask));
            }
            catch (Exception exc)
            {
                var store = await StoreContext.GetCurrentStoreAsync();

                var scheduleTaskUrl = $"{store.Url}{NopTaskDefaults.ScheduleTaskPath}";

                scheduleTask.Enabled = !scheduleTask.StopOnError;
                scheduleTask.LastEndUtc = DateTime.UtcNow;
                await ScheduleTaskService.UpdateTaskAsync(scheduleTask);

                var message = string.Format(await LocalizationService.GetResourceAsync("ScheduleTasks.Error"), scheduleTask.Name,
                    exc.Message, scheduleTask.Type, store.Name, scheduleTaskUrl);

                //log error
                await Logger.ErrorAsync(message, exc);
                if (throwException)
                    throw;
            }
        }

        #endregion
    }
}
