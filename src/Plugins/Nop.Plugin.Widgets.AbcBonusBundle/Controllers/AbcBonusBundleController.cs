using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.AbcCore;
using Nop.Plugin.Widgets.AbcBonusBundle.Models;
using Nop.Plugin.Widgets.AbcBonusBundle.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcBonusBundle.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class AbcBonusBundleController : BasePluginController
    {
        private readonly ILogger _logger;
        private readonly IProductAbcBundleService _productAbcBundleService;
        private readonly IProductService _productService;
        private readonly AbcBonusBundleSettings _abcBonusBundleSettings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;



        public AbcBonusBundleController(ILogger logger,
            IProductAbcBundleService productAbcBundleService,
            IProductService productService,
            AbcBonusBundleSettings abcBonusBundleSettings,
            ISettingService settingService,
            ILocalizationService localizationService,
            INotificationService notificationService
        )
        {
            _logger = logger;
            _productAbcBundleService = productAbcBundleService;
            _productService = productService;
            _abcBonusBundleSettings = abcBonusBundleSettings;
            _settingService = settingService;
            _localizationService = localizationService;
            _notificationService = notificationService;


        }

        public IActionResult Configure()
        {
            var model = ConfigurationModel.FromSettings(_abcBonusBundleSettings);

            return View("~/Plugins/Widgets.AbcBonusBundle/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            await _settingService.SaveSettingAsync(model.ToSettings());
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return View("~/Plugins/Widgets.AbcBonusBundle/Views/Configure.cshtml", model);
        }
    }
}
