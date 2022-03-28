using Nop.Core;
using Nop.Core.Domain.Tasks;
using Nop.Plugin.Widgets.AbcBonusBundle.Tasks;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Services.Tasks;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;

namespace Nop.Plugin.Widgets.AbcBonusBundle
{
    public class AbcBonusBundlePlugin : BasePlugin, IWidgetPlugin
    {
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IWebHelper _webHelper;

        private readonly string TaskType = $"{typeof(BonusBundleProductRibbonUpdateTask).FullName}, {typeof(AbcBonusBundlePlugin).Namespace}";

        public AbcBonusBundlePlugin(
            IScheduleTaskService scheduleTaskService,
            IWebHelper webHelper
        )
        {
            _scheduleTaskService = scheduleTaskService;
            _webHelper = webHelper;
        }

        public bool HideInWidgetList => false;

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AbcBonusBundle/Configure";
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "AbcBonusBundle";
        }

        public System.Threading.Tasks.Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { "productdetails_overview_bottom" });
        }

        public override async Task InstallAsync()
        {
            await RemoveTaskAsync();
            await AddTaskAsync();

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await RemoveTaskAsync();

            await base.UninstallAsync();
        }

        private async Task AddTaskAsync()
        {
            ScheduleTask task = new ScheduleTask
            {
                Name = $"Sync ABC Bonus Bundles Product Ribbons",
                Seconds = 14400,
                Type = TaskType,
                Enabled = true,
                StopOnError = false
            };

            await _scheduleTaskService.InsertTaskAsync(task);
        }

        private async Task RemoveTaskAsync()
        {
            var task = await _scheduleTaskService.GetTaskByTypeAsync(TaskType);
            if (task != null)
            {
                await _scheduleTaskService.DeleteTaskAsync(task);
            }
        }
    }
}
