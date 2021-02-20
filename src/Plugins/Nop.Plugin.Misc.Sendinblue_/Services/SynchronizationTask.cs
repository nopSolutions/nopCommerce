using Nop.Services.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Nop.Plugin.Misc.Sendinblue.Services
{
    /// <summary>
    /// Represents a schedule task to synchronize contacts
    /// </summary>
    public class SynchronizationTask : IScheduleTask
    {
        #region Fields

        private readonly SendinblueManager _sendinBlueEmailManager;

        #endregion

        #region Ctor

        public SynchronizationTask(SendinblueManager sendinBlueEmailManager)
        {
            _sendinBlueEmailManager = sendinBlueEmailManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Execute task
        /// </summary>
        public async Task ExecuteAsync()
        {
            await _sendinBlueEmailManager.SynchronizeAsync();
        }

        #endregion
    }
}