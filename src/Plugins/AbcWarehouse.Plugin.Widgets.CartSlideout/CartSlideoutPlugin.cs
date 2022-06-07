using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbcWarehouse.Plugin.Widgets.CartSlideout.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Tasks;
using Nop.Plugin.Misc.AbcCore.Delivery;
using Nop.Services.Catalog;
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

        private readonly IProductAttributeService _productAttributeService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ISettingService _settingService;

        public CartSlideoutPlugin(
            IProductAttributeService productAttributeService,
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService)
        {
            _productAttributeService = productAttributeService;
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

            await RemoveProductAttributesAsync();
            await AddProductAttributesAsync();

            await base.InstallAsync();
        }

        public override async System.Threading.Tasks.Task UninstallAsync()
        {
            await RemoveTaskAsync();
            await RemoveProductAttributesAsync();

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

        private async System.Threading.Tasks.Task AddProductAttributesAsync()
        {
            var pas = new ProductAttribute[]
            {
                new ProductAttribute() { Name = AbcDeliveryConsts.DeliveryPickupOptionsProductAttributeName },
                new ProductAttribute() { Name = AbcDeliveryConsts.HaulAwayDeliveryProductAttributeName },
                new ProductAttribute() { Name = AbcDeliveryConsts.HaulAwayDeliveryInstallProductAttributeName },
            };

            foreach (var pa in pas)
            {
                await _productAttributeService.InsertProductAttributeAsync(pa);
            }
        }

        private async System.Threading.Tasks.Task RemoveProductAttributesAsync()
        {
            var attributes = (await _productAttributeService.GetAllProductAttributesAsync()).Where(pa =>
                pa.Name == AbcDeliveryConsts.DeliveryPickupOptionsProductAttributeName ||
                pa.Name == AbcDeliveryConsts.HaulAwayDeliveryProductAttributeName ||
                pa.Name == AbcDeliveryConsts.HaulAwayDeliveryInstallProductAttributeName);

            foreach (var attribute in attributes)
            {
                await _productAttributeService.DeleteProductAttributeAsync(attribute);
            }
        }
    }
}
