using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Plugin.Payments.PayInStore.Models;
using Nop.Services.Configuration;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.PayInStore.Controllers
{
    public class PaymentPayInStoreController : BaseNopPaymentController
    {
        private readonly ISettingService _settingService;
        private readonly PayInStorePaymentSettings _payInStorePaymentSettings;

        public PaymentPayInStoreController(ISettingService settingService, PayInStorePaymentSettings payInStorePaymentSettings)
        {
            this._settingService = settingService;
            this._payInStorePaymentSettings = payInStorePaymentSettings;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.DescriptionText = _payInStorePaymentSettings.DescriptionText;
            model.AdditionalFee = _payInStorePaymentSettings.AdditionalFee;
            
            return View("Nop.Plugin.Payments.PayInStore.Views.PaymentPayInStore.Configure", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();
            
            //save settings
            _payInStorePaymentSettings.DescriptionText = model.DescriptionText;
            _payInStorePaymentSettings.AdditionalFee = model.AdditionalFee;
            _settingService.SaveSetting(_payInStorePaymentSettings);
            
            return View("Nop.Plugin.Payments.PayInStore.Views.PaymentPayInStore.Configure", model);
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel()
            {
                DescriptionText = _payInStorePaymentSettings.DescriptionText
            };

            return View("Nop.Plugin.Payments.PayInStore.Views.PaymentPayInStore.PaymentInfo", model);
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