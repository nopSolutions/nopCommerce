using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.TestTask.Models;
using Nop.Plugin.Widgets.TestTask;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.TestTask.Controllers
{
    [Area(AreaNames.Admin)]
    public class WidgetsTestTaskController : BasePluginController
    {
        private readonly IStoreContext _storeContext;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly TestTaskSettings _testTaskSettings;

        public WidgetsTestTaskController(IStoreContext storeContext,
            IPermissionService permissionService,
            ISettingService settingService,
            ILocalizationService localizationService,
            TestTaskSettings testTaskSettings)
        {
            this._storeContext = storeContext;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._testTaskSettings = testTaskSettings;
        }


        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var testTaskSettings = _settingService.LoadSetting<TestTaskSettings>(storeScope);
            var model = new ConfigurationModel
            {
                ActiveStoreScopeConfiguration = storeScope,
                PromoMessage = testTaskSettings.PromoMessage
            };
            return View("~/Plugins/Widgets.TestTask/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _testTaskSettings.PromoMessage = model.PromoMessage;
            _settingService.SaveSetting(_testTaskSettings);

            SuccessNotification("Saved successfully.");
            return Configure();
        }
    }
}
