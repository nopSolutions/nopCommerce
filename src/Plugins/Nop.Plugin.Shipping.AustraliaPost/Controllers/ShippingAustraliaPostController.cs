using System.Web.Mvc;
using Nop.Plugin.Shipping.AustraliaPost.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Shipping.AustraliaPost.Controllers
{
    [AdminAuthorize]
    public class ShippingAustraliaPostController : Controller
    {
        private readonly AustraliaPostSettings _australiaPostSettings;
        private readonly ISettingService _settingService;

        public ShippingAustraliaPostController(AustraliaPostSettings australiaPostSettings, ISettingService settingService)
        {
            this._australiaPostSettings = australiaPostSettings;
            this._settingService = settingService;
        }

        public ActionResult Configure()
        {
            var model = new AustraliaPostShippingModel();
            model.GatewayUrl = _australiaPostSettings.GatewayUrl;
            model.ShippedFromZipPostalCode = _australiaPostSettings.ShippedFromZipPostalCode;
            model.AdditionalHandlingCharge = _australiaPostSettings.AdditionalHandlingCharge;
            return View("Nop.Plugin.Shipping.AustraliaPost.Views.ShippingAustraliaPost.Configure", model);
        }

        [HttpPost]
        public ActionResult Configure(AustraliaPostShippingModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }
            
            //save settings
            _australiaPostSettings.GatewayUrl = model.GatewayUrl;
            _australiaPostSettings.ShippedFromZipPostalCode = model.ShippedFromZipPostalCode;
            _australiaPostSettings.AdditionalHandlingCharge = model.AdditionalHandlingCharge;
            _settingService.SaveSetting(_australiaPostSettings);

            return View("Nop.Plugin.Shipping.AustraliaPost.Views.ShippingAustraliaPost.Configure", model);
        }

    }
}
