using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using AbcWarehouse.Plugin.Widgets.UniFi.Models;

namespace AbcWarehouse.Plugin.Widgets.UniFi.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class UniFiController : BasePluginController
    {
        private readonly UniFiSettings _settings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public UniFiController(
            UniFiSettings settings,
            ISettingService settingService,
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _settings = settings;
            _settingService = settingService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        public ActionResult Configure()
        {
            return View(
                "~/Plugins/Widgets.UniFi/Views/Configure.cshtml",
                _settings.ToModel());
        }

        [HttpPost]
        public async Task<ActionResult> Configure(ConfigModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            await _settingService.SaveSettingAsync(UniFiSettings.FromModel(model));

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return Configure();
        }
    }
}
