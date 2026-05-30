using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Reminders;
using Nop.Data;
using Nop.Services.Messages;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Reminders;

/// <summary>
/// Represents a task to process abandoned carts 
/// </summary>
public partial class ProcessAbandonedCartsTask : IScheduleTask
{
    #region Fields

    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<ShoppingCartItem> _shoppingCartRepository;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly ReminderSettings _reminderSettings;

    #endregion

    #region Ctor

    public ProcessAbandonedCartsTask(IMessageTemplateService messageTemplateService,
        IRepository<Customer> customerRepository,
        IRepository<ShoppingCartItem> shoppingCartRepository,
        IWorkflowMessageService workflowMessageService,
        ReminderSettings reminderSettings)
    {
        _messageTemplateService = messageTemplateService;
        _customerRepository = customerRepository;
        _shoppingCartRepository = shoppingCartRepository;
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
        if (!_reminderSettings.AbandonedCartEnabled)
            return;

        var messageTemplates = new LinkedList<MessageTemplate>(await NopReminderDefaults.AbandonedCarts.FollowUpList
            .SelectManyAwait(async followUp => await _messageTemplateService.GetMessageTemplatesByNameAsync(followUp))
            .Where(template => template.IsActive && template.DelayBeforeSend is not null)
            .ToListAsync());

        if (!messageTemplates.Any())
            return;

        var followUps = messageTemplates
            .DistinctBy(template => template.Name)
            .ToDictionary(
                mt => mt.Name,
                mt => new
                {
                    FollowUp = mt,
                    Date = DateTime.UtcNow - TimeSpan.FromHours(mt.DelayPeriod.ToHours(mt.DelayBeforeSend.Value))
                });

        //get registered customers with abandoned carts
        var customersWithItems =
            from c in _customerRepository.Table
            join cartItem in _shoppingCartRepository.Table on c.Id equals cartItem.CustomerId
            where !c.Deleted && !string.IsNullOrEmpty(c.Email)
                && c.HasShoppingCartItems && c.LastShoppingCartUpdateDateUtc != null
                && (_reminderSettings.ProcessingStartDateUtc == null || c.LastShoppingCartUpdateDateUtc >= _reminderSettings.ProcessingStartDateUtc)
                && (
                    ((c.LastAbandonedCartFollowUpNumber == null || c.LastAbandonedCartFollowUpNumber == 0) && c.LastShoppingCartUpdateDateUtc < followUps[MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_1_MESSAGE].Date)
                    || (c.LastAbandonedCartFollowUpNumber == 1 && c.LastAbandonedCartFollowUpDateUtc < followUps[MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_2_MESSAGE].Date)
                    || (c.LastAbandonedCartFollowUpNumber == 2 && c.LastAbandonedCartFollowUpDateUtc < followUps[MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_3_MESSAGE].Date)
                )
                && cartItem.ShoppingCartTypeId == (int)ShoppingCartType.ShoppingCart
            select new { Customer = c, CartItem = cartItem };

        var skip = 0;
        while (true)
        {
            var customersWithItemsPaged = customersWithItems.Skip(skip).Take(NopReminderDefaults.ProcessingBatchSize).ToList();
            if (!customersWithItemsPaged.Any())
                break;

            //get customers with abandoned carts for the current store
            var customersWithCart = customersWithItemsPaged
                .GroupBy(customerItem => customerItem.Customer.Id)
                .Select(group => new
                {
                    Customer = group.Select(item => item.Customer).FirstOrDefault(customer => customer.Id == group.Key),
                    Cart = group
                        .Select(item => item.CartItem)
                        .DistinctBy(item => item.Id)
                        .ToList()
                })
                .Where(customer => customer.Cart.Any())
                .ToList();

            var followedUpCustomers = new List<Customer>();
            try
            {
                foreach (var customerInfo in customersWithCart)
                {
                    var customer = customerInfo.Customer;
                    var nextFollowUpNumber = (customer.LastAbandonedCartFollowUpNumber ?? 0) + 1;

                    var followUpMessage = messageTemplates
                        .FirstOrDefault(template => template.Name == NopReminderDefaults.AbandonedCarts.FollowUpList[nextFollowUpNumber - 1]);

                    if (followUpMessage is null)
                        continue;

                    var emailIds = await _workflowMessageService.SendAbandonedCartFollowUpCustomerNotificationAsync(customer, customerInfo.Cart, followUpMessage.Name);

                    //the follow up has been sent
                    if (emailIds.Any())
                    {
                        customer.LastAbandonedCartFollowUpNumber = nextFollowUpNumber;
                        customer.LastAbandonedCartFollowUpDateUtc = DateTime.UtcNow;
                        followedUpCustomers.Add(customer);
                    }
                }
            }
            finally
            {
                await _customerRepository.UpdateAsync(followedUpCustomers);
            }

            if (customersWithItemsPaged.Count < NopReminderDefaults.ProcessingBatchSize)
                break;

            skip += NopReminderDefaults.ProcessingBatchSize;
        }
    }

    #endregion
}