using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.ScheduleTasks;

/// <summary>
/// Represents task manager
/// </summary>
public partial class TaskScheduler : ITaskScheduler
{
    #region Fields

    protected static readonly List<TaskThread> _taskThreads = new();
    protected readonly AppSettings _appSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private Task _taskThread;

    #endregion

    #region Ctor

    public TaskScheduler(AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IServiceScopeFactory serviceScopeFactory)
    {
        _appSettings = appSettings;
        _serviceScopeFactory = serviceScopeFactory;
        TaskThread.HttpClientFactory = httpClientFactory;
        TaskThread.ServiceScopeFactory = serviceScopeFactory;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes task scheduler
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InitializeAsync()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        if (_taskThreads.Any())
            return;

        using var scope = _serviceScopeFactory.CreateScope();
        var scheduleTaskService = scope.ServiceProvider.GetService<IScheduleTaskService>() ?? throw new NullReferenceException($"Can't get {nameof(IScheduleTaskService)} implementation from the scope");

        //initialize and start schedule tasks
        var scheduleTasks = (await scheduleTaskService.GetAllTasksAsync())
            .OrderBy(x => x.Seconds)
            .ToList();

        var storeContext = scope.ServiceProvider.GetService<IStoreContext>() ?? throw new NullReferenceException($"Can't get {nameof(IStoreContext)} implementation from the scope");

        var store = await storeContext.GetCurrentStoreAsync();

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
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task StartSchedulerAsync()
    {
        _taskThread = Task.WhenAll(_taskThreads.Select(taskThread => taskThread.InitTimerAsync()));

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the task scheduler
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task StopSchedulerAsync()
    {
        foreach (var taskThread in _taskThreads)
            taskThread.Dispose();

        try
        {
            if (_taskThread is { IsCompleted: false })
                await _taskThread;
        }
        catch
        {
           //ignore
        }

        _taskThread?.Dispose();
        _taskThreads.Clear();
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
        protected bool _disposed;
        protected readonly CancellationTokenSource _cancellationToken;

        #endregion

        #region Ctor

        public TaskThread(ScheduleTask task, string scheduleTaskUrl, int? timeout)
        {
            _scheduleTaskUrl = scheduleTaskUrl;
            _scheduleTask = task;
            _timeout = timeout;
            IsStarted = false;
            _cancellationToken = new CancellationTokenSource();
            
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
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">Specifies whether to disposing resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            IsStarted = false;
            _disposed = true;
            _cancellationToken.Cancel();
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
        public async Task InitTimerAsync()
        {
            var interval = TimeSpan.FromMilliseconds(Interval);
            IsStarted = true;
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(InitInterval > 0 ? InitInterval : 1));
            
            while (await timer.WaitForNextTickAsync(_cancellationToken.Token))
            {
                if (IsDisposed)
                    break;

                try
                {
                    await RunAsync();
                }
                catch
                {
                    // ignore
                }
                finally
                {
                    if (!IsDisposed && RunOnlyOnce) 
                        Dispose();
                }

                if (timer.Period != interval)
                    timer.Period = interval;
            }
        }

        #endregion

        #region Properties

        internal static IHttpClientFactory HttpClientFactory { get; set; }

        internal static IServiceScopeFactory ServiceScopeFactory { get; set; }

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
        public bool IsStarted { get; set; }

        /// <summary>
        /// Gets a value indicating whether the timer is disposed
        /// </summary>
        public bool IsDisposed => _disposed;

        #endregion
    }

    #endregion
}