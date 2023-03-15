using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Common;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Payments.CyberSource.Services
{
    /// <summary>
    /// Represents a schedule task to update order status
    /// </summary>
    public class OrderStatusUpdateTask : IScheduleTask
    {
        #region Fields

        protected readonly CyberSourceService _cyberSourceService;
        protected readonly CyberSourceSettings _cyberSourceSettings;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly IOrderProcessingService _orderProcessingService;
        protected readonly IOrderService _orderService;
        protected readonly IPaymentPluginManager _paymentPluginManager;

        #endregion

        #region Ctor

        public OrderStatusUpdateTask(CyberSourceService cyberSourceService,
            CyberSourceSettings cyberSourceSettings,
            IGenericAttributeService genericAttributeService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager)
        {
            _cyberSourceService = cyberSourceService;
            _cyberSourceSettings = cyberSourceSettings;
            _genericAttributeService = genericAttributeService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Execute task
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ExecuteAsync()
        {
            //ensure that CyberSource payment method is active
            if (!await _paymentPluginManager.IsPluginActiveAsync(CyberSourceDefaults.SystemName))
                return;

            if (!CyberSourceService.IsConfigured(_cyberSourceSettings))
                return;

            var (report, _) = await _cyberSourceService
                .GetConversionDetailTransactionsAsync(startTime: DateTime.UtcNow.AddDays(-1), endTime: DateTime.UtcNow);

            if (!report?.ConversionDetails?.Any() ?? true)
                return;

            foreach (var conversion in report.ConversionDetails)
            {
                if (!Guid.TryParse(conversion.MerchantReferenceNumber, out var orderGuid))
                    continue;

                if (await _orderService.GetOrderByGuidAsync(orderGuid) is not Order order)
                    continue;

                if (order.PaymentMethodSystemName != CyberSourceDefaults.SystemName || order.OrderStatus != OrderStatus.Pending)
                    continue;

                if (string.IsNullOrEmpty(conversion.NewDecision))
                    continue;

                if (conversion.NewDecision != CyberSourceDefaults.Decisions.Rejected && conversion.NewDecision != CyberSourceDefaults.Decisions.Accepted)
                    continue;

                var note = $"Order status has been changed by CyberSource decision. Decision request details: {Environment.NewLine}{conversion}";
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = note,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                var paymentStatus = await _genericAttributeService.GetAttributeAsync<PaymentStatus?>(order, CyberSourceDefaults.PaymentStatusAttributeName);
                await _genericAttributeService.SaveAttributeAsync<PaymentStatus?>(order, CyberSourceDefaults.PaymentStatusAttributeName, null);

                if (conversion.NewDecision == CyberSourceDefaults.Decisions.Rejected)
                    await _orderProcessingService.CancelOrderAsync(order, false);

                if (conversion.NewDecision == CyberSourceDefaults.Decisions.Accepted)
                {
                    order.PaymentStatus = paymentStatus ?? PaymentStatus.Paid;
                    await _orderProcessingService.CheckOrderStatusAsync(order);
                }
            }
        }

        #endregion
    }
}