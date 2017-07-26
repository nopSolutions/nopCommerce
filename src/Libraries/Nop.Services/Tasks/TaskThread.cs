using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading;
using Nop.Core;
using Nop.Core.Domain.Tasks;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;

namespace Nop.Services.Tasks
{
    /// <summary>
    /// Represents task thread
    /// </summary>
    public partial class TaskThread : IDisposable
    {
        #region Fields

        private Timer _timer;
        private bool _disposed;
        private readonly Dictionary<string, string> _tasks;
        private static readonly string _scheduleTaskUrl;

        #endregion

        #region Ctors

        static TaskThread()
        {
            var storeContext = EngineContext.Current.Resolve<IStoreContext>();
            
            _scheduleTaskUrl = storeContext.CurrentStore.Url + TaskManager.ScheduleTaskPatch;
        }

        internal TaskThread()
        {
            this._tasks = new Dictionary<string, string>();
            this.Seconds = 10 * 60;
        }

        #endregion

        #region Utilities

        private void Run()
        {
            if (Seconds <= 0)
                return;

            this.StartedUtc = DateTime.UtcNow;
            this.IsRunning = true;
            foreach (var taskType in _tasks.Values)
            {
                //create and send post data
                var postData = new NameValueCollection
                {
                    {"taskType", taskType}
                };

                try
                {
                    using (var client = new WebClient())
                    {
                        client.UploadValues(_scheduleTaskUrl, postData);
                    }
                }
                catch (Exception ex)
                {
                    var logger = EngineContext.Current.Resolve<ILogger>();
                    logger.Error(ex.Message, ex);
                }
               
            }
            this.IsRunning = false;
        }

        private void TimerHandler(object state)
        {
            this._timer.Change(-1, -1);
            this.Run();
            if (this.RunOnlyOnce)
            {
                this.Dispose();
            }
            else
            {
                this._timer.Change(this.Interval, this.Interval);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes the instance
        /// </summary>
        public void Dispose()
        {
            if ((this._timer != null) && !this._disposed)
            {
                lock (this)
                {
                    this._timer.Dispose();
                    this._timer = null;
                    this._disposed = true;
                }
            }
        }

        /// <summary>
        /// Inits a timer
        /// </summary>
        public void InitTimer()
        {
            if (this._timer == null)
            {
                this._timer = new Timer(this.TimerHandler, null, this.Interval, this.Interval);
            }
        }

        /// <summary>
        /// Adds a task to the thread
        /// </summary>
        /// <param name="task">The task to be added</param>
        public void AddTask(ScheduleTask task)
        {
            if (!this._tasks.ContainsKey(task.Name))
            {
                this._tasks.Add(task.Name, task.Type);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the interval in seconds at which to run the tasks
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// Get or sets a datetime when thread has been started
        /// </summary>
        public DateTime StartedUtc { get; private set; }

        /// <summary>
        /// Get or sets a value indicating whether thread is running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the interval (in milliseconds) at which to run the task
        /// </summary>
        public int Interval
        {
            get
            {
                //if somebody entered more than "2147483" seconds, then an exception could be thrown (exceeds int.MaxValue)
                var interval = this.Seconds * 1000;
                if (interval <= 0)
                    interval = int.MaxValue;
                return interval;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the thread would be run only once (on application start)
        /// </summary>
        public bool RunOnlyOnce { get; set; }

        #endregion
    }
}
