using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
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

        protected AdminAreaSettings AdminAreaSettings { get; }
        protected ICommonModelFactory CommonModelFactory { get; }
        protected IHomeModelFactory HomeModelFactory { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected ISettingService SettingService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public HomeController(AdminAreaSettings adminAreaSettings,
            ICommonModelFactory commonModelFactory,
            IHomeModelFactory homeModelFactory,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext)
        {
            AdminAreaSettings = adminAreaSettings;
            CommonModelFactory = commonModelFactory;
            HomeModelFactory = homeModelFactory;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            SettingService = settingService;
            WorkContext = workContext;
            GenericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> Index()
        {
            //display a warning to a store owner if there are some error
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var hideCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
            var closeCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

            if ((hideCard || closeCard) && await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            {
                var warnings = await CommonModelFactory.PrepareSystemWarningModelsAsync();
                if (warnings.Any(warning => warning.Level == SystemWarningLevel.Fail ||
                                            warning.Level == SystemWarningLevel.CopyrightRemovalKey ||
                                            warning.Level == SystemWarningLevel.Warning))
                    NotificationService.WarningNotification(
                        string.Format(await LocalizationService.GetResourceAsync("Admin.System.Warnings.Errors"),
                        Url.Action("Warnings", "Common")),
                        //do not encode URLs
                        false);
            }

            //progress of localozation 
            var currentLanguage = await WorkContext.GetWorkingLanguageAsync();
            var progress = await GenericAttributeService.GetAttributeAsync<string>(currentLanguage, NopCommonDefaults.LanguagePackProgressAttribute);
            if (!string.IsNullOrEmpty(progress))
            {
                var locale = await LocalizationService.GetResourceAsync("Admin.Configuration.LanguagePackProgressMessage");
                NotificationService.SuccessNotification(string.Format(locale, progress, NopLinksDefaults.OfficialSite.Translations), false);
                await GenericAttributeService.SaveAttributeAsync(currentLanguage, NopCommonDefaults.LanguagePackProgressAttribute, string.Empty);
            }

            //prepare model
            var model = await HomeModelFactory.PrepareDashboardModelAsync(new DashboardModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> NopCommerceNewsHideAdv()
        {
            AdminAreaSettings.HideAdvertisementsOnAdminArea = !AdminAreaSettings.HideAdvertisementsOnAdminArea;
            await SettingService.SaveSettingAsync(AdminAreaSettings);

            return Content("Setting changed");
        }

        #endregion
    }
}