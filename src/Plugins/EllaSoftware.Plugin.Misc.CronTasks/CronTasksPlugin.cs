using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Tasks;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using Task = System.Threading.Tasks.Task; 

namespace EllaSoftware.Plugin.Misc.CronTasks
{
    public class CronTasksPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly WidgetSettings _widgetSettings;

        public CronTasksPlugin(
            ISettingService settingService,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            IScheduleTaskService scheduleTaskService,
            WidgetSettings widgetSettings)
        {
            _settingService = settingService;
            _webHelper = webHelper;
            _localizationService = localizationService;
            _scheduleTaskService = scheduleTaskService;
            _widgetSettings = widgetSettings;
        }

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/CronTasks/Configure";
        }

        public System.Threading.Tasks.Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { AdminWidgetZones.ScheduleTaskListButtons });
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return CronTasksDefaults.ScheduleTaskListViewComponentName;
        }

        public bool HideInWidgetList => true;

        /// <summary>
        /// Install plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            var settings = new CronTasksSettings
            {
            };
            await _settingService.SaveSettingAsync(settings);

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(CronTasksDefaults.PluginSystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(CronTasksDefaults.PluginSystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync("EllaSoftware.Plugin.Misc.CronTasks.Title", "CRON Tasks");
            await _localizationService.AddOrUpdateLocaleResourceAsync("EllaSoftware.Plugin.Misc.CronTasks.GenerateCron", "Generate CRON");
            await _localizationService.AddOrUpdateLocaleResourceAsync("EllaSoftware.Plugin.Misc.CronTasks.ScheduleTask.InvalidInterval", "It is required to set interval to 10 seconds for this task");
            await _localizationService.AddOrUpdateLocaleResourceAsync("EllaSoftware.Plugin.Misc.CronTasks.ScheduleTask.RemoveCronTaskBeforeEnable", "This " +
                "schedule task is operated by \"Cron Tasks\" plugin. Please remove it from cron tasks before enabling this task here");
            await _localizationService.AddOrUpdateLocaleResourceAsync("EllaSoftware.Plugin.Misc.CronTasks.Settings.Title", "CRON Tasks - Settings");
            await _localizationService.AddOrUpdateLocaleResourceAsync("EllaSoftware.Plugin.Misc.CronTasks.CronTask.CronExpression", "CRON Expression (UTC)");
            await _localizationService.AddOrUpdateLocaleResourceAsync("EllaSoftware.Plugin.Misc.CronTasks.CronTask.CronExpression.Hint", "CRON Expression. Please check the CRON format rules on \"CRON Format\" tab. CRON expression should be set in UTC");
            await _localizationService.AddOrUpdateLocaleResourceAsync("EllaSoftware.Plugin.Misc.CronTasks.CronTask.CronExpression.Required", "CRON Expression cannot be empty");
            await _localizationService.AddOrUpdateLocaleResourceAsync("EllaSoftware.Plugin.Misc.CronTasks.CronTask.CronExpression.Invalid", "Cannot parse CRON Expression. Please check the CRON format rules on \"CRON Format\" tab");
            await _localizationService.AddOrUpdateLocaleResourceAsync("EllaSoftware.Plugin.Misc.CronTasks.CronTask.CronNextOccurrence", "Next Occurrence");
            await _localizationService.AddOrUpdateLocaleResourceAsync("EllaSoftware.Plugin.Misc.CronTasks.CronTask.ScheduleTaskId", "Schedule Task");
            await _localizationService.AddOrUpdateLocaleResourceAsync("EllaSoftware.Plugin.Misc.CronTasks.CronTask.ScheduleTaskId.Hint", "NopCommerce Schedule Task");
            await _localizationService.AddOrUpdateLocaleResourceAsync("EllaSoftware.Plugin.Misc.CronTasks.CronTask.ScheduleTaskId.Required", "Please select a schedule task");

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<CronTasksSettings>();

            await base.UninstallAsync();
        }

        public Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            return Task.Run(() => 
            {
                var pluginRootNode = rootNode.ChildNodes.FirstOrDefault(n => n.SystemName == "EllaSoftware");
                if (pluginRootNode == null)
                {
                    pluginRootNode = new SiteMapNode()
                    {
                        SystemName = "EllaSoftware",
                        Title = "Ella Software",
                        Visible = true,
                        IconClass = "fa-plug"
                    };
                    rootNode.ChildNodes.Add(pluginRootNode);
                }

                var pluginNode = new SiteMapNode()
                {
                    SystemName = "CRONTasks",
                    Title = "CRON Tasks",
                    Visible = true,
                    IconClass = "fa-dot-circle-o",
                    ControllerName = "CronTasks",
                    ActionName = "Configure",
                    RouteValues = new RouteValueDictionary { { "area", "Admin" } },
                };
                pluginRootNode.ChildNodes.Add(pluginNode);
            });
        }
    }
}
