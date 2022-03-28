using Nop.Services.Cms;
using Nop.Services.Plugins;
using System.Collections.Generic;
using Nop.Web.Framework.Infrastructure;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Core.Domain.Tasks;
using Nop.Services.Tasks;
using Nop.Plugin.Widgets.AbcPromos.Tasks;
using Nop.Plugin.Misc.AbcCore.Infrastructure;
using Task = System.Threading.Tasks.Task;

namespace Nop.Plugin.Widgets.AbcPromos
{
    public class AbcPromosPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly IWebHelper _webHelper;

        private readonly ILocalizationService _localizationService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ISettingService _settingService;

        private readonly string TaskType =
            $"{typeof(UpdatePromosTask).FullName}, {typeof(AbcPromosPlugin).Namespace}";

        public AbcPromosPlugin(
            IWebHelper webHelper,
            ILocalizationService localizationService,
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService
        )
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _scheduleTaskService = scheduleTaskService;
            _settingService = settingService;
        }

        public bool HideInWidgetList => false;

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsAbcPromos";
        }

        public System.Threading.Tasks.Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> {
                PublicWidgetZones.ProductDetailsAfterBreadcrumb,
                CustomPublicWidgetZones.ProductBoxAddinfoReviews,
                CustomPublicWidgetZones.ProductDetailsAfterPrice,
                CustomPublicWidgetZones.OrderSummaryAfterProductMiniDescription
            });
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AbcPromos/Configure";
        }

        public override async Task UpdateAsync(string currentVersion, string targetVersion)
        {
            await AddTaskAsync();
            AddLocales();

            await base.UpdateAsync(currentVersion, targetVersion);
        }

        public override async Task InstallAsync()
        {
            await RemoveTaskAsync();
            await AddTaskAsync();
            AddLocales();

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await RemoveTaskAsync();
            await _localizationService.DeleteLocaleResourcesAsync(AbcPromosLocales.Base);
            await _settingService.DeleteSettingAsync<AbcPromosSettings>();

            await base.UninstallAsync();
        }

        private void AddLocales()
        {
            _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                [AbcPromosLocales.IncludeExpiredPromosOnRebatesPromosPage]
                    = "Include Expired Promos on Rebates/Promos Page",
                [AbcPromosLocales.IncludeExpiredPromosOnRebatesPromosPageHint]
                    = "Shows expired promos (by one month) on the rebates/promos page.",
            });
        }

        private async Task AddTaskAsync()
        {
            ScheduleTask task = new ScheduleTask
            {
                Name = $"Update Promos",
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
