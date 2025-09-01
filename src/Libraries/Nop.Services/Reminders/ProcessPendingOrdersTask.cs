using Nop.Core.Domain.Common;
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
    protected readonly IRepository<GenericAttribute> _genericAttributeRepository;
    protected readonly IRepository<Order> _orderRepository;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly ReminderSettings _reminderSettings;

    #endregion

    #region Ctor

    public ProcessPendingOrdersTask(ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IMessageTemplateService messageTemplateService,
        IRepository<Customer> customerRepository,
        IRepository<CustomerCustomerRoleMapping> customerCustomerRoleMappingRepository,
        IRepository<GenericAttribute> genericAttributeRepository,
        IRepository<Order> orderRepository,
        IWorkflowMessageService workflowMessageService,
        ReminderSettings reminderSettings)
    {
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _messageTemplateService = messageTemplateService;
        _customerRepository = customerRepository;
        _customerCustomerRoleMappingRepository = customerCustomerRoleMappingRepository;
        _genericAttributeRepository = genericAttributeRepository;
        _orderRepository = orderRepository;
        _workflowMessageService = workflowMessageService;
        _reminderSettings = reminderSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
        if (!_reminderSettings.PendingOrdersEnabled)
            return;

        var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
        var attributeName = NopReminderDefaults.PendingOrders.FollowUpAttributeName;

        //get message templates
        var messageTemplates = await NopReminderDefaults.PendingOrders.FollowUpList
            .SelectManyAwait(async followUp => await _messageTemplateService.GetMessageTemplatesByNameAsync(followUp))
            .Where(template => template.IsActive && template.DelayBeforeSend is not null)
            .ToListAsync();

        var timeToFirstFollowUp = messageTemplates
            .Select(template => DateTime.UtcNow - TimeSpan.FromHours(template.DelayPeriod.ToHours(template.DelayBeforeSend.Value)))
            .OrderDescending()
            .FirstOrDefault();

        if (timeToFirstFollowUp == default)
            return;

        var pendingOrders = (from order in _orderRepository.Table
                             join customer in _customerRepository.Table on order.CustomerId equals customer.Id
                             join attribute in _genericAttributeRepository.Table on
                                 new { EntityId = order.Id, KeyGroup = nameof(Order), Key = attributeName } equals
                                 new { attribute.EntityId, attribute.KeyGroup, attribute.Key } into attribute
                             from attributeJoined in attribute.DefaultIfEmpty()
                             join ccrm in _customerCustomerRoleMappingRepository.Table on
                                 new { customerId = customer.Id, roleId = registeredRole.Id } equals
                                 new { customerId = ccrm.CustomerId, roleId = ccrm.CustomerRoleId }
                             where !customer.Deleted && !order.Deleted && order.OrderStatusId == (int)OrderStatus.Pending
                             && (
                                 (attributeJoined != null && int.Parse(attributeJoined.Value) < NopReminderDefaults.PendingOrders.FollowUpList.Length) ||
                                 (attributeJoined == null && order.CreatedOnUtc < timeToFirstFollowUp)
                             )
                             orderby order.CreatedOnUtc
                             select new { Customer = customer, Order = order, Attribute = attributeJoined }).ToList();

        if (!pendingOrders.Any())
            return;

        var customersWithOrders = pendingOrders
            .GroupBy(customerItem => customerItem.Customer.Id)
            .Select(group => new
            {
                Customer = group.Select(item => item.Customer).FirstOrDefault(customer => customer.Id == group.Key),
                Orders = group.Select(item => item.Order).DistinctBy(order => order.Id).OrderBy(order => order.CreatedOnUtc).ToList()
            })
            .Where(customer => customer.Orders.Any())
            .ToList();

        if (!customersWithOrders.Any())
            return;

        //prepare attributes
        var attributes = pendingOrders
            .Select(item => item.Attribute)
            .Where(attribute => attribute is not null)
            .DistinctBy(attribute => attribute.Id)
            .ToList();

        foreach (var customer in customersWithOrders)
        {
            foreach (var order in customer.Orders)
            {
                var orderAttribute = attributes.FirstOrDefault(attribute => attribute.EntityId == order.Id);
                MessageTemplate followUpMessage = null;
                _ = int.TryParse(orderAttribute?.Value ?? string.Empty, out var lastFollowUp);

                while (followUpMessage is null && lastFollowUp < NopReminderDefaults.PendingOrders.FollowUpList.Length)
                {
                    lastFollowUp++;
                    followUpMessage = messageTemplates
                        .FirstOrDefault(template => template.Name == NopReminderDefaults.PendingOrders.FollowUpList[lastFollowUp - 1]);
                }
                if (followUpMessage is null)
                    continue;

                var timeToFollowUp = DateTime.UtcNow - TimeSpan.FromHours(followUpMessage.DelayPeriod.ToHours(followUpMessage.DelayBeforeSend.Value));

                //check the order created date or the last follow up sent date
                var date = order.CreatedOnUtc;
                if (orderAttribute?.CreatedOrUpdatedDateUTC is not null && orderAttribute.CreatedOrUpdatedDateUTC.Value > date)
                    date = orderAttribute.CreatedOrUpdatedDateUTC.Value;
                if (date > timeToFollowUp)
                    continue;

                var emailIds = await _workflowMessageService
                    .SendPendingOrderFollowUpCustomerNotificationAsync(customer.Customer, order, followUpMessage.Name);

                //the follow up has been sent
                if (emailIds.Any())
                {
                    await _genericAttributeService.SaveAttributeAsync(order, attributeName, lastFollowUp);

                    //send only one follow up to a customer at a time to avoid being annoying
                    break;
                }
            }
        }
    }

    #endregion
}