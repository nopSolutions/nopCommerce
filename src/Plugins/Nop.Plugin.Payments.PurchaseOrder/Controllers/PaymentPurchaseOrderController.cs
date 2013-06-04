using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.PurchaseOrder.Models;
using Nop.Services.Configuration;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.PurchaseOrder.Controllers
{
    public class PaymentPurchaseOrderController : BaseNopPaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;

        public PaymentPurchaseOrderController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var purchaseOrderPaymentSettings = _settingService.LoadSetting<PurchaseOrderPaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            model.AdditionalFee = purchaseOrderPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = purchaseOrderPaymentSettings.AdditionalFeePercentage;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(purchaseOrderPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(purchaseOrderPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
            }

            return View("Nop.Plugin.Payments.PurchaseOrder.Views.PaymentPurchaseOrder.Configure", model);
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
            var purchaseOrderPaymentSettings = _settingService.LoadSetting<PurchaseOrderPaymentSettings>(storeScope);

            //save settings
            purchaseOrderPaymentSettings.AdditionalFee = model.AdditionalFee;
            purchaseOrderPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.AdditionalFee_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(purchaseOrderPaymentSettings, x => x.AdditionalFee, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(purchaseOrderPaymentSettings, x => x.AdditionalFee, storeScope);

            if (model.AdditionalFeePercentage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(purchaseOrderPaymentSettings, x => x.AdditionalFeePercentage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(purchaseOrderPaymentSettings, x => x.AdditionalFeePercentage, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            return Configure();
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