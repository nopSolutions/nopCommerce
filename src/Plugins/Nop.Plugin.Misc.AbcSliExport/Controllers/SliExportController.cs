using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcSliExport.Models
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class SliExportController : BasePluginController
    {
        private readonly SliExportSettings _sliExportSettings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public SliExportController(
            SliExportSettings sliExportSettings,
            ISettingService settingService,
            ILocalizationService localizationService,
            INotificationService notificationService
        )
        {
            _sliExportSettings = sliExportSettings;
            _settingService = settingService;
            _localizationService = localizationService;
            _notificationService = notificationService;

            return;
        }

        public IActionResult Configure()
        {
            return View("~/Plugins/Misc.AbcSliExport/Views/Configure.cshtml", _sliExportSettings.ToModel());
        }

        [HttpPost]
        public async Task<IActionResult> Configure(SliExportModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            await _settingService.SaveSettingAsync(SliExportSettings.FromModel(model));

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Plugins.Saved")
            );

            return Configure();
        }
    }
}