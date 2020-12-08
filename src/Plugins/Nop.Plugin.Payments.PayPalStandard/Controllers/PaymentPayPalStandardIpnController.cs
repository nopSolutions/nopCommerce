using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.PayPalStandard.Controllers
{
    public class PaymentPayPalStandardIpnController : Controller
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;

        #endregion

        #region Ctor

        public PaymentPayPalStandardIpnController(ILogger logger,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager)
        {
            _logger = logger;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
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
                            var recurringPaymentHistory = _orderService.GetRecurringPaymentHistory(rp);
                            if (!recurringPaymentHistory.Any())
                            {
                                _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory
                                {
                                    RecurringPaymentId = rp.Id,
                                    OrderId = order.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
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
            _orderService.InsertOrderNote(new OrderNote
            {
                OrderId = order.Id,
                Note = ipnInfo,
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            //validate order total
            if ((newPaymentStatus == PaymentStatus.Authorized || newPaymentStatus == PaymentStatus.Paid) && !Math.Round(mcGross, 2).Equals(Math.Round(order.OrderTotal, 2)))
            {
                var errorStr = $"PayPal IPN. Returned order total {mcGross} doesn't equal order total {order.OrderTotal}. Order# {order.Id}.";
                //log
                _logger.Error(errorStr);
                //order note
                _orderService.InsertOrderNote(new OrderNote
                {
                    OrderId = order.Id,
                    Note = errorStr,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

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

        public async Task<IActionResult> IPNHandler()
        {
            await using var stream = new MemoryStream();
            await Request.Body.CopyToAsync(stream);
            var strRequest = Encoding.ASCII.GetString(stream.ToArray());

            if (!(_paymentPluginManager.LoadPluginBySystemName("Payments.PayPalStandard") is PayPalStandardPaymentProcessor processor) || !_paymentPluginManager.IsPluginActive(processor))
                throw new NopException("PayPal Standard module cannot be loaded");

            if (!processor.VerifyIpn(strRequest, out var values))
            {
                _logger.Error("PayPal IPN failed.", new NopException(strRequest));

                //nothing should be rendered to visitor
                return Ok();
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
            return Ok();
        }

        #endregion
    }
}