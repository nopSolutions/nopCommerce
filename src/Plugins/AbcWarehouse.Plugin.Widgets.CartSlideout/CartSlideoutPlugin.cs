using System.Collections.Generic;
using System.Threading.Tasks;
using AbcWarehouse.Plugin.Widgets.CartSlideout.Tasks;
using Nop.Core.Domain.Tasks;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Services.Tasks;
using Nop.Web.Framework.Infrastructure;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout
{
    public class CartSlideoutPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly string _taskType = $"{typeof(UpdateDeliveryOptionsTask).FullName}, {typeof(CartSlideoutPlugin).Namespace}";

        private readonly IScheduleTaskService _scheduleTaskService;

        private readonly ISettingService _settingService;

        public CartSlideoutPlugin(
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService)
        {
            _scheduleTaskService = scheduleTaskService;
            _settingService = settingService;
        }

        public bool HideInWidgetList => false;

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "CartSlideout";
        }

        public System.Threading.Tasks.Task<IList<string>> GetWidgetZonesAsync()
        {
            return System.Threading.Tasks.Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.BodyStartHtmlTagAfter });
        }

        public override async System.Threading.Tasks.Task InstallAsync()
        {
            // Need to have show cart after add to cart turned off:
            await _settingService.SetSettingAsync<bool>(
                "shoppingcartsettings.displaycartafteraddingproduct",
                false);

            await RemoveTaskAsync();
            await AddTaskAsync();

            await base.InstallAsync();
        }

        public override async System.Threading.Tasks.Task UninstallAsync()
        {
            await RemoveTaskAsync();

            await base.UninstallAsync();
        }

        private async System.Threading.Tasks.Task AddTaskAsync()
        {
            ScheduleTask task = new ScheduleTask
            {
                Name = $"Update Delivery Options",
                Seconds = 14400,
                Type = _taskType,
                Enabled = true,
                StopOnError = false,
            };

            await _scheduleTaskService.InsertTaskAsync(task);
        }

        private async System.Threading.Tasks.Task RemoveTaskAsync()
        {
            var task = await _scheduleTaskService.GetTaskByTypeAsync(_taskType);
            if (task != null)
            {
                await _scheduleTaskService.DeleteTaskAsync(task);
            }
        }
    }
}
