using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.PayPalStandard.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.PayPalStandard.Controllers
{
    public class PaymentPayPalStandardController : BasePaymentController
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public PaymentPayPalStandardController(IGenericAttributeService genericAttributeService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ILogger logger,
            INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            ShoppingCartSettings shoppingCartSettings)
        {
            _genericAttributeService = genericAttributeService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _logger = logger;
            _notificationService = notificationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Utilities

        protected virtual void ProcessRecurringPayment(string invoiceId, PaymentStatus newPaymentStatus, string transactionId, string ipnInfo)
        {
            Guid orderNumberGuid;

            try
            {
                orderNumberGuid = new Guid(invoiceId);
            }
            catch
            {
                orderNumberGuid = Guid.Empty;
            }

            var order = _orderService.GetOrderByGuid(orderNumberGuid);
            if (order == null)
            {
                _logger.Error("PayPal IPN. Order is not found", new NopException(ipnInfo));
                return;
            }

            var recurringPayments = _orderService.SearchRecurringPayments(initialOrderId: order.Id);

            foreach (var rp in recurringPayments)
            {
                switch (newPaymentStatus)
                {
                    case PaymentStatus.Authorized:
                    case PaymentStatus.Paid:
                        {
                            var recurringPaymentHistory = rp.RecurringPaymentHistory;
                            if (!recurringPaymentHistory.Any())
                            {
                                //first payment
                                var rph = new RecurringPaymentHistory
                                {
                                    RecurringPaymentId = rp.Id,
                                    OrderId = order.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                };
                                rp.RecurringPaymentHistory.Add(rph);
                                _orderService.UpdateRecurringPayment(rp);
                            }
                            else
                            {
                                //next payments
                                var processPaymentResult = new ProcessPaymentResult
                                {
                                    NewPaymentStatus = newPaymentStatus
                                };
                                if (newPaymentStatus == PaymentStatus.Authorized)
                                    processPaymentResult.AuthorizationTransactionId = transactionId;
                                else
                                    processPaymentResult.CaptureTransactionId = transactionId;

                                _orderProcessingService.ProcessNextRecurringPayment(rp,
                                    processPaymentResult);
                            }
                        }

                        break;
                    case PaymentStatus.Voided:
                        //failed payment
                        var failedPaymentResult = new ProcessPaymentResult
                        {
                            Errors = new[] { $"PayPal IPN. Recurring payment is {nameof(PaymentStatus.Voided).ToLower()} ." },
                            RecurringPaymentFailed = true
                        };
                        _orderProcessingService.ProcessNextRecurringPayment(rp, failedPaymentResult);
                        break;
                }
            }

            //OrderService.InsertOrderNote(newOrder.OrderId, sb.ToString(), DateTime.UtcNow);
            _logger.Information("PayPal IPN. Recurring info", new NopException(ipnInfo));
        }

        protected virtual void ProcessPayment(string orderNumber, string ipnInfo, PaymentStatus newPaymentStatus, decimal mcGross, string transactionId)
        {
            Guid orderNumberGuid;

            try
            {
                orderNumberGuid = new Guid(orderNumber);
            }
            catch
            {
                orderNumberGuid = Guid.Empty;
            }

            var order = _orderService.GetOrderByGuid(orderNumberGuid);

            if (order == null)
            {
                _logger.Error("PayPal IPN. Order is not found", new NopException(ipnInfo));
                return;
            }

            //order note
            order.OrderNotes.Add(new OrderNote
            {
                Note = ipnInfo,
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            _orderService.UpdateOrder(order);

            //validate order total
            if ((newPaymentStatus == PaymentStatus.Authorized || newPaymentStatus == PaymentStatus.Paid) && !Math.Round(mcGross, 2).Equals(Math.Round(order.OrderTotal, 2)))
            {
                var errorStr = $"PayPal IPN. Returned order total {mcGross} doesn't equal order total {order.OrderTotal}. Order# {order.Id}.";
                //log
                _logger.Error(errorStr);
                //order note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = errorStr,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);

                return;
            }

            switch (newPaymentStatus)
            {
                case PaymentStatus.Authorized:
                    if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
                        _orderProcessingService.MarkAsAuthorized(order);
                    break;
                case PaymentStatus.Paid:
                    if (_orderProcessingService.CanMarkOrderAsPaid(order))
                    {
                        order.AuthorizationTransactionId = transactionId;
                        _orderService.UpdateOrder(order);

                        _orderProcessingService.MarkOrderAsPaid(order);
                    }

                    break;
                case PaymentStatus.Refunded:
                    var totalToRefund = Math.Abs(mcGross);
                    if (totalToRefund > 0 && Math.Round(totalToRefund, 2).Equals(Math.Round(order.OrderTotal, 2)))
                    {
                        //refund
                        if (_orderProcessingService.CanRefundOffline(order))
                            _orderProcessingService.RefundOffline(order);
                    }
                    else
                    {
                        //partial refund
                        if (_orderProcessingService.CanPartiallyRefundOffline(order, totalToRefund))
                            _orderProcessingService.PartiallyRefundOffline(order, totalToRefund);
                    }

                    break;
                case PaymentStatus.Voided:
                    if (_orderProcessingService.CanVoidOffline(order))
                        _orderProcessingService.VoidOffline(order);

                    break;
            }
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var payPalStandardPaymentSettings = _settingService.LoadSetting<PayPalStandardPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                UseSandbox = payPalStandardPaymentSettings.UseSandbox,
                BusinessEmail = payPalStandardPaymentSettings.BusinessEmail,
                PdtToken = payPalStandardPaymentSettings.PdtToken,
                PassProductNamesAndTotals = payPalStandardPaymentSettings.PassProductNamesAndTotals,
                AdditionalFee = payPalStandardPaymentSettings.AdditionalFee,
                AdditionalFeePercentage = payPalStandardPaymentSettings.AdditionalFeePercentage,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope <= 0)
                return View("~/Plugins/Payments.PayPalStandard/Views/Configure.cshtml", model);

            model.UseSandbox_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.UseSandbox, storeScope);
            model.BusinessEmail_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.BusinessEmail, storeScope);
            model.PdtToken_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.PdtToken, storeScope);
            model.PassProductNamesAndTotals_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.PassProductNamesAndTotals, storeScope);
            model.AdditionalFee_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.AdditionalFee, storeScope);
            model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.AdditionalFeePercentage, storeScope);

            return View("~/Plugins/Payments.PayPalStandard/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var payPalStandardPaymentSettings = _settingService.LoadSetting<PayPalStandardPaymentSettings>(storeScope);

            //save settings
            payPalStandardPaymentSettings.UseSandbox = model.UseSandbox;
            payPalStandardPaymentSettings.BusinessEmail = model.BusinessEmail;
            payPalStandardPaymentSettings.PdtToken = model.PdtToken;
            payPalStandardPaymentSettings.PassProductNamesAndTotals = model.PassProductNamesAndTotals;
            payPalStandardPaymentSettings.AdditionalFee = model.AdditionalFee;
            payPalStandardPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(payPalStandardPaymentSettings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(payPalStandardPaymentSettings, x => x.BusinessEmail, model.BusinessEmail_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(payPalStandardPaymentSettings, x => x.PdtToken, model.PdtToken_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(payPalStandardPaymentSettings, x => x.PassProductNamesAndTotals, model.PassProductNamesAndTotals_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(payPalStandardPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(payPalStandardPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        //action displaying notification (warning) to a store owner about inaccurate PayPal rounding
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult RoundingWarning(bool passProductNamesAndTotals)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prices and total aren't rounded, so display warning
            if (passProductNamesAndTotals && !_shoppingCartSettings.RoundPricesDuringCalculation)
                return Json(new { Result = _localizationService.GetResource("Plugins.Payments.PayPalStandard.RoundingWarning") });

            return Json(new { Result = string.Empty });
        }

        public IActionResult PDTHandler()
        {
            var tx = _webHelper.QueryString<string>("tx");

            if (!(_paymentPluginManager.LoadPluginBySystemName("Payments.PayPalStandard") is PayPalStandardPaymentProcessor processor) || !_paymentPluginManager.IsPluginActive(processor))
                throw new NopException("PayPal Standard module cannot be loaded");

            if (processor.GetPdtDetails(tx, out var values, out var response))
            {
                values.TryGetValue("custom", out var orderNumber);
                var orderNumberGuid = Guid.Empty;
                try
                {
                    orderNumberGuid = new Guid(orderNumber);
                }
                catch
                {
                    // ignored
                }

                var order = _orderService.GetOrderByGuid(orderNumberGuid);

                if (order == null)
                    return RedirectToAction("Index", "Home", new { area = string.Empty });

                var mcGross = decimal.Zero;

                try
                {
                    mcGross = decimal.Parse(values["mc_gross"], new CultureInfo("en-US"));
                }
                catch (Exception exc)
                {
                    _logger.Error("PayPal PDT. Error getting mc_gross", exc);
                }

                values.TryGetValue("payer_status", out var payerStatus);
                values.TryGetValue("payment_status", out var paymentStatus);
                values.TryGetValue("pending_reason", out var pendingReason);
                values.TryGetValue("mc_currency", out var mcCurrency);
                values.TryGetValue("txn_id", out var txnId);
                values.TryGetValue("payment_type", out var paymentType);
                values.TryGetValue("payer_id", out var payerId);
                values.TryGetValue("receiver_id", out var receiverId);
                values.TryGetValue("invoice", out var invoice);
                values.TryGetValue("payment_fee", out var paymentFee);

                var sb = new StringBuilder();
                sb.AppendLine("PayPal PDT:");
                sb.AppendLine("mc_gross: " + mcGross);
                sb.AppendLine("Payer status: " + payerStatus);
                sb.AppendLine("Payment status: " + paymentStatus);
                sb.AppendLine("Pending reason: " + pendingReason);
                sb.AppendLine("mc_currency: " + mcCurrency);
                sb.AppendLine("txn_id: " + txnId);
                sb.AppendLine("payment_type: " + paymentType);
                sb.AppendLine("payer_id: " + payerId);
                sb.AppendLine("receiver_id: " + receiverId);
                sb.AppendLine("invoice: " + invoice);
                sb.AppendLine("payment_fee: " + paymentFee);

                var newPaymentStatus = PayPalHelper.GetPaymentStatus(paymentStatus, string.Empty);
                sb.AppendLine("New payment status: " + newPaymentStatus);

                //order note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = sb.ToString(),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);

                //validate order total
                var orderTotalSentToPayPal = _genericAttributeService.GetAttribute<decimal?>(order, PayPalHelper.OrderTotalSentToPayPal);
                if (orderTotalSentToPayPal.HasValue && mcGross != orderTotalSentToPayPal.Value)
                {
                    var errorStr = $"PayPal PDT. Returned order total {mcGross} doesn't equal order total {order.OrderTotal}. Order# {order.Id}.";
                    //log
                    _logger.Error(errorStr);
                    //order note
                    order.OrderNotes.Add(new OrderNote
                    {
                        Note = errorStr,
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(order);

                    return RedirectToAction("Index", "Home", new { area = string.Empty });
                }

                //clear attribute
                if (orderTotalSentToPayPal.HasValue)
                    _genericAttributeService.SaveAttribute<decimal?>(order, PayPalHelper.OrderTotalSentToPayPal, null);

                if (newPaymentStatus != PaymentStatus.Paid)
                    return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });

                if (!_orderProcessingService.CanMarkOrderAsPaid(order))
                    return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });

                //mark order as paid
                order.AuthorizationTransactionId = txnId;
                _orderService.UpdateOrder(order);
                _orderProcessingService.MarkOrderAsPaid(order);

                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
            else
            {
                if (!values.TryGetValue("custom", out var orderNumber))
                    orderNumber = _webHelper.QueryString<string>("cm");

                var orderNumberGuid = Guid.Empty;

                try
                {
                    orderNumberGuid = new Guid(orderNumber);
                }
                catch
                {
                    // ignored
                }

                var order = _orderService.GetOrderByGuid(orderNumberGuid);
                if (order == null)
                    return RedirectToAction("Index", "Home", new { area = string.Empty });

                //order note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = "PayPal PDT failed. " + response,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);

                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
        }

        public IActionResult IPNHandler()
        {
            byte[] parameters;

            using (var stream = new MemoryStream())
            {
                Request.Body.CopyTo(stream);
                parameters = stream.ToArray();
            }

            var strRequest = Encoding.ASCII.GetString(parameters);

            if (!(_paymentPluginManager.LoadPluginBySystemName("Payments.PayPalStandard") is PayPalStandardPaymentProcessor processor) || !_paymentPluginManager.IsPluginActive(processor))
                throw new NopException("PayPal Standard module cannot be loaded");

            if (!processor.VerifyIpn(strRequest, out var values))
            {
                _logger.Error("PayPal IPN failed.", new NopException(strRequest));

                //nothing should be rendered to visitor
                return Content(string.Empty);
            }

            var mcGross = decimal.Zero;

            try
            {
                mcGross = decimal.Parse(values["mc_gross"], new CultureInfo("en-US"));
            }
            catch
            {
                // ignored
            }

            values.TryGetValue("payment_status", out var paymentStatus);
            values.TryGetValue("pending_reason", out var pendingReason);
            values.TryGetValue("txn_id", out var txnId);
            values.TryGetValue("txn_type", out var txnType);
            values.TryGetValue("rp_invoice_id", out var rpInvoiceId);

            var sb = new StringBuilder();
            sb.AppendLine("PayPal IPN:");
            foreach (var kvp in values)
            {
                sb.AppendLine(kvp.Key + ": " + kvp.Value);
            }

            var newPaymentStatus = PayPalHelper.GetPaymentStatus(paymentStatus, pendingReason);
            sb.AppendLine("New payment status: " + newPaymentStatus);

            var ipnInfo = sb.ToString();

            switch (txnType)
            {
                case "recurring_payment":
                    ProcessRecurringPayment(rpInvoiceId, newPaymentStatus, txnId, ipnInfo);
                    break;
                case "recurring_payment_failed":
                    if (Guid.TryParse(rpInvoiceId, out var orderGuid))
                    {
                        var order = _orderService.GetOrderByGuid(orderGuid);
                        if (order != null)
                        {
                            var recurringPayment = _orderService.SearchRecurringPayments(initialOrderId: order.Id)
                                .FirstOrDefault();
                            //failed payment
                            if (recurringPayment != null)
                                _orderProcessingService.ProcessNextRecurringPayment(recurringPayment,
                                    new ProcessPaymentResult
                                    {
                                        Errors = new[] { txnType },
                                        RecurringPaymentFailed = true
                                    });
                        }
                    }

                    break;
                default:
                    values.TryGetValue("custom", out var orderNumber);
                    ProcessPayment(orderNumber, ipnInfo, newPaymentStatus, mcGross, txnId);

                    break;
            }

            //nothing should be rendered to visitor
            return Content(string.Empty);
        }

        public IActionResult CancelOrder()
        {
            var order = _orderService.SearchOrders(_storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1).FirstOrDefault();

            if (order != null)
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });

            return RedirectToRoute("Homepage");
        }

        #endregion
    }
}