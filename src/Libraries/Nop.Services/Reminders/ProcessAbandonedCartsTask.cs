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
using Nop.Services.Stores;

namespace Nop.Services.Reminders;

/// <summary>
/// Represents a task to process abandoned carts 
/// </summary>
public partial class ProcessAbandonedCartsTask : IScheduleTask
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<CustomerCustomerRoleMapping> _customerCustomerRoleMappingRepository;
    protected readonly IRepository<GenericAttribute> _genericAttributeRepository;
    protected readonly IRepository<ShoppingCartItem> _shoppingCartRepository;
    protected readonly IStoreService _storeService;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly ReminderSettings _reminderSettings;
    protected readonly ShoppingCartSettings _shoppingCartSettings;

    #endregion

    #region Ctor

    public ProcessAbandonedCartsTask(ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IMessageTemplateService messageTemplateService,
        IRepository<Customer> customerRepository,
        IRepository<CustomerCustomerRoleMapping> customerCustomerRoleMappingRepository,
        IRepository<GenericAttribute> genericAttributeRepository,
        IRepository<ShoppingCartItem> shoppingCartRepository,
        IStoreService storeService,
        IWorkflowMessageService workflowMessageService,
        ReminderSettings reminderSettings,
        ShoppingCartSettings shoppingCartSettings)
    {
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _messageTemplateService = messageTemplateService;
        _customerRepository = customerRepository;
        _customerCustomerRoleMappingRepository = customerCustomerRoleMappingRepository;
        _genericAttributeRepository = genericAttributeRepository;
        _shoppingCartRepository = shoppingCartRepository;
        _storeService = storeService;
        _workflowMessageService = workflowMessageService;
        _reminderSettings = reminderSettings;
        _shoppingCartSettings = shoppingCartSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
        if (!_reminderSettings.AbandonedCartEnabled)
            return;

        var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
        var attributeName = NopReminderDefaults.AbandonedCarts.FollowUpAttributeName;

        //get registered customers with abandoned carts
        var customersWithItems = _shoppingCartRepository.Table
            .Join(_customerRepository.Table, item => item.CustomerId, customer => customer.Id,
                (item, customer) => new { Customer = customer, Item = item })
            .Join(_customerCustomerRoleMappingRepository.Table, item => new { customerId = item.Customer.Id, roleId = registeredRole.Id }, role => new { customerId = role.CustomerId, roleId = role.CustomerRoleId },
                (item, role) => new { Customer = item.Customer, Item = item.Item, Role = role })
            .SelectMany(item => _genericAttributeRepository.Table
                .Where(attribute => attribute.EntityId == item.Customer.Id && attribute.KeyGroup == nameof(Customer) && attribute.Key == attributeName)
                .DefaultIfEmpty(),
                (item, attribute) => new { Customer = item.Customer, Item = item.Item, Role = item.Role, Attribute = attribute })
            .Where(item => !item.Customer.Deleted)
            .Where(item => item.Item.ShoppingCartTypeId == (int)ShoppingCartType.ShoppingCart)
            .Select(item => new { Customer = item.Customer, Item = item.Item, Attribute = item.Attribute })
            .ToList();

        if (!customersWithItems.Any())
            return;

        foreach (var store in await _storeService.GetAllStoresAsync())
        {
            //get customers with abandoned carts for the current store
            var customersWithCart = customersWithItems
                .GroupBy(customerItem => customerItem.Customer.Id)
                .Select(group => new
                {
                    Customer = group.Select(item => item.Customer).FirstOrDefault(customer => customer.Id == group.Key),
                    Attribute = group.Select(item => item.Attribute).FirstOrDefault(attribute => attribute?.StoreId == store.Id),
                    Cart = group
                        .Select(item => item.Item)
                        .Where(item => _shoppingCartSettings.CartsSharedBetweenStores || item.StoreId == store.Id)
                        .DistinctBy(item => item.Id)
                        .ToList()
                })
                .Where(customer => customer.Cart.Any())
                .ToList();

            if (!customersWithCart.Any())
                continue;

            //get message templates
            var messageTemplates = await NopReminderDefaults.AbandonedCarts.FollowUpList
                .SelectManyAwait(async followUp => await _messageTemplateService.GetMessageTemplatesByNameAsync(followUp, store.Id))
                .ToListAsync();

            foreach (var customer in customersWithCart)
            {
                MessageTemplate followUpMessage = null;
                _ = int.TryParse(customer.Attribute?.Value ?? string.Empty, out var lastFollowUp);

                while (followUpMessage is null && lastFollowUp < NopReminderDefaults.AbandonedCarts.FollowUpList.Length)
                {
                    lastFollowUp++;
                    followUpMessage = messageTemplates
                        .FirstOrDefault(template => template.Name == NopReminderDefaults.AbandonedCarts.FollowUpList[lastFollowUp - 1]
                            && template.IsActive
                            && template.DelayBeforeSend is not null);
                }
                if (followUpMessage is null)
                    continue;

                var timeToFollowUp = DateTime.UtcNow - TimeSpan.FromHours(followUpMessage.DelayPeriod.ToHours(followUpMessage.DelayBeforeSend.Value));

                //check the last updated date or the last follow up sent date
                var lastUpdatedDate = customer.Cart.Max(item => item.UpdatedOnUtc);
                if (customer.Attribute?.CreatedOrUpdatedDateUTC is not null && customer.Attribute.CreatedOrUpdatedDateUTC.Value > lastUpdatedDate)
                    lastUpdatedDate = customer.Attribute.CreatedOrUpdatedDateUTC.Value;
                if (lastUpdatedDate > timeToFollowUp)
                    continue;

                var emailIds = await _workflowMessageService
                    .SendAbandonedCartFollowUpCustomerNotificationAsync(customer.Customer, customer.Cart, store.Id, followUpMessage.Name);

                //the follow up has been sent
                if (emailIds.Any())
                    await _genericAttributeService.SaveAttributeAsync(customer.Customer, attributeName, lastFollowUp, store.Id);
            }
        }
    }

    #endregion
}