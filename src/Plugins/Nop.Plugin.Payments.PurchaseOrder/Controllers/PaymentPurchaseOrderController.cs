using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Plugin.Payments.PurchaseOrder.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.PurchaseOrder.Controllers
{
    public class PaymentPurchaseOrderController : BaseNopPaymentController
    {
        private readonly ISettingService _settingService;
        private readonly PurchaseOrderPaymentSettings _purchaseOrderPaymentSettings;

        public PaymentPurchaseOrderController(ISettingService settingService, PurchaseOrderPaymentSettings purchaseOrderPaymentSettings)
        {
            this._settingService = settingService;
            this._purchaseOrderPaymentSettings = purchaseOrderPaymentSettings;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.AdditionalFee = _purchaseOrderPaymentSettings.AdditionalFee;
            
            return View("Nop.Plugin.Payments.PurchaseOrder.Views.PaymentPurchaseOrder.Configure", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();
            
            //save settings
            _purchaseOrderPaymentSettings.AdditionalFee = model.AdditionalFee;
            _settingService.SaveSetting(_purchaseOrderPaymentSettings);
            
            return View("Nop.Plugin.Payments.PurchaseOrder.Views.PaymentPurchaseOrder.Configure", model);
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel();
            
            //set postback values
            var form = this.Request.Form;
            model.PurchaseOrderNumber = form["PurchaseOrderNumber"];
            return View("Nop.Plugin.Payments.PurchaseOrder.Views.PaymentPurchaseOrder.PaymentInfo", model);
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
            paymentInfo.PurchaseOrderNumber = form["PurchaseOrderNumber"];
            return paymentInfo;
        }
    }
}