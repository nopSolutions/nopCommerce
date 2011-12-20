using System;
using System.Linq;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tasks;
using Nop.Core.Plugins;
using Nop.Plugin.Misc.MailChimp.Data;
using Nop.Services.Localization;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.MailChimp.Services
{
    public class MailChimpInstallationService
    {
        private const string API_KEY_NAME = "Plugin.Misc.MailChimp.ApiKey";
        private const string DEFAULT_LIST_NAME = "Plugin.Misc.MailChimp.DefaultListId";
        private const string AUTO_SYNC_NAME = "Plugin.Misc.MailChimp.AutoSync";
        private const string AUTO_SYNC_PERIOD_NAME = "Plugin.Misc.MailChimp.AutoSyncEachMinutes";
        private const string WEB_HOOK_KEY = "Plugin.Misc.MailChimp.WebHookKey";
        private const string QUEUE_ALL = "Plugin.Misc.MailChimp.QueueAll";
        private const string MANUAL_SYNC = "Plugin.Misc.MailChimp.ManualSync";
        private readonly MailChimpObjectContext _mailChimpObjectContext;
        private readonly IScheduleTaskService _scheduleTaskService;

        public MailChimpInstallationService(MailChimpObjectContext mailChimpObjectContext,
            IScheduleTaskService scheduleTaskService)
        {
            _mailChimpObjectContext = mailChimpObjectContext;
            _scheduleTaskService = scheduleTaskService;
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
            return _scheduleTaskService.GetAllTasks().Where(x => x.Type.Equals("Nop.Plugin.Misc.MailChimp.MailChimpSynchronizationTask, Nop.Plugin.Misc.MailChimp", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// Installs this instance.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        public virtual void Install(BasePlugin plugin)
        {
            //Install string resources
            plugin.AddOrUpdatePluginLocaleResource(API_KEY_NAME, "MailChimp API Key");
            plugin.AddOrUpdatePluginLocaleResource(DEFAULT_LIST_NAME, "Default MailChimp List");
            plugin.AddOrUpdatePluginLocaleResource(AUTO_SYNC_NAME, "Use AutoSync task");
            plugin.AddOrUpdatePluginLocaleResource(AUTO_SYNC_PERIOD_NAME, "AutoSync task period (minutes)");
            plugin.AddOrUpdatePluginLocaleResource(WEB_HOOK_KEY, "WebHooks Key");
            plugin.AddOrUpdatePluginLocaleResource(QUEUE_ALL, "Initial Queue");
            plugin.AddOrUpdatePluginLocaleResource(MANUAL_SYNC, "Manual Sync");

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
            //Uninstall string resources
            plugin.DeletePluginLocaleResource(API_KEY_NAME);
            plugin.DeletePluginLocaleResource(DEFAULT_LIST_NAME);
            plugin.DeletePluginLocaleResource(AUTO_SYNC_NAME);
            plugin.DeletePluginLocaleResource(AUTO_SYNC_PERIOD_NAME);
            plugin.DeletePluginLocaleResource(WEB_HOOK_KEY);
            plugin.DeletePluginLocaleResource(QUEUE_ALL);
            plugin.DeletePluginLocaleResource(MANUAL_SYNC);

            //Remove scheduled task
            var task = FindScheduledTask();
            if (task != null)
                _scheduleTaskService.DeleteTask(task);

            //Uninstall the database tables
            _mailChimpObjectContext.Uninstall();
        }
    }
}