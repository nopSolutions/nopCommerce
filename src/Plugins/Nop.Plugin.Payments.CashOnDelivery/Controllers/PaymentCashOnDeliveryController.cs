using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.CashOnDelivery.Models;
using Nop.Services.Configuration;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.CashOnDelivery.Controllers
{
    public class PaymentCashOnDeliveryController : BaseNopPaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public PaymentCashOnDeliveryController(IWorkContext workContext,
            IStoreService storeService, 
            ISettingService settingService,
            IStoreContext storeContext)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._storeContext = storeContext;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var cashOnDeliveryPaymentSettings = _settingService.LoadSetting<CashOnDeliveryPaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            model.DescriptionText = cashOnDeliveryPaymentSettings.DescriptionText;
            model.AdditionalFee = cashOnDeliveryPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = cashOnDeliveryPaymentSettings.AdditionalFeePercentage;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.DescriptionText_OverrideForStore = _settingService.SettingExists(cashOnDeliveryPaymentSettings, x => x.DescriptionText, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(cashOnDeliveryPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(cashOnDeliveryPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
            }

            return View("Nop.Plugin.Payments.CashOnDelivery.Views.PaymentCashOnDelivery.Configure", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var cashOnDeliveryPaymentSettings = _settingService.LoadSetting<CashOnDeliveryPaymentSettings>(storeScope);

            //save settings
            cashOnDeliveryPaymentSettings.DescriptionText = model.DescriptionText;
            cashOnDeliveryPaymentSettings.AdditionalFee = model.AdditionalFee;
            cashOnDeliveryPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.DescriptionText_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(cashOnDeliveryPaymentSettings, x => x.DescriptionText, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(cashOnDeliveryPaymentSettings, x => x.DescriptionText, storeScope);

            if (model.AdditionalFee_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(cashOnDeliveryPaymentSettings, x => x.AdditionalFee, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(cashOnDeliveryPaymentSettings, x => x.AdditionalFee, storeScope);

            if (model.AdditionalFeePercentage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(cashOnDeliveryPaymentSettings, x => x.AdditionalFeePercentage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(cashOnDeliveryPaymentSettings, x => x.AdditionalFeePercentage, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var cashOnDeliveryPaymentSettings = _settingService.LoadSetting<CashOnDeliveryPaymentSettings>(_storeContext.CurrentStore.Id);

            var model = new PaymentInfoModel()
            {
                DescriptionText = cashOnDeliveryPaymentSettings.DescriptionText
            };

            return View("Nop.Plugin.Payments.CashOnDelivery.Views.PaymentCashOnDelivery.PaymentInfo", model);
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }
    }
}