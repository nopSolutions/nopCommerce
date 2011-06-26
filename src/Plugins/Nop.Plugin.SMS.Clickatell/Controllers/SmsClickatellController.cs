using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.SMS.Clickatell;
using Nop.Plugin.Sms.Clickatell.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Sms.Clickatell.Controllers
{
    [AdminAuthorize]
    public class SmsClickatellController : Controller
    {
        private readonly ClickatellSettings _clickatellSettings;
        private readonly ISettingService _settingService;
        private readonly ISmsService _smsService;
        private readonly ILocalizationService _localizationService;

        public SmsClickatellController(ClickatellSettings clickatellSettings,
            ISettingService settingService, ISmsService smsService,
            ILocalizationService localizationService)
        {
            this._clickatellSettings = clickatellSettings;
            this._settingService = settingService;
            this._smsService = smsService;
            this._localizationService = localizationService;
        }

        public ActionResult Configure()
        {
            var model = new SmsClickatellModel();
            model.PhoneNumber = _clickatellSettings.PhoneNumber;
            model.ApiId = _clickatellSettings.ApiId;
            model.Username = _clickatellSettings.Username;
            model.Password = _clickatellSettings.Password;
            return View("Nop.Plugin.SMS.Clickatell.Views.SmsClickatell.Configure", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public ActionResult ConfigurePOST(SmsClickatellModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }
            
            //save settings
            _clickatellSettings.PhoneNumber = model.PhoneNumber;
            _clickatellSettings.ApiId = model.ApiId;
            _clickatellSettings.Username = model.Username;
            _clickatellSettings.Password = model.Password;
            _settingService.SaveSetting(_clickatellSettings);

            return View("Nop.Plugin.SMS.Clickatell.Views.SmsClickatell.Configure", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("test-sms")]
        public ActionResult TestSms(SmsClickatellModel model)
        {
            try
            {
                if (String.IsNullOrEmpty(model.TestMessage))
                    throw new NopException("Enter test message");

                var smsProvider = _smsService.LoadSmsProviderBySystemName("Mobile.SMS.Clickatell");

                if (!smsProvider.SendSms(model.TestMessage))
                {
                    model.TestSmsResult = _localizationService.GetResource("Plugins.Sms.Clickatell.TestFailed");
                }
                else
                {
                    model.TestSmsResult = _localizationService.GetResource("Plugins.Sms.Clickatell.TestSuccess");
                }
            }
            catch(Exception exc)
            {
                model.TestSmsResult = exc.ToString();
            }

            return View("Nop.Plugin.SMS.Clickatell.Views.SmsClickatell.Configure", model);
        }
    }
}