using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Home;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class HomeController : BaseAdminController
    {
        #region Fields

        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly IHomeModelFactory _homeModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public HomeController(AdminAreaSettings adminAreaSettings,
            ICommonModelFactory commonModelFactory,
            IHomeModelFactory homeModelFactory,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService)
        {
            _adminAreaSettings = adminAreaSettings;
            _commonModelFactory = commonModelFactory;
            _homeModelFactory = homeModelFactory;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> Index()
        {
            //display a warning to a store owner if there are some error
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            {
                var warnings = await _commonModelFactory.PrepareSystemWarningModelsAsync();
                if (warnings.Any(warning => warning.Level == SystemWarningLevel.Fail ||
                                            warning.Level == SystemWarningLevel.CopyrightRemovalKey ||
                                            warning.Level == SystemWarningLevel.Warning))
                    _notificationService.WarningNotification(
                        string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.Errors"),
                        Url.Action("Warnings", "Common")),
                        //do not encode URLs
                        false);
            }

            //prepare model
            var model = await _homeModelFactory.PrepareDashboardModelAsync(new DashboardModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> NopCommerceNewsHideAdv()
        {
            _adminAreaSettings.HideAdvertisementsOnAdminArea = !_adminAreaSettings.HideAdvertisementsOnAdminArea;
            await _settingService.SaveSettingAsync(_adminAreaSettings);

            return Content("Setting changed");
        }

        #endregion
    }
}