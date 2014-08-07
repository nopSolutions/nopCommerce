using System.Web.Mvc;
using Nop.Plugin.Shipping.CanadaPost.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Shipping.CanadaPost.Controllers
{
    [AdminAuthorize]
    public class ShippingCanadaPostController : BasePluginController
    {
        private readonly CanadaPostSettings _canadaPostSettings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public ShippingCanadaPostController(CanadaPostSettings canadaPostSettings, 
            ISettingService settingService,
            ILocalizationService localizationService)
        {
            this._canadaPostSettings = canadaPostSettings;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new CanadaPostShippingModel();
            model.Url = _canadaPostSettings.Url;
            model.Port = _canadaPostSettings.Port;
            model.CustomerId = _canadaPostSettings.CustomerId;

            return View("~/Plugins/Shipping.CanadaPost/Views/ShippingCanadaPost/Configure.cshtml", model);
        }

        [HttpPost]
        [ChildActionOnly]
        public ActionResult Configure(CanadaPostShippingModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }
            
            //save settings
            _canadaPostSettings.Url = model.Url;
            _canadaPostSettings.Port = model.Port;
            _canadaPostSettings.CustomerId = model.CustomerId;
            _settingService.SaveSetting(_canadaPostSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return View("~/Plugins/Shipping.CanadaPost/Views/ShippingCanadaPost/Configure.cshtml", model);
        }

    }
}
