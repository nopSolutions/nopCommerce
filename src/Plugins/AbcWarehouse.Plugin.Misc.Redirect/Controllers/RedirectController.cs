using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AbcWarehouse.Plugin.Misc.Redirect.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace AbcWarehouse.Plugin.Misc.Redirect.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class RedirectController : BasePluginController
    {
        private readonly RedirectSettings _settings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public RedirectController(
            RedirectSettings settings,
            ISettingService settingService,
            ILocalizationService localizationService,
            INotificationService notificationService
        ) {
            _settings = settings;
            _settingService = settingService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        public ActionResult Configure()
        {
            return View(
                "~/Plugins/Misc.Redirect/Views/Configure.cshtml",
                _settings.ToModel());
        }

        [HttpPost]
        public async Task<ActionResult> Configure(ConfigModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            await _settingService.SaveSettingAsync(RedirectSettings.FromModel(model));

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return Configure();
        }

    }
}
