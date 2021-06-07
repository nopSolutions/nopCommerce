using System;
using System.Collections.Generic;
using Expo.Server.Client;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Services.Common.PushApiTask;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Tasks;

namespace Nop.Services.Common
{
    /// <summary>
    /// Represents a task for sending reminding notification to customer
    /// </summary>
    public partial class RemindMeAndRateRemainderNotificationTask : IScheduleTask
    {
        #region Fields

        private readonly CatalogSettings _catalogSetting;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public RemindMeAndRateRemainderNotificationTask(CatalogSettings catalogSetting,
            ICustomerService customerService,
            IOrderService orderService,
            ILocalizationService localizationService)
        {
            _catalogSetting = catalogSetting;
            _customerService = customerService;
            _orderService = orderService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            if (DateTime.Now.Hour >= _catalogSetting.StartingTimeOfRemindMeTask && DateTime.Now.Hour <= _catalogSetting.EndingTimeOfRemindMeTask /*&& DateTime.Now.DayOfWeek >= DayOfWeek.Monday && DateTime.Now.DayOfWeek <= DayOfWeek.Friday*/)
            {
                var customers = await _customerService.GetAllPushNotificationCustomersAsync(isRemindMeNotification: true);
                if (customers.Count > 0)
                {
                    DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    var osIds = new List<int> { (int)OrderStatus.Complete, (int)OrderStatus.Pending, (int)OrderStatus.Processing };
                    foreach (var customer in customers)
                    {
                        var order = await _orderService.SearchOrdersAsync(customerId: customer.Id, createdToUtc: currentDate, osIds: osIds);
                        if (order.Count == 0)
                        {
                            if (!string.IsNullOrEmpty(customer.PushToken))
                            {
                                var expoSDKClient = new PushApiTaskClient();
                                var pushTicketReq = new PushApiTaskTicketRequest()
                                {
                                    PushTo = new List<string>() { customer.PushToken },
                                    PushTitle = await _localizationService.GetResourceAsync("RemindMeNotificationTask.Title"),
                                    PushBody = await _localizationService.GetResourceAsync("RemindMeNotificationTask.Body")
                                };
                                var result = await expoSDKClient.PushSendAsync(pushTicketReq);
                            }
                        }
                    }
                }
            }
            var rateRemainderCustomers = await _customerService.GetAllPushNotificationCustomersAsync(isRateReminderNotification: true);
            if (rateRemainderCustomers.Count > 0)
            {
                DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var osIds = new List<int> { (int)OrderStatus.Complete, (int)OrderStatus.Pending, (int)OrderStatus.Processing };
                foreach (var rateRemainderCustomer in rateRemainderCustomers)
                {
                    var customerOrders = await _orderService.SearchOrdersAsync(customerId: rateRemainderCustomer.Id, createdToUtc: currentDate, osIds: osIds, sendRateNotification: true);
                    if (customerOrders.Count > 0)
                    {
                        foreach (var customerOrder in customerOrders)
                        {
                            TimeSpan diff = DateTime.Now.Subtract(customerOrder.ScheduleDate);
                            if (diff.Hours >= 1)
                            {
                                if (!string.IsNullOrEmpty(rateRemainderCustomer.PushToken))
                                {
                                    var expoSDKClient = new PushApiTaskClient();
                                    var pushTicketReq = new PushApiTaskTicketRequest()
                                    {
                                        PushTo = new List<string>() { rateRemainderCustomer.PushToken },
                                        PushTitle = await _localizationService.GetResourceAsync("RateRemainderNotificationTask.Title"),
                                        PushBody = await _localizationService.GetResourceAsync("RateRemainderNotificationTask.Body"),
                                        PushData = new { customerOrder.Id }
                                    };
                                    var result = await expoSDKClient.PushSendAsync(pushTicketReq);

                                    customerOrder.RateNotificationSend = true;
                                    await _orderService.UpdateOrderAsync(customerOrder);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}