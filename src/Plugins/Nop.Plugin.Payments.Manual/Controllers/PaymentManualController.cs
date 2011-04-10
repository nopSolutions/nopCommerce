using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Manual.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.Manual.Controllers
{
    [AdminAuthorize]
    public class PaymentManualController : Controller
    {
        private readonly ISettingService _settingService;
        private readonly ManualPaymentSettings _manualPaymentSettings;

        public PaymentManualController(ISettingService settingService, ManualPaymentSettings manualPaymentSettings)
        {
            this._settingService = settingService;
            this._manualPaymentSettings = manualPaymentSettings;
        }

        public ActionResult Configure()
        {
            var model = new PaymentManualModel();
            model.TransactModeId = Convert.ToInt32(_manualPaymentSettings.TransactMode);
            model.AdditionalFee = _manualPaymentSettings.AdditionalFee;
            model.TransactModeValues = _manualPaymentSettings.TransactMode.ToSelectList();
            
            return View("Nop.Plugin.Payments.Manual.Views.PaymentManual.Configure", model);
        }

        [HttpPost]
        public ActionResult Configure(PaymentManualModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Configure");
            }
            
            //save settings
            _manualPaymentSettings.TransactMode = (TransactMode)model.TransactModeId;
            _manualPaymentSettings.AdditionalFee = model.AdditionalFee;
            _settingService.SaveSetting(_manualPaymentSettings);
            
            model.TransactModeValues = _manualPaymentSettings.TransactMode.ToSelectList();

            return View("Nop.Plugin.Payments.Manual.Views.PaymentManual.Configure", model);
        }

    }
}