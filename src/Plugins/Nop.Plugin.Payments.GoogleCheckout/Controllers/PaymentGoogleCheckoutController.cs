using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using GCheckout;
using GCheckout.Checkout;
using GCheckout.Util;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.GoogleCheckout.Models;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.GoogleCheckout.Controllers
{
    public class PaymentGoogleCheckoutController : BaseNopPaymentController
    {
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IWebHelper _webHelper;
        private readonly OrderSettings _orderSettings;
        private readonly IWorkContext _workContext;
        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly PaymentSettings _paymentSettings;

        public PaymentGoogleCheckoutController(ISettingService settingService, 
            IPaymentService paymentService, IOrderProcessingService orderProcessingService, 
            IWebHelper webHelper, OrderSettings orderSettings, IWorkContext workContext,
            CurrencySettings currencySettings, ICurrencyService currencyService,
            PaymentSettings paymentSettings)
        {
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderProcessingService = orderProcessingService;
            this._webHelper = webHelper;
            this._orderSettings = orderSettings;
            this._workContext = workContext;
            this._currencySettings = currencySettings;
            this._currencyService = currencyService;
            this._paymentSettings = paymentSettings;
        }

        private string GetSampleFileContent(string resourceName)
        {
            string fullResourceName = string.Format("Nop.Plugin.Payments.GoogleCheckout.Samples.{0}", resourceName);
            var assem = this.GetType().Assembly;
            using (var stream = assem.GetManifestResourceStream(fullResourceName))
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();

            if (AppDomain.CurrentDomain.IsFullyTrusted)
            {
                //full trust
                System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                string googleEnvironment = config.AppSettings.Settings["GoogleEnvironment"].Value;
                model.UseSandbox = googleEnvironment == "Sandbox";
                model.GoogleVendorId = config.AppSettings.Settings["GoogleMerchantID"].Value;
                model.GoogleMerchantKey = config.AppSettings.Settings["GoogleMerchantKey"].Value;
                model.AuthenticateCallback = Convert.ToBoolean(config.AppSettings.Settings["GoogleAuthenticateCallback"].Value);
            }
            else
            {
                //medium trust (can't edit)
                ModelState.AddModelError("", "Configuring Google Checkout is not allowed in medium trust. Manually update web.config file.");
            }
            return View("Nop.Plugin.Payments.GoogleCheckout.Views.PaymentGoogleCheckout.Configure", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            try
            {
                System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

                if (model.UseSandbox)
                    config.AppSettings.Settings["GoogleEnvironment"].Value = "Sandbox";
                else
                    config.AppSettings.Settings["GoogleEnvironment"].Value = "Production";
                config.AppSettings.Settings["GoogleMerchantId"].Value = model.GoogleVendorId;
                config.AppSettings.Settings["GoogleMerchantKey"].Value = model.GoogleMerchantKey;
                config.AppSettings.Settings["GoogleAuthenticateCallback"].Value = model.AuthenticateCallback.ToString();
                config.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }
            return View("Nop.Plugin.Payments.GoogleCheckout.Views.PaymentGoogleCheckout.Configure", model);
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return Content("");

            bool minOrderSubtotalAmountOk = _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);
            if (!minOrderSubtotalAmountOk)
                return Content("");

            var model = new PaymentInfoModel()
            {
                GifFileName = "checkout",
                BackgroundType = BackgroundType.Transparent,
                MerchantId = GCheckoutConfigurationHelper.MerchantID.ToString(),
                MerchantKey = GCheckoutConfigurationHelper.MerchantKey,
                Environment = GCheckoutConfigurationHelper.Environment,
                Currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode,
                UseHttps = _webHelper.IsCurrentConnectionSecured(),
            };
            return View("Nop.Plugin.Payments.GoogleCheckout.Views.PaymentGoogleCheckout.PaymentInfo", model);
        }

        public ActionResult SubmitButton()
        {
            try
            {
                //user validation
                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    return RedirectToRoute("Login");

                var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
                if (cart.Count == 0)
                    return RedirectToRoute("ShoppingCart");

                //USD for US dollars, GBP for British pounds, SEK for Swedish krona, EUR for Euro etc
                var currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

                if (String.IsNullOrEmpty(GCheckoutConfigurationHelper.MerchantID.ToString()))
                    throw new ApplicationException("GoogleMerchantID is not set");
                if (String.IsNullOrEmpty(GCheckoutConfigurationHelper.MerchantKey))
                    throw new ApplicationException("GoogleMerchantKey is not set");
                if (GCheckoutConfigurationHelper.Environment == EnvironmentType.Unknown)
                    throw new ApplicationException("GoogleEnvironment is not set");

                var cartExpirationMinutes = 0;
                var req = new CheckoutShoppingCartRequest(GCheckoutConfigurationHelper.MerchantID.ToString(),
                    GCheckoutConfigurationHelper.MerchantKey, GCheckoutConfigurationHelper.Environment,
                    currency, cartExpirationMinutes, false);

                var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.GoogleCheckout") as GoogleCheckoutPaymentProcessor;
                if (processor == null ||
                    !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                    throw new NopException("Google Checkout module cannot be loaded");
                
                var resp = processor.PostCartToGoogle(req, cart);
                if (resp.IsGood)
                {
                    return new RedirectResult(resp.RedirectUrl);
                }
                else
                {
                    return Content("Resp.RedirectUrl = " + resp.RedirectUrl + "<br />" + 
                        "Resp.IsGood = " + resp.IsGood + "<br />" + 
                        "Resp.ErrorMessage = " + Server.HtmlEncode(resp.ErrorMessage) + "<br />" + 
                        "Resp.ResponseXml = " + Server.HtmlEncode(resp.ResponseXml) + "<br />");
                }
            }
            catch (Exception exc)
            {
                return Content("Error: " + exc);
            }
        }

        [ValidateInput(false)]
        public ActionResult NotificationHandler()
        {
            string xmlData = "";
            var requestStream = Request.InputStream;
            using (var requestStreamReader = new StreamReader(requestStream))
                xmlData = requestStreamReader.ReadToEnd();

            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.GoogleCheckout") as GoogleCheckoutPaymentProcessor;
            if (processor == null ||
                !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("Google Checkout module cannot be loaded");
                
            //authorize google callback request
            if (!processor.VerifyMessageAuthentication(Request.Headers["Authorization"]))
            {
                return new HttpStatusCodeResult(401, "Access Denied");
                //return new HttpUnauthorizedResult("Access Denied");
            }

            bool debugModeEnabled = _settingService.GetSettingByKey<bool>("googlecheckoutpaymentsettings.debugmodeenabled");
            if (debugModeEnabled)
            {
                if (_webHelper.QueryString<int>("nopCommerceTestNewOrder1") > 0)
                {
                    xmlData = GetSampleFileContent("sample-neworder.txt");
                }
                if (_webHelper.QueryString<int>("nopCommerceTestNewOrder2") > 0)
                {
                    xmlData = GetSampleFileContent("sample-neworder-noShipment.txt");
                }
                else if (_webHelper.QueryString<int>("nopCommerceTestOrderChange") > 0)
                {
                    xmlData = GetSampleFileContent("sample-orderchangestate.txt");
                }
                else if (_webHelper.QueryString<int>("nopCommerceTestRisk") > 0)
                {
                    xmlData = GetSampleFileContent("sample-risk.txt");
                }
            }
            processor.ProcessCallBackRequest(xmlData);

            //ack
            string notificationAcknowledgment = processor.GetNotificationAcknowledgmentText();
            return Content(notificationAcknowledgment);
        }
        
        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }
    }
}