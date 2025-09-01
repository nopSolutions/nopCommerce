using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Reminders;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Reminders;

/// <summary>
/// Represents a task to process incomplete orders 
/// </summary>
public partial class ProcessPendingOrdersTask : IScheduleTask
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<CustomerCustomerRoleMapping> _customerCustomerRoleMappingRepository;
    protected readonly IRepository<Order> _orderRepository;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly RemindersSettings _remindersSettings;

    #endregion

    #region Ctor

    public ProcessPendingOrdersTask(ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IMessageTemplateService messageTemplateService,
        IRepository<Customer> customerRepository,
        IRepository<CustomerCustomerRoleMapping> customerCustomerRoleMappingRepository,
        IRepository<Order> orderRepository,
        IWorkflowMessageService workflowMessageService,
        RemindersSettings remindersSettings)
    {
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _messageTemplateService = messageTemplateService;
        _customerRepository = customerRepository;
        _customerCustomerRoleMappingRepository = customerCustomerRoleMappingRepository;
        _orderRepository = orderRepository;
        _workflowMessageService = workflowMessageService;
        _remindersSettings = remindersSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
        if (!_remindersSettings.PendingOrdersEnabled)
            return;

        var guestRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName);
        var followUpCount = RemindersDefaults.PendingOrders.FollowUpList.Length;

        for (var followUpNumber = 1; followUpNumber <= followUpCount; followUpNumber++)
        {
            var followUpMessageName = RemindersDefaults.PendingOrders.FollowUpList[followUpNumber - 1];
            var followUpMessage = (await _messageTemplateService.GetMessageTemplatesByNameAsync(followUpMessageName))
                .FirstOrDefault();

            if (followUpMessage.DelayBeforeSend is null)
                continue;

            var timeToFollowUp = DateTime.UtcNow - TimeSpan.FromHours(followUpMessage.DelayPeriod.ToHours(followUpMessage.DelayBeforeSend.Value));

            //get all incomplete orders to follow up
            var incompleteOrdersInfo = from c in _customerRepository.Table
                                       join ccrm in _customerCustomerRoleMappingRepository.Table on c.Id equals ccrm.CustomerId
                                       join o in _orderRepository.Table on c.Id equals o.CustomerId
                                       orderby o.CreatedOnUtc
                                       where !c.Deleted && !o.Deleted
                                            && ccrm.CustomerRoleId != guestRole.Id
                                            && timeToFollowUp >= o.CreatedOnUtc
                                            && new int[] { (int)OrderStatus.Pending }.Contains(o.OrderStatusId)
                                       group o by c into orderGrouped
                                       select new { Customer = orderGrouped.Key, Orders = orderGrouped.Where(og => og.CreatedOnUtc <= timeToFollowUp) };

            if (!incompleteOrdersInfo.Any())
                return;

            foreach (var customerOrders in incompleteOrdersInfo)
            {
                var followUps = await _genericAttributeService.GetAttributeAsync(customerOrders.Customer, RemindersDefaults.PendingOrders.FollowUpAttributeName, defaultValue: new Dictionary<int, int>());
                var orderToFollowUp = customerOrders.Orders.FirstOrDefault(x => followUps.GetValueOrDefault(x.Id) < followUpNumber);

                if (orderToFollowUp is not null)
                {
                    var emailIds = await _workflowMessageService.SendPendingOrderFollowUpCustomerNotificationAsync(customerOrders.Customer, orderToFollowUp, followUpMessageName);

                    if (emailIds.Any())
                    {
                        if (!followUps.TryAdd(orderToFollowUp.Id, followUpNumber))
                            followUps[orderToFollowUp.Id] = followUpNumber;

                        await _genericAttributeService.SaveAttributeAsync(customerOrders.Customer, RemindersDefaults.PendingOrders.FollowUpAttributeName, followUps);
                    }
                }
            }
        }
    }

    #endregion
}
