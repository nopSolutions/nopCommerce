using System;
using System.Collections.Generic;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Tasks;

namespace Nop.Services.Common
{
    /// <summary>
    /// Represents a task for sending reminding notification to customer
    /// </summary>
    public partial class RemindMeNotificationTask : IScheduleTask
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;

        #endregion

        #region Ctor

        public RemindMeNotificationTask(ICustomerService customerService, IOrderService orderService)
        {
            _customerService = customerService;
            _orderService = orderService;
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
                    foreach (var customer in customers)
                    {
                        var order = _orderService.SearchOrdersAsync(customerId: customer.Id, createdFromUtc: DateTime.Now.AddDays(-1));
                        if (order == null)
                        {
                            if (!string.IsNullOrEmpty(customer.PushToken))
                            {
                                //woking is in progress
                                //var expoSDKClient = new PushApiClient();
                                //var pushTicketReq = new PushTicketRequest()
                                //{
                                //    PushTo = new List<string>() { customer.PushToken },
                                //    PushTitle = await _localizationService.GetResourceAsync("RemindMeNotificationTask.Title"),
                                //    PushBody = await _localizationService.GetResourceAsync("RemindMeNotificationTask.Body")
                                //};
                                //var result = expoSDKClient.PushSendAsync(pushTicketReq).GetAwaiter().GetResult();
                            }
                        }

                    }
                }
            }
        }

        #endregion
    }
}