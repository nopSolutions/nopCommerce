using Nop.Services.Tasks;

namespace Nop.Services.Logging
{
    /// <summary>
    /// Represents a task to clear [Log] table
    /// </summary>
    public partial class ClearLogTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public ClearLogTask(ILogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual async System.Threading.Tasks.Task ExecuteAsync()
        {
            await _logger.ClearLogAsync();
        }

        #endregion
    }
}