using System;
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
        #region Ctor

        /// <summary>
        /// Ctor for Task
        /// </summary>
        private Task()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Ctor for Task
        /// </summary>
        /// <param name="task">Task </param>
        public Task(ScheduleTask task)
        {
            this.Type = task.Type;
            this.Enabled = task.Enabled;
            this.StopOnError = task.StopOnError;
            this.Name = task.Name;
            this.LastSuccessUtc = task.LastSuccessUtc;
        }

        #endregion

        #region Utilities

        private ITask CreateTask()
        {
            if (!this.Enabled)
                return null;

            var type = System.Type.GetType(this.Type);
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
            var scheduleTask = scheduleTaskService.GetTaskByType(this.Type);

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
                            //actually in this case we can generate some unique string (e.g. Guid) and store it in some "static" (!!!) variable
                            //then it can be used as a machine name
                        }

                        if (scheduleTask != null)
                        {
                            if (nopConfig.RedisCachingEnabled)
                            {
                                //get expiration time
                                var expirationInSeconds = scheduleTask.Seconds <= 300 ? scheduleTask.Seconds - 1 : 300;

                                var executeTaskAction = new Action(() =>
                                {
                                    //execute task
                                    taskExecuted = true;
                                    var task = this.CreateTask();
                                    if (task != null)
                                    {
                                        //update appropriate datetime properties
                                        scheduleTask.LastStartUtc = DateTime.UtcNow;
                                        scheduleTaskService.UpdateTask(scheduleTask);
                                        task.Execute();
                                        this.LastEndUtc = this.LastSuccessUtc = DateTime.UtcNow;
                                    }
                                });

                                //execute task with lock
#if NET451
                                var redisWrapper = EngineContext.Current.ContainerManager.Resolve<IRedisConnectionWrapper>(scope: scope);
                                if (!redisWrapper.PerformActionWithLock(scheduleTask.Type, TimeSpan.FromSeconds(expirationInSeconds), executeTaskAction))
                                    return;
#endif
                            }
                            else
                            {
                                //lease can't be acquired only if for a different machine and it has not expired
                                if (scheduleTask.LeasedUntilUtc.HasValue &&
                                    scheduleTask.LeasedUntilUtc.Value >= DateTime.UtcNow &&
                                    scheduleTask.LeasedByMachineName != machineName)
                                    return;

                                //lease the task. so it's run on one farm node at a time
                                scheduleTask.LeasedByMachineName = machineName;
                                scheduleTask.LeasedUntilUtc = DateTime.UtcNow.AddMinutes(30);
                                scheduleTaskService.UpdateTask(scheduleTask);
                            }
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
                        this.LastStartUtc = DateTime.UtcNow;
                        if (scheduleTask != null)
                        {
                            //update appropriate datetime properties
                            scheduleTask.LastStartUtc = this.LastStartUtc;
                            scheduleTaskService.UpdateTask(scheduleTask);
                        }
                        task.Execute();
                        this.LastEndUtc = this.LastSuccessUtc = DateTime.UtcNow;
                    }
                }
            }
            catch (Exception exc)
            {
                this.Enabled = !this.StopOnError;
                this.LastEndUtc = DateTime.UtcNow;

                //log error
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error(string.Format("Error while running the '{0}' schedule task. {1}", this.Name, exc.Message), exc);
                if (throwException)
                    throw;
            }

            if (scheduleTask != null)
            {
                //update appropriate datetime properties
                scheduleTask.LastEndUtc = this.LastEndUtc;
                scheduleTask.LastSuccessUtc = this.LastSuccessUtc;
                scheduleTaskService.UpdateTask(scheduleTask);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Datetime of the last start
        /// </summary>
        public DateTime? LastStartUtc { get; private set; }

        /// <summary>
        /// Datetime of the last end
        /// </summary>
        public DateTime? LastEndUtc { get; private set; }

        /// <summary>
        /// Datetime of the last success
        /// </summary>
        public DateTime? LastSuccessUtc { get; private set; }

        /// <summary>
        /// A value indicating type of the task
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// A value indicating whether to stop task on error
        /// </summary>
        public bool StopOnError { get; private set; }

        /// <summary>
        /// Get the task name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A value indicating whether the task is enabled
        /// </summary>
        public bool Enabled { get; set; }

        #endregion
    }
}
