using Nop.Plugin.Misc.MailChimp.Services;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.MailChimp {
    public class MailChimpSynchronizationTask : ITask {
        private readonly IMailChimpApiService _mailChimpApiService;

        public MailChimpSynchronizationTask(IMailChimpApiService mailChimpApiService) {
            _mailChimpApiService = mailChimpApiService;
        }

        #region Implementation of ITask

        /// <summary>
        /// Execute task
        /// </summary>
        public void Execute() {
            _mailChimpApiService.BatchSubscribe();
            _mailChimpApiService.BatchUnsubscribe();
        }

        #endregion
    }
}