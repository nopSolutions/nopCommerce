using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.Worldpay.Domain;
using Nop.Plugin.Payments.Worldpay.Models;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.Worldpay.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class PaymentWorldpayController : BasePaymentController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly WorldpayPaymentSettings _worldpayPaymentSettings;

        #endregion

        #region Ctor

        public PaymentWorldpayController(ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            WorldpayPaymentSettings worldpayPaymentSettings)
        {
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._worldpayPaymentSettings = worldpayPaymentSettings;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            //whether user has the authority
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prepare model
            var model = new ConfigurationModel
            {
                SecureNetId = _worldpayPaymentSettings.SecureNetId,
                SecureKey = _worldpayPaymentSettings.SecureKey,
                PublicKey = _worldpayPaymentSettings.PublicKey,
                DeveloperId = _worldpayPaymentSettings.DeveloperId,
                DeveloperVersion = _worldpayPaymentSettings.DeveloperVersion,
                TransactionModeId = (int)_worldpayPaymentSettings.TransactionMode,
                TransactionModes = _worldpayPaymentSettings.TransactionMode.ToSelectList(),
                UseSandbox = _worldpayPaymentSettings.UseSandbox,
                ValidateAddress = _worldpayPaymentSettings.ValidateAddress,
                AdditionalFee = _worldpayPaymentSettings.AdditionalFee,
                AdditionalFeePercentage = _worldpayPaymentSettings.AdditionalFeePercentage
            };

            return View("~/Plugins/Payments.Worldpay/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(ConfigurationModel model)
        {
            //whether user has the authority
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _worldpayPaymentSettings.SecureNetId = model.SecureNetId;
            _worldpayPaymentSettings.SecureKey = model.SecureKey;
            _worldpayPaymentSettings.PublicKey = model.PublicKey;
            _worldpayPaymentSettings.DeveloperId = model.DeveloperId;
            _worldpayPaymentSettings.DeveloperVersion = model.DeveloperVersion;
            _worldpayPaymentSettings.TransactionMode = (TransactionMode)model.TransactionModeId;
            _worldpayPaymentSettings.UseSandbox = model.UseSandbox;
            _worldpayPaymentSettings.ValidateAddress = model.ValidateAddress;
            _worldpayPaymentSettings.AdditionalFee = model.AdditionalFee;
            _worldpayPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            _settingService.SaveSetting(_worldpayPaymentSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}