using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Shipping.AustraliaPost.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Plugin.Shipping.AustraliaPost.Controllers
{
    [AuthorizeAdmin]
    [Area("Admin")]
    public class ShippingAustraliaPostController : BasePluginController
    {
        #region Fields

        private readonly AustraliaPostSettings _australiaPostSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public ShippingAustraliaPostController(AustraliaPostSettings australiaPostSettings,
            ILocalizationService localizationService,
            ISettingService settingService)
        {
            this._australiaPostSettings = australiaPostSettings;
            this._localizationService = localizationService;
            this._settingService = settingService;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            var model = new AustraliaPostShippingModel()
            {
                ApiKey = _australiaPostSettings.ApiKey,
                AdditionalHandlingCharge = _australiaPostSettings.AdditionalHandlingCharge
            };

            return View("~/Plugins/Shipping.AustraliaPost/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(AustraliaPostShippingModel model)
        {
            if (!ModelState.IsValid)
                return Configure();
            
            //save settings
            _australiaPostSettings.ApiKey = model.ApiKey;
            _australiaPostSettings.AdditionalHandlingCharge = model.AdditionalHandlingCharge;
            _settingService.SaveSetting(_australiaPostSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}
