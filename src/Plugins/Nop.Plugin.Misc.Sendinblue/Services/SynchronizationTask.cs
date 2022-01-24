using System.Threading.Tasks;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.Sendinblue.Services
{
    /// <summary>
    /// Represents a schedule task to synchronize contacts
    /// </summary>
    public class SynchronizationTask : IScheduleTask
    {
        #region Fields

        private readonly SendinblueManager _sendinblueEmailManager;

        #endregion

        #region Ctor

        public SynchronizationTask(SendinblueManager sendinblueEmailManager)
        {
            _sendinblueEmailManager = sendinblueEmailManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Execute task
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ExecuteAsync()
        {
            await _sendinblueEmailManager.SynchronizeAsync();
        }

        #endregion
    }
}