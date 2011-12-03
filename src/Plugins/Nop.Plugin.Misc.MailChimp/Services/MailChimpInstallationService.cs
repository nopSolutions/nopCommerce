using System;
using Nop.Core.Domain.Localization;
using Nop.Plugin.Misc.MailChimp.Data;
using Nop.Services.Localization;

namespace Nop.Plugin.Misc.MailChimp.Services {
    public class MailChimpInstallationService {
        private const string API_KEY_NAME = "Nop.Plugin.Misc.MailChimp.ApiKey";
        private const string DEFAULT_LIST_NAME = "Nop.Plugin.Misc.MailChimp.DefaultListId";
        private const string AUTO_SYNC_NAME = "Nop.Plugin.Misc.MailChimp.AutoSync";

        private readonly ILocalizationService _localizationService;
        private readonly MailChimpObjectContext _mailChimpObjectContext;

        public MailChimpInstallationService(MailChimpObjectContext mailChimpObjectContext, ILocalizationService localizationService) {
            _mailChimpObjectContext = mailChimpObjectContext;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Installs this instance.
        /// </summary>
        public virtual void Install() {
            //Install string resources
            UpdateOrInstallStringResource(API_KEY_NAME, "MailChimp API Key");
            UpdateOrInstallStringResource(DEFAULT_LIST_NAME, "Default MailChimp List");
            UpdateOrInstallStringResource(AUTO_SYNC_NAME, "Use AutoSync Task");

            //Install the database tables
            _mailChimpObjectContext.Install();
        }

        /// <summary>
        /// Updates the or install string resource.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        private void UpdateOrInstallStringResource(string name, string value) {
            LocaleStringResource resource = (_localizationService.GetLocaleStringResourceByName(name) ?? new LocaleStringResource {ResourceName = name});
            resource.ResourceValue = value;

            if (resource.Id > 0) {
                _localizationService.UpdateLocaleStringResource(resource);
            }
            else {
                _localizationService.InsertLocaleStringResource(resource);
            }
        }

        /// <summary>
        /// Uninstalls this instance.
        /// </summary>
        public virtual void Uninstall() {
            //Uninstall string resources
            DeleteStringResource(API_KEY_NAME);
            DeleteStringResource(DEFAULT_LIST_NAME);
            DeleteStringResource(AUTO_SYNC_NAME);

            //Uninstall the database tables
            _mailChimpObjectContext.Uninstall();
        }

        /// <summary>
        /// Deletes the string resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        private void DeleteStringResource(string resourceName) {
            LocaleStringResource resource = _localizationService.GetLocaleStringResourceByName(resourceName);

            if (resource != null) {
                _localizationService.DeleteLocaleStringResource(resource);
            }
        }
    }
}