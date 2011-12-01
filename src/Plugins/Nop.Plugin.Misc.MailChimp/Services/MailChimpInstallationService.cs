using Nop.Plugin.Misc.MailChimp.Data;

namespace Nop.Plugin.Misc.MailChimp.Services {
    public class MailChimpInstallationService {
        private readonly MailChimpObjectContext _mailChimpObjectContext;

        public MailChimpInstallationService(MailChimpObjectContext mailChimpObjectContext) {
            _mailChimpObjectContext = mailChimpObjectContext;
        }

        /// <summary>
        /// Installs this instance.
        /// </summary>
        public virtual void Install() {
            _mailChimpObjectContext.Install();
        }

        /// <summary>
        /// Uninstalls this instance.
        /// </summary>
        public virtual void Uninstall() {
            _mailChimpObjectContext.Uninstall();
        }
    }
}