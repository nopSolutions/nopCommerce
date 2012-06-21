using Nop.Core.Domain.Tasks;
using Nop.Core.Plugins;
using Nop.Plugin.Misc.MailChimp.Data;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.MailChimp.Services
{
    public class MailChimpInstallationService
    {
        private readonly MailChimpObjectContext _mailChimpObjectContext;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ISettingService _settingService;

        public MailChimpInstallationService(MailChimpObjectContext mailChimpObjectContext,
            IScheduleTaskService scheduleTaskService, ISettingService settingService)
        {
            this._mailChimpObjectContext = mailChimpObjectContext;
            this._scheduleTaskService = scheduleTaskService;
            this._settingService = settingService;
        }

        /// <summary>
        /// Installs the sync task.
        /// </summary>
        private void InstallSyncTask()
        {
            //Check the database for the task
            var task = FindScheduledTask();

            if (task == null)
            {
                task = new ScheduleTask
                {
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

        private ScheduleTask FindScheduledTask()
        {
            return _scheduleTaskService.GetTaskByType("Nop.Plugin.Misc.MailChimp.MailChimpSynchronizationTask, Nop.Plugin.Misc.MailChimp");
        }

        /// <summary>
        /// Installs this instance.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        public virtual void Install(BasePlugin plugin)
        {
            //settings
            var settings = new MailChimpSettings()
            {
                ApiKey = "",
                DefaultListId = "",
                WebHookKey = "",
            };
            _settingService.SaveSetting(settings);


            //locales
            plugin.AddOrUpdatePluginLocaleResource("Plugin.Misc.MailChimp.ApiKey", "MailChimp API Key");
            plugin.AddOrUpdatePluginLocaleResource("Plugin.Misc.MailChimp.DefaultListId", "Default MailChimp List");
            plugin.AddOrUpdatePluginLocaleResource("Plugin.Misc.MailChimp.AutoSync", "Use AutoSync task");
            plugin.AddOrUpdatePluginLocaleResource("Plugin.Misc.MailChimp.AutoSyncEachMinutes", "AutoSync task period (minutes)");
            plugin.AddOrUpdatePluginLocaleResource("Plugin.Misc.MailChimp.AutoSyncRestart", "If sync task period has been changed, please restart the application");
            plugin.AddOrUpdatePluginLocaleResource("Plugin.Misc.MailChimp.WebHookKey", "WebHooks Key");
            plugin.AddOrUpdatePluginLocaleResource("Plugin.Misc.MailChimp.QueueAll", "Initial Queue");
            plugin.AddOrUpdatePluginLocaleResource("Plugin.Misc.MailChimp.QueueAll.Hint", "Queue existing newsletter subscribers (run only once)");
            plugin.AddOrUpdatePluginLocaleResource("Plugin.Misc.MailChimp.ManualSync", "Manual Sync");
            plugin.AddOrUpdatePluginLocaleResource("Plugin.Misc.MailChimp.ManualSync.Hint", "Manually synchronize nopCommerce newsletter subscribers with MailChimp database");

            //Install sync task
            InstallSyncTask();

            //Install the database tables
            _mailChimpObjectContext.Install();
        }

        /// <summary>
        /// Uninstalls this instance.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        public virtual void Uninstall(BasePlugin plugin)
        {
            //settings
            _settingService.DeleteSetting<MailChimpSettings>();

            //locales
            plugin.DeletePluginLocaleResource("Plugin.Misc.MailChimp.ApiKey");
            plugin.DeletePluginLocaleResource("Plugin.Misc.MailChimp.DefaultListId");
            plugin.DeletePluginLocaleResource("Plugin.Misc.MailChimp.AutoSync");
            plugin.DeletePluginLocaleResource("Plugin.Misc.MailChimp.AutoSyncEachMinutes");
            plugin.DeletePluginLocaleResource("Plugin.Misc.MailChimp.AutoSyncRestart");
            plugin.DeletePluginLocaleResource("Plugin.Misc.MailChimp.WebHookKey");
            plugin.DeletePluginLocaleResource("Plugin.Misc.MailChimp.QueueAll");
            plugin.DeletePluginLocaleResource("Plugin.Misc.MailChimp.QueueAll.Hint");
            plugin.DeletePluginLocaleResource("Plugin.Misc.MailChimp.ManualSync");
            plugin.DeletePluginLocaleResource("Plugin.Misc.MailChimp.ManualSync.Hint");

            //Remove scheduled task
            var task = FindScheduledTask();
            if (task != null)
                _scheduleTaskService.DeleteTask(task);

            //Uninstall the database tables
            _mailChimpObjectContext.Uninstall();
        }
    }
}