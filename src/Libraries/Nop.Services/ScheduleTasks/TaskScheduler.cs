using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.ScheduleTasks
{
    /// <summary>
    /// Represents task manager
    /// </summary>
    public partial class TaskScheduler : ITaskScheduler
    {
        #region Fields

        protected static readonly List<TaskThread> _taskThreads = new();
        protected readonly AppSettings _appSettings;
        protected readonly IScheduleTaskService _scheduleTaskService;
        protected readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public TaskScheduler(AppSettings appSettings,
            IHttpClientFactory httpClientFactory,
            IScheduleTaskService scheduleTaskService,
            IServiceScopeFactory serviceScopeFactory,
            IStoreContext storeContext)
        {
            _appSettings = appSettings;
            TaskThread.HttpClientFactory = httpClientFactory;
            _scheduleTaskService = scheduleTaskService;
            TaskThread.ServiceScopeFactory = serviceScopeFactory;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the task manager
        /// </summary>
        public async Task InitializeAsync()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            if (_taskThreads.Any())
                return;

            //initialize and start schedule tasks
            var scheduleTasks = (await _scheduleTaskService.GetAllTasksAsync())
                .OrderBy(x => x.Seconds)
                .ToList();

            var store = await _storeContext.GetCurrentStoreAsync();

            var scheduleTaskUrl = $"{store.Url.TrimEnd('/')}/{NopTaskDefaults.ScheduleTaskPath}";
            var timeout = _appSettings.Get<CommonConfig>().ScheduleTaskRunTimeout;

            foreach (var scheduleTask in scheduleTasks)
            {
                var taskThread = new TaskThread(scheduleTask, scheduleTaskUrl, timeout)
                {
                    Seconds = scheduleTask.Seconds
                };

                //sometimes a task period could be set to several hours (or even days)
                //in this case a probability that it'll be run is quite small (an application could be restarted)
                //calculate time before start an interrupted task
                if (scheduleTask.LastStartUtc.HasValue)
                {
                    //seconds left since the last start
                    var secondsLeft = (DateTime.UtcNow - scheduleTask.LastStartUtc).Value.TotalSeconds;

                    if (secondsLeft >= scheduleTask.Seconds)
                        //run now (immediately)
                        taskThread.InitSeconds = 0;
                    else
                        //calculate start time
                        //and round it (so "ensureRunOncePerPeriod" parameter was fine)
                        taskThread.InitSeconds = (int)(scheduleTask.Seconds - secondsLeft) + 1;
                }
                else if (scheduleTask.LastEnabledUtc.HasValue)
                {
                    //seconds left since the last enable
                    var secondsLeft = (DateTime.UtcNow - scheduleTask.LastEnabledUtc).Value.TotalSeconds;

                    if (secondsLeft >= scheduleTask.Seconds)
                        //run now (immediately)
                        taskThread.InitSeconds = 0;
                    else
                        //calculate start time
                        //and round it (so "ensureRunOncePerPeriod" parameter was fine)
                        taskThread.InitSeconds = (int)(scheduleTask.Seconds - secondsLeft) + 1;
                }
                else
                    //first start of a task
                    taskThread.InitSeconds = scheduleTask.Seconds;

                _taskThreads.Add(taskThread);
            }
        }

        /// <summary>
        /// Starts the task scheduler
        /// </summary>
        public void StartScheduler()
        {
            foreach (var taskThread in _taskThreads)
                taskThread.InitTimer();
        }

        /// <summary>
        /// Stops the task scheduler
        /// </summary>
        public void StopScheduler()
        {
            foreach (var taskThread in _taskThreads)
                taskThread.Dispose();
        }

        #endregion

        #region Nested class

        /// <summary>
        /// Represents task thread
        /// </summary>
        protected partial class TaskThread : IDisposable
        {
            #region Fields

            protected readonly string _scheduleTaskUrl;
            protected readonly ScheduleTask _scheduleTask;
            protected readonly int? _timeout;

            protected Timer _timer;
            protected bool _disposed;

            internal static IHttpClientFactory HttpClientFactory { get; set; }
            internal static IServiceScopeFactory ServiceScopeFactory { get; set; }

            #endregion

            #region Ctor

            public TaskThread(ScheduleTask task, string scheduleTaskUrl, int? timeout)
            {
                _scheduleTaskUrl = scheduleTaskUrl;
                _scheduleTask = task;
                _timeout = timeout;

                Seconds = 10 * 60;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Run task
            /// </summary>
            /// <returns>A task that represents the asynchronous operation</returns>
            protected virtual async Task RunAsync()
            {
                if (Seconds <= 0)
                    return;

                StartedUtc = DateTime.UtcNow;
                IsRunning = true;
                HttpClient client = null;

                try
                {
                    //create and configure client
                    client = HttpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
                    if (_timeout.HasValue)
                        client.Timeout = TimeSpan.FromMilliseconds(_timeout.Value);

                    //send post data
                    var data = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("taskType", _scheduleTask.Type) });
                    await client.PostAsync(_scheduleTaskUrl, data);
                }
                catch (Exception ex)
                {
                    using var scope = ServiceScopeFactory.CreateScope();

                    // Resolve
                    var logger = EngineContext.Current.Resolve<ILogger>(scope);
                    var localizationService = EngineContext.Current.Resolve<ILocalizationService>(scope);
                    var storeContext = EngineContext.Current.Resolve<IStoreContext>(scope);

                    var message = ex.InnerException?.GetType() == typeof(TaskCanceledException) ? await localizationService.GetResourceAsync("ScheduleTasks.TimeoutError") : ex.Message;
                    var store = await storeContext.GetCurrentStoreAsync();

                    message = string.Format(await localizationService.GetResourceAsync("ScheduleTasks.Error"), _scheduleTask.Name,
                        message, _scheduleTask.Type, store.Name, _scheduleTaskUrl);

                    await logger.ErrorAsync(message, ex);
                }
                finally
                {
                    client?.Dispose();
                }

                IsRunning = false;
            }

            /// <summary>
            /// Method that handles calls from a <see cref="T:System.Threading.Timer" />
            /// </summary>
            /// <param name="state">An object containing application-specific information relevant to the method invoked by this delegate</param>
            protected void TimerHandler(object state)
            {
                try
                {
                    _timer.Change(-1, -1);

                    RunAsync().Wait();
                }
                catch
                {
                    // ignore
                }
                finally
                {
                    if (!_disposed && _timer != null)
                    {
                        if (RunOnlyOnce)
                            Dispose();
                        else
                            _timer.Change(Interval, Interval);
                    }
                }
            }

            /// <summary>
            /// Protected implementation of Dispose pattern.
            /// </summary>
            /// <param name="disposing">Specifies whether to disposing resources</param>
            protected virtual void Dispose(bool disposing)
            {
                if (_disposed)
                    return;

                if (disposing)
                    lock (this)
                        _timer?.Dispose();

                _disposed = true;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Disposes the instance
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Inits a timer
            /// </summary>
            public void InitTimer()
            {
                _timer ??= new Timer(TimerHandler, null, InitInterval, Interval);
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
            public DateTime StartedUtc { get; protected set; }

            /// <summary>
            /// Get or sets a value indicating whether thread is running
            /// </summary>
            public bool IsRunning { get; protected set; }

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

            /// <summary>
            /// Gets a value indicating whether the timer is started
            /// </summary>
            public bool IsStarted => _timer != null;

            /// <summary>
            /// Gets a value indicating whether the timer is disposed
            /// </summary>
            public bool IsDisposed => _disposed;

            #endregion
        }

        #endregion
    }
}
