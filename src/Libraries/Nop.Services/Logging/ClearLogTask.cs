using Nop.Services.Tasks;

namespace Nop.Services.Logging
{
    /// <summary>
    /// Represents a task to clear [Log] table
    /// </summary>
    public partial class ClearLogTask : ITask
    {
        private readonly ILogger _logger;

        public ClearLogTask(ILogger logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual void Execute()
        {
            _logger.ClearLog();
        }
    }
}
