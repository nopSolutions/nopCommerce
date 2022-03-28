using Microsoft.AspNetCore.Mvc;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Localization;
using Nop.Services.Configuration;
using Nop.Web.Framework;
using Nop.Plugin.Widgets.AbcPromos.Models;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcPromos.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class AbcPromosController : BasePluginController
    {
        private readonly AbcPromosSettings _settings;

        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

        public AbcPromosController(
            AbcPromosSettings settings,
            INotificationService notificationService,
            ILocalizationService localizationService,
            ISettingService settingService
        )
        {
            _settings = settings;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _settingService = settingService;
        }

        public IActionResult Configure()
        {
            return View(
                "~/Plugins/Widgets.AbcPromos/Views/Configure.cshtml",
                _settings.ToModel()
            );
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigModel model)
        {
            await _settingService.SaveSettingAsync(AbcPromosSettings.FromModel(model));
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return Configure();
        }
    }
}


