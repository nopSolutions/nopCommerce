using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Shipping.CanadaPost.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Plugin.Shipping.CanadaPost.Controllers
{
    [AuthorizeAdmin]
    [Area("Admin")]
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

        public IActionResult Configure()
        {
            var model = new CanadaPostShippingModel()
            {
                CustomerNumber = _canadaPostSettings.CustomerNumber,
                ContractId = _canadaPostSettings.ContractId,
                ApiKey = _canadaPostSettings.ApiKey,
                UseSandbox = _canadaPostSettings.UseSandbox
            };

            return View("~/Plugins/Shipping.CanadaPost/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(CanadaPostShippingModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //Canada Post page provides the API key with extra spaces
            model.ApiKey = model.ApiKey.Replace(" : ", ":");

            //save settings
            _canadaPostSettings.CustomerNumber = model.CustomerNumber;
            _canadaPostSettings.ContractId = model.ContractId;
            _canadaPostSettings.ApiKey = model.ApiKey;
            _canadaPostSettings.UseSandbox = model.UseSandbox;
            _settingService.SaveSetting(_canadaPostSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}
