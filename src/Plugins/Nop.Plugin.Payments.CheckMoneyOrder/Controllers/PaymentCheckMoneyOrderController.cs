using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Plugin.Payments.CheckMoneyOrder.Models;
using Nop.Services.Configuration;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.CheckMoneyOrder.Controllers
{
    public class PaymentCheckMoneyOrderController : BaseNopPaymentController
    {
        private readonly ISettingService _settingService;
        private readonly CheckMoneyOrderPaymentSettings _checkMoneyOrderPaymentSettings;

        public PaymentCheckMoneyOrderController(ISettingService settingService,  CheckMoneyOrderPaymentSettings checkMoneyOrderPaymentSettings)
        {
            this._settingService = settingService;
            this._checkMoneyOrderPaymentSettings = checkMoneyOrderPaymentSettings;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.DescriptionText = _checkMoneyOrderPaymentSettings.DescriptionText;
            model.AdditionalFee = _checkMoneyOrderPaymentSettings.AdditionalFee;
            
            return View("Nop.Plugin.Payments.CheckMoneyOrder.Views.PaymentCheckMoneyOrder.Configure", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();
            
            //save settings
            _checkMoneyOrderPaymentSettings.DescriptionText = model.DescriptionText;
            _checkMoneyOrderPaymentSettings.AdditionalFee = model.AdditionalFee;
            _settingService.SaveSetting(_checkMoneyOrderPaymentSettings);
            
            return View("Nop.Plugin.Payments.CheckMoneyOrder.Views.PaymentCheckMoneyOrder.Configure", model);
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel()
            {
                DescriptionText = _checkMoneyOrderPaymentSettings.DescriptionText
            };

            return View("Nop.Plugin.Payments.CheckMoneyOrder.Views.PaymentCheckMoneyOrder.PaymentInfo", model);
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