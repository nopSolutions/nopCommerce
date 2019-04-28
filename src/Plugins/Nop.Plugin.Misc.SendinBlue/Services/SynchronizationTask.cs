using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.SendinBlue.Services
{
    /// <summary>
    /// Represents a schedule task to synchronize contacts
    /// </summary>
    public class SynchronizationTask : IScheduleTask
    {
        #region Fields

        private readonly SendinBlueManager _sendinBlueEmailManager;

        #endregion

        #region Ctor

        public SynchronizationTask(SendinBlueManager sendinBlueEmailManager)
        {
            _sendinBlueEmailManager = sendinBlueEmailManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Execute task
        /// </summary>
        public void Execute()
        {
            _sendinBlueEmailManager.Synchronize();
        }

        #endregion
    }
}