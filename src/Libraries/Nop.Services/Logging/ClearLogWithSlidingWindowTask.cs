using Nop.Services.Tasks;

namespace Nop.Services.Logging
{
    /// <summary>
    /// Represents a task to clear [Log] table
    /// </summary>
    public partial class ClearLogWithSlidingWindowTask : IScheduleTask
    {
        #region Fields

        private readonly LogSettings _logSettings;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public ClearLogWithSlidingWindowTask(ILogger logger, LogSettings logSettings)
        {   
            _logger = logger;
            _logSettings = logSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual void Execute()
        {
            var retention = _logSettings.NumberOfDaysToRetainLogs;
            _logger.ClearLogWithRetentionPeriod(retention);
        }

        public static string TaskName => $"{typeof(ClearLogWithSlidingWindowTask).Namespace}.{nameof(ClearLogWithSlidingWindowTask)}";

        public static string TaskDisplayName => "Clear logs (Sliding Window)";

        public static int TaskPeriod => 60 * 60 * 24;

        #endregion
    }
}