using System;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Tasks;
using Nop.Core.Infrastructure;
using Nop.Services.Infrastructure;
using Nop.Services.Logging;

namespace Nop.Services.Tasks
{
    /// <summary>
    /// Task
    /// </summary>
    public partial class Task
    {
        #region Fields

        private bool? _enabled;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor for Task
        /// </summary>
        /// <param name="task">Task </param>
        public Task(ScheduleTask task)
        {
            ScheduleTask = task;
        }

        #endregion

        #region Utilities

        private ITask CreateTask()
        {
            if (!Enabled)
                return null;

            var type = Type.GetType(ScheduleTask.Type);
            if (type == null)
                return null;

            object instance = null;
            try
            {
                instance = EngineContext.Current.Resolve(type);
            }
            catch
            {
                //try resolve
            }
            if (instance == null)
            {
                //not resolved
                instance = EngineContext.Current.ResolveUnregistered(type);
            }

            return instance as ITask;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="throwException">A value indicating whether exception should be thrown if some error happens</param>
        /// <param name="ensureRunOnOneWebFarmInstance">A value indicating whether we should ensure this task is run on one farm node at a time</param>
        public void Execute(bool throwException = false, bool ensureRunOnOneWebFarmInstance = true)
        {
            var scheduleTaskService = EngineContext.Current.Resolve<IScheduleTaskService>();

            if (ScheduleTask == null || !Enabled)
                return;

            try
            {
                //flag that task is already executed
                var taskExecuted = false;

                //task is run on one farm node at a time?
                if (ensureRunOnOneWebFarmInstance)
                {
                    //is web farm enabled (multiple instances)?
                    var nopConfig = EngineContext.Current.Resolve<NopConfig>();
                    if (nopConfig.MultipleInstancesEnabled)
                    {
                        var machineNameProvider = EngineContext.Current.Resolve<IMachineNameProvider>();
                        var machineName = machineNameProvider.GetMachineName();
                        if (String.IsNullOrEmpty(machineName))
                        {
                            throw new Exception("Machine name cannot be detected. You cannot run in web farm.");
                            //actually in this case we can generate some unique string (e.g. GUID) and store it in some "static" (!!!) variable
                            //then it can be used as a machine name
                        }
                        if (nopConfig.RedisCachingEnabled)
                        {
                            //get expiration time
                            var expirationInSeconds = ScheduleTask.Seconds <= 300 ? ScheduleTask.Seconds - 1 : 300;

                            var executeTaskAction = new Action(() =>
                            {
                                //execute task
                                taskExecuted = true;
                                var task = this.CreateTask();
                                if (task != null)
                                {
                                    //update appropriate datetime properties
                                    ScheduleTask.LastStartUtc = DateTime.UtcNow;
                                    scheduleTaskService.UpdateTask(ScheduleTask);
                                    task.Execute();
                                    ScheduleTask.LastEndUtc = ScheduleTask.LastSuccessUtc = DateTime.UtcNow;
                                }
                            });

                            //execute task with lock
                            var redisWrapper = EngineContext.Current.Resolve<IRedisConnectionWrapper>();
                            if (!redisWrapper.PerformActionWithLock(ScheduleTask.Type,
                                TimeSpan.FromSeconds(expirationInSeconds), executeTaskAction))
                                return;
                        }
                        else
                        {
                            //lease can't be acquired only if for a different machine and it has not expired
                            if (ScheduleTask.LeasedUntilUtc.HasValue &&
                                ScheduleTask.LeasedUntilUtc.Value >= DateTime.UtcNow &&
                                ScheduleTask.LeasedByMachineName != machineName)
                                return;

                            //lease the task. so it's run on one farm node at a time
                            ScheduleTask.LeasedByMachineName = machineName;
                            ScheduleTask.LeasedUntilUtc = DateTime.UtcNow.AddMinutes(30);
                            scheduleTaskService.UpdateTask(ScheduleTask);
                        }
                    }
                }

                //execute task in case if is not executed yet
                if (!taskExecuted)
                {
                    //initialize and execute
                    var task = this.CreateTask();
                    if (task != null)
                    {
                        ScheduleTask.LastStartUtc = DateTime.UtcNow;
                        scheduleTaskService.UpdateTask(ScheduleTask);
                        task.Execute();
                        ScheduleTask.LastEndUtc = ScheduleTask.LastSuccessUtc = DateTime.UtcNow;
                    }
                }
            }
            catch (Exception exc)
            {
                ScheduleTask.Enabled = !ScheduleTask.StopOnError;
                ScheduleTask.LastEndUtc = DateTime.UtcNow;

                //log error
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error(string.Format("Error while running the '{0}' schedule task. {1}", ScheduleTask.Name, exc.Message), exc);
                if (throwException)
                    throw;
            }

            //update appropriate datetime properties
            scheduleTaskService.UpdateTask(ScheduleTask);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Schedule task
        /// </summary>
        public ScheduleTask ScheduleTask { get; }

        /// <summary>
        /// A value indicating whether the task is enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                if (!_enabled.HasValue)
                    _enabled = ScheduleTask?.Enabled;

                    return _enabled.HasValue && _enabled.Value;
            }
            set => _enabled = value;
        }

        #endregion
    }
}
