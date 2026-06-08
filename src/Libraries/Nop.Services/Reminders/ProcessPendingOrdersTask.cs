using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Reminders;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Messages;
using Nop.Services.ScheduleTasks;
using Nop.Services.Stores;

namespace Nop.Services.Reminders;

/// <summary>
/// Represents a task to process incomplete orders 
/// </summary>
public partial class ProcessPendingOrdersTask : IScheduleTask
{
    #region Fields

    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<Order> _orderRepository;
    protected readonly ISettingService _settingService;
    protected readonly IStoreService _storeService;
    protected readonly IWorkflowMessageService _workflowMessageService;

    #endregion

    #region Ctor

    public ProcessPendingOrdersTask(IMessageTemplateService messageTemplateService,
        IRepository<Customer> customerRepository,
        IRepository<Order> orderRepository,
        ISettingService settingService,
        IStoreService storeService,
        IWorkflowMessageService workflowMessageService)
    {
        _messageTemplateService = messageTemplateService;
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
        _settingService = settingService;
        _storeService = storeService;
        _workflowMessageService = workflowMessageService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
        foreach (var store in await _storeService.GetAllStoresAsync())
        {
            var reminderSettings = await _settingService.LoadSettingAsync<ReminderSettings>(store.Id);
            if (!reminderSettings.PendingOrdersEnabled)
                continue;

            //get message templates
            var messageTemplates = await NopReminderDefaults.PendingOrders.FollowUpList
                .SelectManyAwait(async followUp => await _messageTemplateService.GetMessageTemplatesByNameAsync(followUp, store.Id))
                .Where(template => template.IsActive && template.DelayBeforeSend is not null)
                .ToListAsync();

            if (!messageTemplates.Any())
                continue;

            var followUps = messageTemplates
                .DistinctBy(template => template.Name)
                .ToDictionary(
                    mt => mt.Name,
                    mt => new
                    {
                        FollowUp = mt,
                        Date = DateTime.UtcNow - TimeSpan.FromHours(mt.DelayPeriod.ToHours(mt.DelayBeforeSend.Value))
                    });

            var pendingOrders =
                from order in _orderRepository.Table
                join c in _customerRepository.Table on order.CustomerId equals c.Id
                where
                    !c.Deleted && !string.IsNullOrEmpty(c.Email)
                    && !order.Deleted && order.OrderStatusId != (int)OrderStatus.Cancelled && order.PaymentStatusId == (int)PaymentStatus.Pending
                    && (reminderSettings.ProcessingStartDateUtc == null || order.CreatedOnUtc >= reminderSettings.ProcessingStartDateUtc)
                    && order.StoreId == store.Id && order.LastPendingOrderFollowUpNumber != 2
                    && (
                        ((order.LastPendingOrderFollowUpNumber == null || order.LastPendingOrderFollowUpNumber == 0) && order.CreatedOnUtc < followUps[MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_1_MESSAGE].Date)
                        || (order.LastPendingOrderFollowUpNumber == 1 && order.LastPendingOrderFollowUpDateUtc < followUps[MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_2_MESSAGE].Date)
                    )
                orderby order.CreatedOnUtc
                select new { Customer = c, Order = order };

            var skip = 0;
            while (true)
            {
                var pendingOrdersPaged = pendingOrders.Skip(skip).Take(NopReminderDefaults.ProcessingBatchSize).ToList();
                if (!pendingOrdersPaged.Any())
                    break;

                var customersWithOrders = pendingOrdersPaged
                    .GroupBy(customerItem => customerItem.Customer.Id)
                    .Select(group => new
                    {
                        Customer = group.Select(item => item.Customer).FirstOrDefault(customer => customer.Id == group.Key),
                        Orders = group.Select(item => item.Order).DistinctBy(order => order.Id).OrderBy(order => order.CreatedOnUtc).ToList()
                    })
                    .Where(customer => customer.Orders.Any())
                    .ToList();

                var followedUpOrders = new List<Order>();
                try
                {
                    foreach (var customerInfo in customersWithOrders)
                    {
                        var customer = customerInfo.Customer;

                        var order = customerInfo.Orders.FirstOrDefault(o => o.LastPendingOrderFollowUpDateUtc != null)
                            ?? customerInfo.Orders.FirstOrDefault();

                        var nextFollowUpNumber = (order.LastPendingOrderFollowUpNumber ?? 0) + 1;

                        var followUpMessage = messageTemplates
                            .FirstOrDefault(template => template.Name == NopReminderDefaults.PendingOrders.FollowUpList[nextFollowUpNumber - 1]);

                        if (followUpMessage is null)
                            continue;

                        var emailIds = await _workflowMessageService.SendPendingOrderFollowUpCustomerNotificationAsync(customer, order, followUpMessage.Name);

                        //the follow up has been sent
                        if (emailIds.Any())
                        {
                            order.LastPendingOrderFollowUpNumber = nextFollowUpNumber;
                            order.LastPendingOrderFollowUpDateUtc = DateTime.UtcNow;

                            followedUpOrders.Add(order);
                        }
                    }
                }
                finally
                {
                    await _orderRepository.UpdateAsync(followedUpOrders);
                }

                if (pendingOrdersPaged.Count < NopReminderDefaults.ProcessingBatchSize)
                    break;

                skip += NopReminderDefaults.ProcessingBatchSize;
            }
        }
    }

    #endregion
}