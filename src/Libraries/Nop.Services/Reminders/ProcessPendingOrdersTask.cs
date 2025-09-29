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

        //get registered customers with pending orders
        var pendingOrders = _orderRepository.Table
            .Join(_customerRepository.Table, order => order.CustomerId, customer => customer.Id,
                (order, customer) => new { Customer = customer, Order = order })
            .Join(_customerCustomerRoleMappingRepository.Table, item => new { customerId = item.Customer.Id, roleId = registeredRole.Id }, role => new { customerId = role.CustomerId, roleId = role.CustomerRoleId },
                (item, role) => new { Customer = item.Customer, Order = item.Order, Role = role })
            .SelectMany(item => _genericAttributeRepository.Table
                .Where(attribute => attribute.EntityId == item.Order.Id && attribute.KeyGroup == nameof(Order) && attribute.Key == attributeName)
                .DefaultIfEmpty(),
                (item, attribute) => new { Customer = item.Customer, Order = item.Order, Role = item.Role, Attribute = attribute })
            .Where(item => !item.Customer.Deleted)
            .Where(item => !item.Order.Deleted && item.Order.OrderStatusId == (int)OrderStatus.Pending)
            .Select(item => new { Customer = item.Customer, Order = item.Order, Attribute = item.Attribute })
            .OrderBy(item => item.Order.CreatedOnUtc)
            .ToList();

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

        //get message templates
        var messageTemplates = await NopReminderDefaults.PendingOrders.FollowUpList
            .SelectManyAwait(async followUp => await _messageTemplateService.GetMessageTemplatesByNameAsync(followUp))
            .ToListAsync();

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
                        .FirstOrDefault(template => template.Name == NopReminderDefaults.PendingOrders.FollowUpList[lastFollowUp - 1]
                            && template.IsActive
                            && template.DelayBeforeSend is not null);
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