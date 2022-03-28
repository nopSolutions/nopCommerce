using Nop.Core;
using Nop.Core.Domain.Tasks;
using Nop.Plugin.Misc.EnhancedLogging.Tasks;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Services.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nop.Plugin.Misc.EnhancedLogging
{
    public class EnhancedLoggingPlugin : BasePlugin, IMiscPlugin
    {
        public static class LocaleKey
        {
            public const string Base = "Plugins.Misc.EnhancedLogging";
            public const string DaysToKeepLogs = Base + ".Fields.DaysToKeepLogs";
        }

        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        private readonly string DefaultClearLogTaskType =
            $"{typeof(ClearLogTask).Namespace}.{typeof(ClearLogTask).Name}, " +
            $"{typeof(ClearLogTask).Assembly.GetName().Name}";
        private readonly string EnhancedClearLogTaskType =
            $"{typeof(EnhancedClearLogTask).Namespace}.{typeof(EnhancedClearLogTask).Name}, " +
            $"{typeof(EnhancedClearLogTask).Assembly.GetName().Name}";

        public EnhancedLoggingPlugin(
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService,
            IWebHelper webHelper,
            ILocalizationService localizationService
        )
        {
            _scheduleTaskService = scheduleTaskService;
            _settingService = settingService;
            _webHelper = webHelper;
            _localizationService = localizationService;
        }

        public override async System.Threading.Tasks.Task InstallAsync()
        {
            await DeleteAllRelatedTasksAsync();

            var task = new ScheduleTask()
            {
                Name = "Clear log (enhanced)",
                Seconds = 3600,
                Type = EnhancedClearLogTaskType,
                Enabled = true,
                StopOnError = false
            };
            await _scheduleTaskService.InsertTaskAsync(task);

            await _settingService.SaveSettingAsync(EnhancedLoggingSettings.CreateDefault());

            await AddOrUpdatePluginLocaleResourcesAsync();

            await base.InstallAsync();
        }

        public override async System.Threading.Tasks.Task UninstallAsync()
        {
            await DeleteAllRelatedTasksAsync();

            var task = new ScheduleTask()
            {
                Name = "Clear log",
                Seconds = 3600,
                Type = DefaultClearLogTaskType,
                Enabled = false,
                StopOnError = false
            };
            await _scheduleTaskService.InsertTaskAsync(task);

            await _settingService.DeleteSettingAsync<EnhancedLoggingSettings>();

            await _localizationService.DeleteLocaleResourcesAsync(LocaleKey.Base);

            await base.UninstallAsync();
        }

        private async System.Threading.Tasks.Task DeleteAllRelatedTasksAsync()
        {
            var defaultClearLogTask = await _scheduleTaskService.GetTaskByTypeAsync(DefaultClearLogTaskType);
            if (defaultClearLogTask != null)
            {
                await _scheduleTaskService.DeleteTaskAsync(defaultClearLogTask);
            }

            var enhancedClearLogTask = await _scheduleTaskService.GetTaskByTypeAsync(EnhancedClearLogTaskType);
            if (enhancedClearLogTask != null)
            {
                await _scheduleTaskService.DeleteTaskAsync(enhancedClearLogTask);
            }

        }

        private async System.Threading.Tasks.Task AddOrUpdatePluginLocaleResourcesAsync()
        {
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                [LocaleKey.DaysToKeepLogs] = "Days to Keep Logs"
            });
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/EnhancedLogging/Configure";
        }
    }
}
