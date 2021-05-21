using System;
using System.Collections.Generic;
using Expo.Server.Client;
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

        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public RemindMeAndRateRemainderNotificationTask(ICustomerService customerService,
            IOrderService orderService,
            ILocalizationService localizationService)
        {
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
            if (DateTime.Now.Hour >= 11 && DateTime.Now.Hour <= 12 && DateTime.Now.DayOfWeek >= DayOfWeek.Monday && DateTime.Now.DayOfWeek <= DayOfWeek.Friday)
            {
                var customers = await _customerService.GetAllPushNotificationCustomersAsync(isRemindMeNotification: true);
                if (customers.Count > 0)
                {
                    DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    foreach (var customer in customers)
                    {
                        var order = await _orderService.SearchOrdersAsync(customerId: customer.Id, createdToUtc: currentDate);
                        if (order == null)
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
                                var result = expoSDKClient.PushSendAsync(pushTicketReq).GetAwaiter().GetResult();
                            }
                        }
                    }
                }
            }
            var rateRemainderCustomers = await _customerService.GetAllPushNotificationCustomersAsync(isRateReminderNotification: true);
            if (rateRemainderCustomers.Count > 0)
            {
                DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                foreach (var rateRemainderCustomer in rateRemainderCustomers)
                {
                    var customerOrders = await _orderService.SearchOrdersAsync(customerId: rateRemainderCustomer.Id, createdToUtc: currentDate);
                    if (customerOrders.Count > 0)
                    {
                        foreach (var customerOrder in customerOrders)
                        {
                            TimeSpan diff = DateTime.Now.Subtract(customerOrder.CreatedOnUtc);
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
                                    var result = expoSDKClient.PushSendAsync(pushTicketReq).GetAwaiter().GetResult();
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