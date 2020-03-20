using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Domain;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Services;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using PayPalCheckoutSdk.Orders;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Controllers
{
    public class PayPalSmartPaymentButtonsWebhookController : Controller
    {
        #region Fields

        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ServiceManager _serviceManager;

        #endregion

        #region Ctor

        public PayPalSmartPaymentButtonsWebhookController(IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            ISettingService settingService,
            IStoreContext storeContext,
            ServiceManager serviceManager)
        {
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _settingService = settingService;
            _storeContext = storeContext;
            _serviceManager = serviceManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Pending
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="authorization">Authorization</param>
        /// <param name="capture">Capture</param>
        private void MarkOrderAsPending(Core.Domain.Orders.Order order, Authorization authorization = null, Capture capture = null)
        {
            order.CaptureTransactionResult = $"{authorization?.Status ?? capture?.Status}. " +
                $"{authorization?.AuthorizationStatusDetails?.Reason ?? capture?.CaptureStatusDetails?.Reason}";
            order.OrderStatus = OrderStatus.Pending;
            _orderService.UpdateOrder(order);
            _orderProcessingService.CheckOrderStatus(order);
        }

        /// <summary>
        /// Authorize
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="authorization">Authorization</param>
        private void MarkOrderAsAuthorized(Core.Domain.Orders.Order order, Authorization authorization)
        {
            //compare amounts
            var orderTotal = Math.Round(order.OrderTotal, 2);
            if (!decimal.TryParse(authorization.Amount?.Value, out var authorizedAmount) || authorizedAmount != orderTotal)
                return;

            //all is ok, so authorize order
            if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
            {
                order.AuthorizationTransactionId = authorization.Id;
                order.AuthorizationTransactionResult = authorization.Status;
                _orderService.UpdateOrder(order);
                _orderProcessingService.MarkAsAuthorized(order);
            }
        }

        /// <summary>
        /// Void
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="authorization">Authorization</param>
        private void MarkOrderAsVoided(Core.Domain.Orders.Order order, Authorization authorization)
        {
            if (_orderProcessingService.CanVoidOffline(order))
            {
                order.AuthorizationTransactionId = authorization.Id;
                order.AuthorizationTransactionResult = $"{authorization.Status}. {authorization.AuthorizationStatusDetails?.Reason}";
                _orderService.UpdateOrder(order);
                _orderProcessingService.VoidOffline(order);
            }
        }

        /// <summary>
        /// Paid
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="capture">Capture</param>
        private void MarkOrderAsPaid(Core.Domain.Orders.Order order, Capture capture)
        {
            //compare amounts
            var orderTotal = Math.Round(order.OrderTotal, 2);
            if (!decimal.TryParse(capture.Amount?.Value, out var capturedAmount) || capturedAmount != orderTotal)
                return;

            //all is ok, so paid order
            if (_orderProcessingService.CanMarkOrderAsPaid(order))
            {
                order.CaptureTransactionId = capture.Id;
                order.CaptureTransactionResult = capture.Status;
                _orderService.UpdateOrder(order);
                _orderProcessingService.MarkOrderAsPaid(order);
            }
        }

        /// <summary>
        /// Refund
        /// </summary>
        /// <param name="order">Order</param>
        private void MarkOrderAsRefunded(Core.Domain.Orders.Order order)
        {
            if (_orderProcessingService.CanRefundOffline(order))
                _orderProcessingService.RefundOffline(order);
        }

        /// <summary>
        /// Partially refund
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="refund">Refund</param>
        private void MarkOrderAsPartiallyRefunded(Core.Domain.Orders.Order order, PayPalCheckoutSdk.Payments.Refund refund)
        {
            if (!decimal.TryParse(refund.Amount?.Value, out var refundedAmount))
                return;

            if (_orderProcessingService.CanPartiallyRefundOffline(order, refundedAmount))
                _orderProcessingService.PartiallyRefundOffline(order, refundedAmount);
        }

        /// <summary>
        /// Cancel
        /// </summary>
        /// <param name="order">Order</param>
        private void MarkOrderAsCancelled(Core.Domain.Orders.Order order)
        {
            if (_orderProcessingService.CanCancelOrder(order))
                _orderProcessingService.CancelOrder(order, true);
        }

        #endregion

        #region Methods

        [HttpPost]
        public IActionResult WebhookHandler()
        {
            try
            {
                //get request details
                var storeScope = _storeContext.CurrentStore.Id;
                var settings = _settingService.LoadSetting<PayPalSmartPaymentButtonsSettings>(storeScope);
                var (details, rawRequestString) = _serviceManager.GetWebhookRequestDetails(Request, settings);
                if (details == null)
                    return Ok();

                var authorization = details as ExtendedAuthorization;
                var capture = details as ExtendedCapture;
                var refund = details as PayPalCheckoutSdk.Payments.Refund;
                if (authorization == null && capture == null && refund == null)
                    return Ok();

                var orderReference = authorization?.CustomId ?? capture?.CustomId;
                if (!Guid.TryParse(orderReference, out var orderGuid))
                    return Ok();

                var order = _orderService.GetOrderByGuid(orderGuid);
                if (order == null)
                    return Ok();

                _orderService.InsertOrderNote(new OrderNote()
                {
                    OrderId = order.Id,
                    Note = $"Webhook details: {Environment.NewLine}{rawRequestString}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                _orderService.UpdateOrder(order);

                //authorization actions
                if (authorization != null)
                {
                    switch (authorization.Status?.ToLowerInvariant())
                    {
                        case "created":
                            MarkOrderAsAuthorized(order, authorization);
                            break;
                        case "denied":
                        case "expired":
                        case "pending":
                            MarkOrderAsPending(order, authorization: authorization);
                            break;
                        case "voided":
                            MarkOrderAsVoided(order, authorization);
                            break;
                    }
                }

                //capture actions
                if (capture != null)
                {
                    switch (capture.Status?.ToLowerInvariant())
                    {
                        case "completed":
                            MarkOrderAsPaid(order, capture);
                            break;
                        case "pending":
                        case "declined":
                            MarkOrderAsPending(order, capture: capture);
                            break;
                        case "refunded":
                            MarkOrderAsRefunded(order);
                            break;
                    }
                }

                //refund actions
                if (refund != null)
                {
                    switch (refund.Status?.ToLowerInvariant())
                    {
                        case "completed":
                            MarkOrderAsPartiallyRefunded(order, refund);
                            break;
                    }
                }

            }
            catch { }

            return Ok();
        }

        #endregion
    }
}