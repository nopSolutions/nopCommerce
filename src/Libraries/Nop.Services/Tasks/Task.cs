using System;
using Nop.Core.Domain.Tasks;

namespace Nop.Services.Tasks
{
    /// <summary>
    /// Task
    /// </summary>
    public partial class Task
    {
        private ITask _task;
        private bool _enabled;
        private readonly Type _taskType;
        private readonly string _name;
        private readonly bool _stopOnError;
        private DateTime _lastStarted;
        private DateTime _lastSuccess;
        private DateTime _lastEnd;
        private bool _isRunning;

        /// <summary>
        /// Ctor for Task
        /// </summary>
        private Task()
        {
            this._enabled = true;
        }

        /// <summary>
        /// Ctor for Task
        /// </summary>
        /// <param name="task">Task </param>
        public Task(ScheduleTask task)
        {
            this._taskType = Type.GetType(task.Type);
            this._enabled = task.Enabled;
            this._stopOnError = task.StopOnError;
            this._name = task.Name;
        }

        private ITask CreateTask()
        {
            if (this.Enabled && (this._task == null))
            {
                if (this._taskType != null)
                {
                    this._task = Activator.CreateInstance(this._taskType) as ITask;
                }
                this._enabled = this._task != null;
            }
            return this._task;
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        public void Execute()
        {
            this._isRunning = true;
            try
            {
                var task = this.CreateTask();
                if (task != null)
                {
                    this._lastStarted = DateTime.Now;
                    task.Execute();
                    this._lastEnd = this._lastSuccess = DateTime.Now;
                }
            }
            catch (Exception exception)
            {
                this._enabled = !this.StopOnError;
                this._lastEnd = DateTime.Now;
                TaskManager.Instance.ProcessException(this, exception);
            }
            this._isRunning = false;
        }

        /// <summary>
        /// A value indicating whether a task is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this._isRunning;
            }
        }

        /// <summary>
        /// Datetime of the last start
        /// </summary>
        public DateTime LastStarted
        {
            get
            {
                return this._lastStarted;
            }
        }

        /// <summary>
        /// Datetime of the last end
        /// </summary>
        public DateTime LastEnd
        {
            get
            {
                return this._lastEnd;
            }
        }

        /// <summary>
        /// Datetime of the last success
        /// </summary>
        public DateTime LastSuccess
        {
            get
            {
                return this._lastSuccess;
            }
        }

        /// <summary>
        /// A value indicating type of the task
        /// </summary>
        public Type TaskType
        {
            get
            {
                return this._taskType;
            }
        }

        /// <summary>
        /// A value indicating whether to stop task on error
        /// </summary>
        public bool StopOnError
        {
            get
            {
                return this._stopOnError;
            }
        }

        /// <summary>
        /// Get the task name
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>
        /// A value indicating whether the task is enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this._enabled;
            }
        }
    }
}
