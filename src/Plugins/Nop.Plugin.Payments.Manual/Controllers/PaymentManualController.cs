using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Manual.Models;
using Nop.Services.Configuration;
using Nop.Services.Payments;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.Manual.Controllers
{
    public class PaymentManualController : BaseNopPaymentController
    {
        private readonly ISettingService _settingService;
        private readonly ManualPaymentSettings _manualPaymentSettings;

        public PaymentManualController(ISettingService settingService, ManualPaymentSettings manualPaymentSettings)
        {
            this._settingService = settingService;
            this._manualPaymentSettings = manualPaymentSettings;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.TransactModeId = Convert.ToInt32(_manualPaymentSettings.TransactMode);
            model.AdditionalFee = _manualPaymentSettings.AdditionalFee;
            model.TransactModeValues = _manualPaymentSettings.TransactMode.ToSelectList();
            
            return View("Nop.Plugin.Payments.Manual.Views.PaymentManual.Configure", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();
            
            //save settings
            _manualPaymentSettings.TransactMode = (TransactMode)model.TransactModeId;
            _manualPaymentSettings.AdditionalFee = model.AdditionalFee;
            _settingService.SaveSetting(_manualPaymentSettings);
            
            model.TransactModeValues = _manualPaymentSettings.TransactMode.ToSelectList();

            return View("Nop.Plugin.Payments.Manual.Views.PaymentManual.Configure", model);
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel();

            model.CreditCardTypes.Add(new SelectListItem()
                {
                    Text = "Visa",
                    Value = "Visa",
                });
            model.CreditCardTypes.Add(new SelectListItem()
            {
                Text = "Master card",
                Value = "MasterCard",
            });
            model.CreditCardTypes.Add(new SelectListItem()
            {
                Text = "Discover",
                Value = "Discover",
            });
            model.CreditCardTypes.Add(new SelectListItem()
            {
                Text = "Amex",
                Value = "Amex",
            });
            
            for (int i = 0; i < 15; i++)
            {
                string year = Convert.ToString(DateTime.Now.Year + i);
                model.ExpireYears.Add(new SelectListItem()
                {
                    Text = year,
                    Value = year,
                });
            }

            for (int i = 1; i <= 12; i++)
            {
                string text = (i < 10) ? "0" + i.ToString() : i.ToString();
                model.ExpireMonths.Add(new SelectListItem()
                {
                    Text = text,
                    Value = i.ToString(),
                });
            }

            return View("Nop.Plugin.Payments.Manual.Views.PaymentManual.PaymentInfo", model);
        }

        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnigns = new List<string>();
            //UNDONE validate form
            string creditCardType = form["creditcardtype"];
            string creditCardName = form["cardholdername"];
            string creditCardNumber = form["cardnumber"];
            string creditCardCvv2 = form["cardcode"];
            return warnigns;
        }

        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            paymentInfo.CreditCardType = form["creditcardtype"];
            paymentInfo.CreditCardName = form["cardholdername"];
            paymentInfo.CreditCardNumber = form["cardnumber"];
            paymentInfo.CreditCardExpireMonth = !String.IsNullOrEmpty(form["creditcardexpiremonth"]) ? int.Parse(form["creditcardexpiremonth"]) : 0;
            paymentInfo.CreditCardExpireYear = !String.IsNullOrEmpty(form["creditcardexpireyear"]) ? int.Parse(form["creditcardexpireyear"]) : 0;
            paymentInfo.CreditCardCvv2 = form["cardcode"];
            return paymentInfo;
        }
    }
}