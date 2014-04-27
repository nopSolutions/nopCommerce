using System.Web.Mvc;
using Nop.Plugin.Shipping.AustraliaPost.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Shipping.AustraliaPost.Controllers
{
    [AdminAuthorize]
    public class ShippingAustraliaPostController : BasePluginController
    {
        private readonly AustraliaPostSettings _australiaPostSettings;
        private readonly ISettingService _settingService;

        public ShippingAustraliaPostController(AustraliaPostSettings australiaPostSettings, ISettingService settingService)
        {
            this._australiaPostSettings = australiaPostSettings;
            this._settingService = settingService;
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new AustraliaPostShippingModel();
            model.GatewayUrl = _australiaPostSettings.GatewayUrl;
            model.AdditionalHandlingCharge = _australiaPostSettings.AdditionalHandlingCharge;

            return View("~/Plugins/Shipping.AustraliaPost/Views/ShippingAustraliaPost/Configure.cshtml", model);
        }

        [HttpPost]
        [ChildActionOnly]
        public ActionResult Configure(AustraliaPostShippingModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }
            
            //save settings
            _australiaPostSettings.GatewayUrl = model.GatewayUrl;
            _australiaPostSettings.AdditionalHandlingCharge = model.AdditionalHandlingCharge;
            _settingService.SaveSetting(_australiaPostSettings);

            return Configure();
        }

    }
}
