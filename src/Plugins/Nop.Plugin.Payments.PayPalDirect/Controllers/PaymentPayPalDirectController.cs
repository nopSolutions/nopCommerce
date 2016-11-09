using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.PayPalDirect.Models;
using Nop.Plugin.Payments.PayPalDirect.Validators;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using PayPal.Api;

namespace Nop.Plugin.Payments.PayPalDirect.Controllers
{
    public class PaymentPayPalDirectController : BasePaymentController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PaymentPayPalDirectController(ILocalizationService localizationService,
            ILogger logger,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWorkContext workContext)
        {
            this._localizationService = localizationService;
            this._logger = logger;
            this._orderProcessingService = orderProcessingService;
            this._orderService = orderService;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Create webhook that receive events for the subscribed event types
        /// </summary>
        /// <returns>Webhook id</returns>
        protected string CreateWebHook()
        {
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var payPalDirectPaymentSettings = _settingService.LoadSetting<PayPalDirectPaymentSettings>(storeScope);

            try
            {
                var apiContext = PaypalHelper.GetApiContext(payPalDirectPaymentSettings);
                if (!string.IsNullOrEmpty(payPalDirectPaymentSettings.WebhookId))
                {
                    try
                    {
                        return Webhook.Get(apiContext, payPalDirectPaymentSettings.WebhookId).id;
                    }
                    catch (PayPal.PayPalException) { }
                }

                var currentStore = storeScope > 0 ? _storeService.GetStoreById(storeScope) : _storeContext.CurrentStore;
                var webhook = new Webhook
                {
                    event_types = new List<WebhookEventType> { new WebhookEventType { name = "*" } },
                    url = string.Format("{0}Plugins/PaymentPayPalDirect/Webhook", currentStore.SslEnabled ? currentStore.SecureUrl : currentStore.Url)
                }.Create(apiContext);

                return webhook.id;
            }
            catch (PayPal.PayPalException exc)
            {
                if (exc is PayPal.ConnectionException)
                {
                    var error = JsonFormatter.ConvertFromJson<Error>((exc as PayPal.ConnectionException).Response);
                    if (error != null)
                    {
                        _logger.Error(string.Format("PayPal error: {0} ({1})", error.message, error.name));
                        if (error.details != null)
                            error.details.ForEach(x => _logger.Error(string.Format("{0} {1}", x.field, x.issue)));
                    }
                    else
                        _logger.Error(exc.InnerException != null ? exc.InnerException.Message : exc.Message);
                }
                else
                    _logger.Error(exc.InnerException != null ? exc.InnerException.Message : exc.Message);

                return string.Empty;
            }
        }

        #endregion

        #region Methods

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var payPalDirectPaymentSettings = _settingService.LoadSetting<PayPalDirectPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                ClientId = payPalDirectPaymentSettings.ClientId,
                ClientSecret = payPalDirectPaymentSettings.ClientSecret,
                WebhookId = payPalDirectPaymentSettings.WebhookId,
                UseSandbox = payPalDirectPaymentSettings.UseSandbox,
                TransactModeId = (int)payPalDirectPaymentSettings.TransactMode,
                AdditionalFee = payPalDirectPaymentSettings.AdditionalFee,
                AdditionalFeePercentage = payPalDirectPaymentSettings.AdditionalFeePercentage,
                TransactModeValues = payPalDirectPaymentSettings.TransactMode.ToSelectList(),
                ActiveStoreScopeConfiguration = storeScope
            };
            if (storeScope > 0)
            {
                model.ClientId_OverrideForStore = _settingService.SettingExists(payPalDirectPaymentSettings, x => x.ClientId, storeScope);
                model.ClientSecret_OverrideForStore = _settingService.SettingExists(payPalDirectPaymentSettings, x => x.ClientSecret, storeScope);
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(payPalDirectPaymentSettings, x => x.UseSandbox, storeScope);
                model.TransactModeId_OverrideForStore = _settingService.SettingExists(payPalDirectPaymentSettings, x => x.TransactMode, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(payPalDirectPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(payPalDirectPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
            }

            return View("~/Plugins/Payments.PayPalDirect/Views/PaymentPayPalDirect/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var payPalDirectPaymentSettings = _settingService.LoadSetting<PayPalDirectPaymentSettings>(storeScope);

            //save settings
            payPalDirectPaymentSettings.ClientId = model.ClientId;
            payPalDirectPaymentSettings.ClientSecret = model.ClientSecret;
            payPalDirectPaymentSettings.WebhookId = model.WebhookId;
            payPalDirectPaymentSettings.UseSandbox = model.UseSandbox;
            payPalDirectPaymentSettings.TransactMode = (TransactMode)model.TransactModeId;
            payPalDirectPaymentSettings.AdditionalFee = model.AdditionalFee;
            payPalDirectPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(payPalDirectPaymentSettings, x => x.ClientId, model.ClientId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(payPalDirectPaymentSettings, x => x.ClientSecret, model.ClientSecret_OverrideForStore, storeScope, false);
            _settingService.SaveSetting(payPalDirectPaymentSettings, x => x.WebhookId, 0, false);
            _settingService.SaveSettingOverridablePerStore(payPalDirectPaymentSettings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(payPalDirectPaymentSettings, x => x.TransactMode, model.TransactModeId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(payPalDirectPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(payPalDirectPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("createwebhook")]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult GetWebhookId(ConfigurationModel model)
        {
            var payPalDirectPaymentSettings = _settingService.LoadSetting<PayPalDirectPaymentSettings>();
            payPalDirectPaymentSettings.WebhookId = CreateWebHook();
            _settingService.SaveSetting(payPalDirectPaymentSettings);

            if (string.IsNullOrEmpty(payPalDirectPaymentSettings.WebhookId))
                ErrorNotification(_localizationService.GetResource("Plugins.Payments.PayPalDirect.WebhookError"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel();

            model.CreditCardTypes = new List<SelectListItem>
            {
                new SelectListItem { Text = "Visa", Value = "visa" },
                new SelectListItem { Text = "Master card", Value = "MasterCard" },
                new SelectListItem { Text = "Discover", Value = "Discover" },
                new SelectListItem { Text = "Amex", Value = "Amex" },
            };

            //years
            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                model.ExpireYears.Add(new SelectListItem
                {
                    Text = year,
                    Value = year,
                });
            }

            //months
            for (var i = 1; i <= 12; i++)
            {
                model.ExpireMonths.Add(new SelectListItem
                {
                    Text = i.ToString("D2"),
                    Value = i.ToString(),
                });
            }

            //set postback values
            model.CardNumber = Request.Form["CardNumber"];
            model.CardCode = Request.Form["CardCode"];
            var selectedCcType = model.CreditCardTypes.FirstOrDefault(x => x.Value.Equals(Request.Form["CreditCardType"], StringComparison.InvariantCultureIgnoreCase));
            if (selectedCcType != null)
                selectedCcType.Selected = true;
            var selectedMonth = model.ExpireMonths.FirstOrDefault(x => x.Value.Equals(Request.Form["ExpireMonth"], StringComparison.InvariantCultureIgnoreCase));
            if (selectedMonth != null)
                selectedMonth.Selected = true;
            var selectedYear = model.ExpireYears.FirstOrDefault(x => x.Value.Equals(Request.Form["ExpireYear"], StringComparison.InvariantCultureIgnoreCase));
            if (selectedYear != null)
                selectedYear.Selected = true;

            return View("~/Plugins/Payments.PayPalDirect/Views/PaymentPayPalDirect/PaymentInfo.cshtml", model);
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();

            //validate
            var validator = new PaymentInfoValidator(_localizationService);
            var model = new PaymentInfoModel
            {
                CardNumber = form["CardNumber"],
                CardCode = form["CardCode"],
                ExpireMonth = form["ExpireMonth"],
                ExpireYear = form["ExpireYear"]
            };
            var validationResult = validator.Validate(model);
            if (!validationResult.IsValid)
                warnings.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));

            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            return new ProcessPaymentRequest
            { 
                CreditCardType = form["CreditCardType"],
                CreditCardNumber = form["CardNumber"],
                CreditCardExpireMonth = int.Parse(form["ExpireMonth"]),
                CreditCardExpireYear = int.Parse(form["ExpireYear"]),
                CreditCardCvv2 = form["CardCode"]
            };
        }

        [HttpPost]
        public ActionResult WebhookEventsHandler()
        {
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var payPalDirectPaymentSettings = _settingService.LoadSetting<PayPalDirectPaymentSettings>(storeScope);

            try
            {
                var requestBody = string.Empty;
                using (var stream = new StreamReader(Request.InputStream))
                {
                    requestBody = stream.ReadToEnd();
                }
                var apiContext = PaypalHelper.GetApiContext(payPalDirectPaymentSettings);

                //validate request
                if (!WebhookEvent.ValidateReceivedEvent(apiContext, Request.Headers, requestBody, payPalDirectPaymentSettings.WebhookId))
                {
                    _logger.Error("PayPal error: webhook event was not validated");
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }

                var webhook = JsonFormatter.ConvertFromJson<WebhookEvent>(requestBody);

                if (webhook.resource_type.ToLowerInvariant().Equals("sale"))
                {
                    var sale = JsonFormatter.ConvertFromJson<Sale>(webhook.resource.ToString());

                    //recurring payment
                    if (!string.IsNullOrEmpty(sale.billing_agreement_id))
                    {
                        //get agreement
                        var agreement = Agreement.Get(apiContext, sale.billing_agreement_id);
                        var initialOrder = _orderService.GetOrderByGuid(new Guid(agreement.description));
                        if (initialOrder != null)
                        {
                            var recurringPayment = _orderService.SearchRecurringPayments(initialOrderId: initialOrder.Id).FirstOrDefault();
                            if (recurringPayment != null)
                                if (sale.state.ToLowerInvariant().Equals("completed"))
                                {
                                    if (recurringPayment.RecurringPaymentHistory.Count == 0)
                                    {
                                        //first payment
                                        initialOrder.PaymentStatus = PaymentStatus.Paid;
                                        initialOrder.CaptureTransactionId = sale.id;
                                        _orderService.UpdateOrder(initialOrder);

                                        recurringPayment.RecurringPaymentHistory.Add(new RecurringPaymentHistory
                                        {
                                            RecurringPaymentId = recurringPayment.Id,
                                            OrderId = initialOrder.Id,
                                            CreatedOnUtc = DateTime.UtcNow
                                        });
                                        _orderService.UpdateRecurringPayment(recurringPayment);
                                    }
                                    else
                                    {
                                        //next payments
                                        var orders = _orderService.GetOrdersByIds(recurringPayment.RecurringPaymentHistory.Select(order => order.OrderId).ToArray());
                                        if (!orders.Any(order => !string.IsNullOrEmpty(order.CaptureTransactionId)
                                            && order.CaptureTransactionId.Equals(sale.id, StringComparison.InvariantCultureIgnoreCase)))
                                        {
                                            var processPaymentResult = new ProcessPaymentResult
                                            {
                                                NewPaymentStatus = PaymentStatus.Paid,
                                                CaptureTransactionId = sale.id
                                            };
                                            _orderProcessingService.ProcessNextRecurringPayment(recurringPayment, processPaymentResult);
                                        }
                                    }
                                }
                                else
                                    _logger.Error(string.Format("PayPal error: Sale is {0} for the order #{1}", sale.state, initialOrder.Id));
                        }
                    }
                    else
                    //standard payment
                    {
                        var order = _orderService.GetOrderByGuid(new Guid(sale.invoice_number));
                        if (order != null)
                        {
                            if (sale.state.ToLowerInvariant().Equals("completed"))
                            {
                                if (_orderProcessingService.CanMarkOrderAsPaid(order))
                                {
                                    order.CaptureTransactionId = sale.id;
                                    order.CaptureTransactionResult = sale.state;
                                    _orderService.UpdateOrder(order);
                                    _orderProcessingService.MarkOrderAsPaid(order);
                                }
                            }
                            if (sale.state.ToLowerInvariant().Equals("denied"))
                            {
                                var reason = string.Format("Payment is denied. {0}", sale.fmf_details != null ?
                                    string.Format("Based on fraud filter: {0}. {1}", sale.fmf_details.name, sale.fmf_details.description) : string.Empty);
                                order.OrderNotes.Add(new OrderNote
                                {
                                    Note = reason,
                                    DisplayToCustomer = false,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                                _logger.Error(string.Format("PayPal error: {0}", reason));
                            }
                        }
                        else
                            _logger.Error(string.Format("PayPal error: Order with guid {0} was not found", sale.invoice_number));
                    }
                }

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (PayPal.PayPalException exc)
            {
                if (exc is PayPal.ConnectionException)
                {
                    var error = JsonFormatter.ConvertFromJson<Error>((exc as PayPal.ConnectionException).Response);
                    if (error != null)
                    {
                        _logger.Error(string.Format("PayPal error: {0} ({1})", error.message, error.name));
                        if (error.details != null)
                            error.details.ForEach(x => _logger.Error(string.Format("{0} {1}", x.field, x.issue)));
                    }
                    else
                        _logger.Error(exc.InnerException != null ? exc.InnerException.Message : exc.Message);
                }
                else
                    _logger.Error(exc.InnerException != null ? exc.InnerException.Message : exc.Message);

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        #endregion
    }
}