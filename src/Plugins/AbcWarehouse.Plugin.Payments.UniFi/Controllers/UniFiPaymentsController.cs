using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Core;
using AbcWarehouse.Plugin.Payments.UniFi.Models;

namespace AbcWarehouse.Plugin.Payments.UniFi.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class UniFiPaymentsController : BasePluginController
    {
        private readonly UniFiPaymentsSettings _settings;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;

        public UniFiPaymentsController(
            UniFiPaymentsSettings settings,
            ILocalizationService localizationService,
            INotificationService notificationService,
            ISettingService settingService)
        {
            _settings = settings;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _settingService = settingService;
        }

        public async Task<ActionResult> Configure()
        {
            return View(
                "~/Plugins/Payments.UniFi/Views/Configure.cshtml",
                _settings.ToModel());
        }

        [HttpPost]
        public async Task<ActionResult> Configure(ConfigModel model)
        {
            if (!ModelState.IsValid)
            {
                return await Configure();
            }

            await _settingService.SaveSettingAsync(UniFiPaymentsSettings.FromModel(model));

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }
    }
}
