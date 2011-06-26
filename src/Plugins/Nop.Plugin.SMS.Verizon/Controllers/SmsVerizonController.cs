using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.SMS.Verizon;
using Nop.Plugin.Sms.Verizon.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Sms.Verizon.Controllers
{
    [AdminAuthorize]
    public class SmsVerizonController : Controller
    {
        private readonly VerizonSettings _verizonSettings;
        private readonly ISettingService _settingService;
        private readonly ISmsService _smsService;
        private readonly ILocalizationService _localizationService;

        public SmsVerizonController(VerizonSettings verizonSettings,
            ISettingService settingService, ISmsService smsService,
            ILocalizationService localizationService)
        {
            this._verizonSettings = verizonSettings;
            this._settingService = settingService;
            this._smsService = smsService;
            this._localizationService = localizationService;
        }

        public ActionResult Configure()
        {
            var model = new SmsVerizonModel();
            model.Email = _verizonSettings.Email;
            return View("Nop.Plugin.SMS.Verizon.Views.SmsVerizon.Configure", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public ActionResult ConfigurePOST(SmsVerizonModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }
            
            //save settings
            _verizonSettings.Email = model.Email;
            _settingService.SaveSetting(_verizonSettings);

            return View("Nop.Plugin.SMS.Verizon.Views.SmsVerizon.Configure", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("test-sms")]
        public ActionResult TestSms(SmsVerizonModel model)
        {
            try
            {
                if (String.IsNullOrEmpty(model.TestMessage))
                    throw new NopException("Enter test message");

                var smsProvider = _smsService.LoadSmsProviderBySystemName("Mobile.SMS.Verizon");

                if (!smsProvider.SendSms(model.TestMessage))
                {
                    model.TestSmsResult = _localizationService.GetResource("Plugins.Sms.Verizon.Test.Failed");
                }
                else
                {
                    model.TestSmsResult = _localizationService.GetResource("Plugins.Sms.Verizon.Test.Success");
                }
            }
            catch(Exception exc)
            {
                model.TestSmsResult = exc.ToString();
            }

            return View("Nop.Plugin.SMS.Verizon.Views.SmsVerizon.Configure", model);
        }
    }
}