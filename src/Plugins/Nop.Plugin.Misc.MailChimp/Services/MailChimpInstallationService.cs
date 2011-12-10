using System;
using System.Linq;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tasks;
using Nop.Plugin.Misc.MailChimp.Data;
using Nop.Services.Localization;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.MailChimp.Services {
    public class MailChimpInstallationService {
        private const string API_KEY_NAME = "Nop.Plugin.Misc.MailChimp.ApiKey";
        private const string DEFAULT_LIST_NAME = "Nop.Plugin.Misc.MailChimp.DefaultListId";
        private const string AUTO_SYNC_NAME = "Nop.Plugin.Misc.MailChimp.AutoSync";
        private const string WEB_HOOK_KEY = "Nop.Plugin.Misc.MailChimp.WebHookKey";
        private const string QUEUE_ALL = "Nop.Plugin.Misc.MailChimp.QueueAll";
        private const string MANUAL_SYNC = "Nop.Plugin.Misc.MailChimp.ManualSync";

        private readonly ILocalizationService _localizationService;
        private readonly MailChimpObjectContext _mailChimpObjectContext;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ILanguageService _languageService;

        public MailChimpInstallationService(MailChimpObjectContext mailChimpObjectContext, ILocalizationService localizationService, IScheduleTaskService scheduleTaskService, ILanguageService languageService) {
            _mailChimpObjectContext = mailChimpObjectContext;
            _localizationService = localizationService;
            _scheduleTaskService = scheduleTaskService;
            _languageService = languageService;
        }

        /// <summary>
        /// Installs this instance.
        /// </summary>
        public virtual void Install() {
            //Install string resources
            UpdateOrInstallStringResource(API_KEY_NAME, "MailChimp API Key");
            UpdateOrInstallStringResource(DEFAULT_LIST_NAME, "Default MailChimp List");
            UpdateOrInstallStringResource(AUTO_SYNC_NAME, "Use AutoSync Task");
            UpdateOrInstallStringResource(WEB_HOOK_KEY, "WebHooks Key");
            UpdateOrInstallStringResource(QUEUE_ALL, "Initial Queue");
            UpdateOrInstallStringResource(MANUAL_SYNC, "Manual Sync");

            //Install sync task
            InstallSyncTask();

            //Install the database tables
            _mailChimpObjectContext.Install();
        }

        /// <summary>
        /// Installs the sync task.
        /// </summary>
        private void InstallSyncTask() {
            //Check the database for the task
            ScheduleTask task = FindScheduledTask();

            if (task == null) {
                task = new ScheduleTask {
                        Name = "MailChimp sync",
                        //each 60 minutes
                        Seconds = 3600,
                        Type = "Nop.Plugin.Misc.MailChimp.MailChimpSynchronizationTask, Nop.Plugin.Misc.MailChimp",
                        Enabled = false,
                        StopOnError = false,
                };
                _scheduleTaskService.InsertTask(task);
            }
        }

        private ScheduleTask FindScheduledTask() {
            return _scheduleTaskService.GetAllTasks().Where(x => x.Type.Equals("Nop.Plugin.Misc.MailChimp.MailChimpSynchronizationTask, Nop.Plugin.Misc.MailChimp", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// Updates the or install string resource.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        private void UpdateOrInstallStringResource(string name, string value) {
            Language languageById = _languageService.GetLanguageById(1);
            LocaleStringResource resource = (_localizationService.GetLocaleStringResourceByName(name) ?? new LocaleStringResource {ResourceName = name, Language = languageById });
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

            //Remove scheduled task
            ScheduleTask task = FindScheduledTask();
            _scheduleTaskService.DeleteTask(task);

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