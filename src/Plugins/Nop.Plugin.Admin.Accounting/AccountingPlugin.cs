using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Admin.Accounting
{
    public class AccountingPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IScheduleTaskService _scheduleTaskService;

        public AccountingPlugin(IWebHelper webHelper, ISettingService settingService, ILocalizationService localizationService, IPermissionService permissionService, IScheduleTaskService scheduleTaskService)
        {
            _webHelper = webHelper;
            _settingService = settingService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _scheduleTaskService = scheduleTaskService;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>        
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Accounting/Configure";
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                SystemName = "Admin.Accounting",
                Title =  await _localizationService.GetResourceAsync("Nop.Plugin.Admin.Accounting.MenuItemTitle"),
                ControllerName = "",
                ActionName = "",
                Visible = true,
                IconClass = "fas fa-list-alt",
                RouteValues = new RouteValueDictionary() { { "area", null } }
            };

            rootNode.ChildNodes.Add(menuItem);

            menuItem = new SiteMapNode()
            {
                SystemName = "Admin.Accounting",
                Title =  await _localizationService.GetResourceAsync("Nop.Plugin.Admin.Accounting.Reconciliation"),
                ControllerName = "Reconciliation",
                ActionName = "List",
                Visible = true,
                IconClass = "nav-icon far fa-dot-circle",
                Url = $"{_webHelper.GetStoreLocation()}Admin/Accounting/List.cshtml"
            };

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Admin.Accounting");
            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync("Nop.Plugin.Admin.Accounting.MenuItemTitle", "Accounting");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Nop.Plugin.Admin.Accounting.Reconciliation", "Reconciliation");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Nop.Plugin.Admin.Accounting.ErrorMessage", "Error message");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Nop.Plugin.Admin.Accounting.ReconciliationListDaysBack", "Reconciliation days back");


            // Add danish values
            string dkCode = "da-DK";
            await _localizationService.AddOrUpdateLocaleResourceAsync("Nop.Plugin.Admin.Accounting.MenuItemTitle", "Bogholderi", dkCode);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Nop.Plugin.Admin.Accounting.Reconciliation", "Kasselukning", dkCode);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Nop.Plugin.Admin.Accounting.ErrorMessage", "Fejlbesked", dkCode);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Nop.Plugin.Admin.Accounting.ReconciliationListDaysBack", "Antal dage der vises i listen", dkCode);


            //install synchronization task
            if (await _scheduleTaskService.GetTaskByTypeAsync("Nop.Plugin.Admin.Accounting.Services.BookingSchedule") == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
                {
                    Enabled = true,
                    Seconds = 4 * 60 * 60,
                    Name = "Synchronization e-conomic",
                    Type = "Nop.Plugin.Admin.Accounting.Services.BookingSchedule"
                });
            }

            await base.InstallAsync();
        }
    }
}