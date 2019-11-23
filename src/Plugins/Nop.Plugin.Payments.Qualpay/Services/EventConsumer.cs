using System;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.UI;

namespace Nop.Plugin.Payments.Qualpay.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<EntityInsertedEvent<RecurringPayment>>,
        IConsumer<PageRenderingEvent>
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;

        #endregion

        #region Ctor

        public EventConsumer(IOrderService orderService,
            IPaymentPluginManager paymentPluginManager)
        {
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Recurring payment inserted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(EntityInsertedEvent<RecurringPayment> eventMessage)
        {
            var recurringPayment = eventMessage?.Entity;
            if (recurringPayment == null)
                return;

            //add first payment to history right after creating recurring payment
            recurringPayment.RecurringPaymentHistory.Add(new RecurringPaymentHistory
            {
                RecurringPayment = recurringPayment,
                CreatedOnUtc = DateTime.UtcNow,
                OrderId = recurringPayment.InitialOrderId,
            });
            _orderService.UpdateRecurringPayment(recurringPayment);
        }

        /// <summary>
        /// Handle page rendering event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(PageRenderingEvent eventMessage)
        {
            if (eventMessage?.Helper?.ViewContext?.ActionDescriptor == null)
                return;

            //check whether the plugin is active
            if (!_paymentPluginManager.IsPluginActive(QualpayDefaults.SystemName))
                return;

            //add Embedded Fields sсript and styles to the one page checkout
            if (eventMessage.GetRouteNames().Any(routeName => routeName.Equals(QualpayDefaults.OnePageCheckoutRouteName)))
            {
                eventMessage.Helper.AddScriptParts(ResourceLocation.Footer, QualpayDefaults.EmbeddedFieldsScriptPath, excludeFromBundle: true);
                eventMessage.Helper.AddCssFileParts(QualpayDefaults.EmbeddedFieldsStylePath, excludeFromBundle: true);
            }
        }

        #endregion
    }
}