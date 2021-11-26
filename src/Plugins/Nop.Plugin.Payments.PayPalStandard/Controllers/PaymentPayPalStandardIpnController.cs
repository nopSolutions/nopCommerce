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

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task ProcessRecurringPaymentAsync(string invoiceId, PaymentStatus newPaymentStatus, string transactionId, string ipnInfo)
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

            var order = await _orderService.GetOrderByGuidAsync(orderNumberGuid);
            if (order == null)
            {
                await _logger.ErrorAsync("PayPal IPN. Order is not found", new NopException(ipnInfo));
                return;
            }

            var recurringPayments = await _orderService.SearchRecurringPaymentsAsync(initialOrderId: order.Id);

            foreach (var rp in recurringPayments)
            {
                switch (newPaymentStatus)
                {
                    case PaymentStatus.Authorized:
                    case PaymentStatus.Paid:
                        {
                            var recurringPaymentHistory = await _orderService.GetRecurringPaymentHistoryAsync(rp);
                            if (!recurringPaymentHistory.Any())
                            {
                                await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory
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

                                await _orderProcessingService.ProcessNextRecurringPaymentAsync(rp,
                                    processPaymentResult);
                            }
                        }

                        break;
                    case PaymentStatus.Voided:
                        //failed payment
                        var failedPaymentResult = new ProcessPaymentResult
                        {
                            Errors = new[] { $"PayPal IPN. Recurring payment is {nameof(PaymentStatus.Voided).ToLowerInvariant()} ." },
                            RecurringPaymentFailed = true
                        };
                        await _orderProcessingService.ProcessNextRecurringPaymentAsync(rp, failedPaymentResult);
                        break;
                }
            }

            //OrderService.InsertOrderNote(newOrder.OrderId, sb.ToString(), DateTime.UtcNow);
            await _logger.InformationAsync("PayPal IPN. Recurring info", new NopException(ipnInfo));
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task ProcessPaymentAsync(string orderNumber, string ipnInfo, PaymentStatus newPaymentStatus, decimal mcGross, string transactionId)
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

            var order = await _orderService.GetOrderByGuidAsync(orderNumberGuid);

            if (order == null)
            {
                await _logger.ErrorAsync("PayPal IPN. Order is not found", new NopException(ipnInfo));
                return;
            }

            //order note
            await _orderService.InsertOrderNoteAsync(new OrderNote
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
                await _logger.ErrorAsync(errorStr);
                //order note
                await _orderService.InsertOrderNoteAsync(new OrderNote
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
                        await _orderProcessingService.MarkAsAuthorizedAsync(order);
                    break;
                case PaymentStatus.Paid:
                    if (_orderProcessingService.CanMarkOrderAsPaid(order))
                    {
                        order.AuthorizationTransactionId = transactionId;
                        await _orderService.UpdateOrderAsync(order);

                        await _orderProcessingService.MarkOrderAsPaidAsync(order);
                    }

                    break;
                case PaymentStatus.Refunded:
                    var totalToRefund = Math.Abs(mcGross);
                    if (totalToRefund > 0 && Math.Round(totalToRefund, 2).Equals(Math.Round(order.OrderTotal, 2)))
                    {
                        //refund
                        if (_orderProcessingService.CanRefundOffline(order))
                            await _orderProcessingService.RefundOfflineAsync(order);
                    }
                    else
                    {
                        //partial refund
                        if (_orderProcessingService.CanPartiallyRefundOffline(order, totalToRefund))
                            await _orderProcessingService.PartiallyRefundOfflineAsync(order, totalToRefund);
                    }

                    break;
                case PaymentStatus.Voided:
                    if (_orderProcessingService.CanVoidOffline(order))
                        await _orderProcessingService.VoidOfflineAsync(order);

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

            if (await _paymentPluginManager.LoadPluginBySystemNameAsync("Payments.PayPalStandard") is not PayPalStandardPaymentProcessor processor || !_paymentPluginManager.IsPluginActive(processor))
                throw new NopException("PayPal Standard module cannot be loaded");

            var (result, values) = await processor.VerifyIpnAsync(strRequest);

            if (!result)
            {
                await _logger.ErrorAsync("PayPal IPN failed.", new NopException(strRequest));

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
                    await ProcessRecurringPaymentAsync(rpInvoiceId, newPaymentStatus, txnId, ipnInfo);
                    break;
                case "recurring_payment_failed":
                    if (Guid.TryParse(rpInvoiceId, out var orderGuid))
                    {
                        var order = await _orderService.GetOrderByGuidAsync(orderGuid);
                        if (order != null)
                        {
                            var recurringPayment = (await _orderService.SearchRecurringPaymentsAsync(initialOrderId: order.Id))
                                .FirstOrDefault();
                            //failed payment
                            if (recurringPayment != null)
                                await _orderProcessingService.ProcessNextRecurringPaymentAsync(recurringPayment,
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
                    await ProcessPaymentAsync(orderNumber, ipnInfo, newPaymentStatus, mcGross, txnId);

                    break;
            }

            //nothing should be rendered to visitor
            return Ok();
        }

        #endregion
    }
}