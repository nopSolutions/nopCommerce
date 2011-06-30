using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Plugin.Payments.CashOnDelivery.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.CashOnDelivery.Controllers
{
    public class PaymentCashOnDeliveryController : BaseNopPaymentController
    {
        private readonly ISettingService _settingService;
        private readonly CashOnDeliveryPaymentSettings _cashOnDeliveryPaymentSettings;

        public PaymentCashOnDeliveryController(ISettingService settingService, CashOnDeliveryPaymentSettings cashOnDeliveryPaymentSettings)
        {
            this._settingService = settingService;
            this._cashOnDeliveryPaymentSettings = cashOnDeliveryPaymentSettings;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.DescriptionText = _cashOnDeliveryPaymentSettings.DescriptionText;
            model.AdditionalFee = _cashOnDeliveryPaymentSettings.AdditionalFee;
            
            return View("Nop.Plugin.Payments.CashOnDelivery.Views.PaymentCashOnDelivery.Configure", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();
            
            //save settings
            _cashOnDeliveryPaymentSettings.DescriptionText = model.DescriptionText;
            _cashOnDeliveryPaymentSettings.AdditionalFee = model.AdditionalFee;
            _settingService.SaveSetting(_cashOnDeliveryPaymentSettings);
            
            return View("Nop.Plugin.Payments.CashOnDelivery.Views.PaymentCashOnDelivery.Configure", model);
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel()
            {
                DescriptionText = _cashOnDeliveryPaymentSettings.DescriptionText
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