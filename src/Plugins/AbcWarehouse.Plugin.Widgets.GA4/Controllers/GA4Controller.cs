using System.Threading.Tasks;
using AbcWarehouse.Plugin.Widgets.GA4.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace AbcWarehouse.Plugin.Widgets.GA4.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class GA4Controller : BasePluginController
    {
        private readonly GA4Settings _settings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public GA4Controller(
            GA4Settings settings,
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
                "~/Plugins/Widgets.GA4/Views/Configure.cshtml",
                _settings.ToModel());
        }

        [HttpPost]
        public async Task<ActionResult> Configure(ConfigModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            await _settingService.SaveSettingAsync(GA4Settings.FromModel(model));

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return Configure();
        }
    }
}
