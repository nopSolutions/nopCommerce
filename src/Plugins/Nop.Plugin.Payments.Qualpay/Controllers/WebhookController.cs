using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Qualpay.Services;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Plugin.Payments.Qualpay.Controllers
{
    public class WebhookController : BaseController
    {
        #region Fields

        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly QualpayManager _qualpayManager;

        #endregion

        #region Ctor

        public WebhookController(IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            QualpayManager qualpayManager)
        {
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _qualpayManager = qualpayManager;
        }

        #endregion

        #region Methods

        [HttpPost]
        [HttpsRequirement(SslRequirement.Yes)]
        public IActionResult WebhookHandler()
        {
            try
            {
                //validate request and get recurring payment subscription details
                var (isValid, webhookEvent) = _qualpayManager.GetSubscriptionFromWebhookRequest(Request);
                var subscription = webhookEvent?.Data;
                if (!isValid || subscription == null)
                    return Ok();

                //try to get the initial order (an order GUID stored in a plan description)
                var initialOrder = _orderService.GetOrderByGuid(new Guid(subscription.PlanDesc));
                if (initialOrder == null)
                    return Ok();

                //try to get related recurring payment
                var recurringPayment = _orderService.SearchRecurringPayments(initialOrderId: initialOrder.Id).FirstOrDefault();
                if (recurringPayment == null)
                    return Ok();

                //whether payment failed
                if (webhookEvent.Event.Equals(QualpayDefaults.SubscriptionPaymentFailureWebhookEvent, StringComparison.InvariantCultureIgnoreCase))
                {
                    _orderProcessingService.ProcessNextRecurringPayment(recurringPayment, new ProcessPaymentResult
                    {
                        RecurringPaymentFailed = true,
                        Errors = new[] { "Qualpay error: recurring payment failed" }
                    });
                    return Ok();
                }

                //or ensure that payment succeeded
                if (!webhookEvent.Event.Equals(QualpayDefaults.SubscriptionPaymentSuccessWebhookEvent, StringComparison.InvariantCultureIgnoreCase))
                    return Ok();

                //try to get last subscription transaction
                var transaction = _qualpayManager.GetSubscriptionTransactions(subscription.SubscriptionId).FirstOrDefault();
                if (transaction == null)
                    return Ok();

                //get all orders of this recurring payment
                var orders = _orderService.GetOrdersByIds(recurringPayment.RecurringPaymentHistory.Select(order => order.OrderId).ToArray());

                //whether an order for this transaction already exists
                var orderExists = orders.Any(order => !string.IsNullOrEmpty(order.CaptureTransactionId) &&
                    order.CaptureTransactionId.Equals(transaction.PgId, StringComparison.InvariantCultureIgnoreCase));
                if (orderExists)
                    return Ok();

                //order doesn't exist, so handle this payment
                _orderProcessingService.ProcessNextRecurringPayment(recurringPayment, new ProcessPaymentResult
                {
                    AuthorizationTransactionCode = transaction.AuthCode,
                    AuthorizationTransactionId = transaction.PgId,
                    CaptureTransactionId = transaction.PgId,
                    CaptureTransactionResult = $"Transaction is {transaction.TranStatus}",
                    AuthorizationTransactionResult = $"Transaction is {transaction.TranStatus}",
                    NewPaymentStatus = PaymentStatus.Paid
                });
            }
            catch { }

            return Ok();
        }

        #endregion
    }
}