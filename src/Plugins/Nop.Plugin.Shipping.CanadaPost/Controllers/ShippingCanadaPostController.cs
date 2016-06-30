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
        #region Fields

        private readonly CanadaPostSettings _canadaPostSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public ShippingCanadaPostController(CanadaPostSettings canadaPostSettings,
            ILocalizationService localizationService,
            ISettingService settingService)
        {
            this._canadaPostSettings = canadaPostSettings;
            this._localizationService = localizationService;
            this._settingService = settingService;            
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new CanadaPostShippingModel
            {
                CustomerNumber = _canadaPostSettings.CustomerNumber,
                ApiKey = _canadaPostSettings.ApiKey,
                UseSandbox = _canadaPostSettings.UseSandbox
            };

            return View("~/Plugins/Shipping.CanadaPost/Views/ShippingCanadaPost/Configure.cshtml", model);
        }

        [HttpPost]
        [ChildActionOnly]
        public ActionResult Configure(CanadaPostShippingModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //Canada Post page provides the API key with extra spaces
            model.ApiKey = model.ApiKey.Replace(" : ", ":");

            //save settings
            _canadaPostSettings.CustomerNumber = model.CustomerNumber;
            _canadaPostSettings.ApiKey = model.ApiKey;
            _canadaPostSettings.UseSandbox = model.UseSandbox;
            _settingService.SaveSetting(_canadaPostSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return View("~/Plugins/Shipping.CanadaPost/Views/ShippingCanadaPost/Configure.cshtml", model);
        }

        #endregion
    }
}
