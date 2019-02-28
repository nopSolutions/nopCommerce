using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Tasks;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.Tasks
{
    /// <summary>
    /// Represents task thread
    /// </summary>
    public partial class TaskThread : IDisposable
    {
        #region Fields

        private static readonly string _scheduleTaskUrl;
        private static readonly IStoreContext _storeContext;
        private readonly Dictionary<string, string> _tasks;
        private Timer _timer;
        private bool _disposed;
        private static readonly int? _timeout;

        #endregion

        #region Ctors

        static TaskThread()
        {
            _storeContext = EngineContext.Current.Resolve<IStoreContext>();
            _scheduleTaskUrl = $"{_storeContext.CurrentStore.Url}{NopTaskDefaults.ScheduleTaskPath}";
            _timeout = EngineContext.Current.Resolve<CommonSettings>().ScheduleTaskRunTimeout;
        }

        internal TaskThread()
        {
            _tasks = new Dictionary<string, string>();
            Seconds = 10 * 60;
        }

        #endregion

        #region Utilities

        private void Run()
        {
            if (Seconds <= 0)
                return;

            StartedUtc = DateTime.UtcNow;
            IsRunning = true;
            foreach (var taskName in _tasks.Keys)
            {
                var taskType = _tasks[taskName];

                //create and send post data
                var postData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("taskType", taskType)
                };

                try
                {
                    using (var client = new HttpClient())
                    {
                        if (_timeout.HasValue)
                        {
                            client.Timeout = TimeSpan.FromMilliseconds(_timeout.Value);
                        }

                        var task = client.PostAsync(_scheduleTaskUrl, new FormUrlEncodedContent(postData));
                        task.Wait();
                    }
                }
                catch (Exception ex)
                {
                    var _serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        // Resolve
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();

                        var message = string.Format(localizationService.GetResource("ScheduleTasks.Error"), taskName,
                            ex.Message, taskType, _storeContext.CurrentStore.Name, _scheduleTaskUrl);

                        logger.Error(message, ex);
                    }
                }
            }

            IsRunning = false;
        }

        private void TimerHandler(object state)
        {
            _timer.Change(-1, -1);
            Run();
            if (RunOnlyOnce)
            {
                Dispose();
            }
            else
            {
                _timer.Change(Interval, Interval);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes the instance
        /// </summary>
        public void Dispose()
        {
            if (_timer == null || _disposed)
                return;

            lock (this)
            {
                _timer.Dispose();
                _timer = null;
                _disposed = true;
            }
        }

        /// <summary>
        /// Inits a timer
        /// </summary>
        public void InitTimer()
        {
            if (_timer == null)
            {
                _timer = new Timer(TimerHandler, null, InitInterval, Interval);
            }
        }

        /// <summary>
        /// Adds a task to the thread
        /// </summary>
        /// <param name="task">The task to be added</param>
        public void AddTask(ScheduleTask task)
        {
            if (!_tasks.ContainsKey(task.Name))
            {
                _tasks.Add(task.Name, task.Type);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the interval in seconds at which to run the tasks
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// Get or set the interval before timer first start 
        /// </summary>
        public int InitSeconds { get; set; }

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
                var interval = Seconds * 1000;
                if (interval <= 0)
                    interval = int.MaxValue;
                return interval;
            }
        }

        /// <summary>
        /// Gets the due time interval (in milliseconds) at which to begin start the task
        /// </summary>
        public int InitInterval
        {
            get
            {
                //if somebody entered less than "0" seconds, then an exception could be thrown
                var interval = InitSeconds * 1000;
                if (interval <= 0)
                    interval = 0;
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