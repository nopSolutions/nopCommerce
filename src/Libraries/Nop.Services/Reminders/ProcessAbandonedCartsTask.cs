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

        var followUpDates = messageTemplates
            .DistinctBy(template => template.Name)
            .ToDictionary(
                mt => mt.Name,
                mt => DateTime.UtcNow - TimeSpan.FromHours(mt.DelayPeriod.ToHours(mt.DelayBeforeSend.Value)));

        //inactive follow-ups have no date, so customers waiting for them are not selected
        var followUp1Date = followUpDates.TryGetValue(MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_1_MESSAGE, out var date1) ? date1 : (DateTime?)null;
        var followUp2Date = followUpDates.TryGetValue(MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_2_MESSAGE, out var date2) ? date2 : (DateTime?)null;
        var followUp3Date = followUpDates.TryGetValue(MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_3_MESSAGE, out var date3) ? date3 : (DateTime?)null;

        //get registered customers with abandoned carts
        var customersWithItems =
            from c in _customerRepository.Table
            join cartItem in _shoppingCartRepository.Table on c.Id equals cartItem.CustomerId
            where !c.Deleted && !string.IsNullOrEmpty(c.Email)
                && c.LastShoppingCartUpdateDateUtc != null
                && (_reminderSettings.ProcessingStartDateUtc == null || c.LastShoppingCartUpdateDateUtc >= _reminderSettings.ProcessingStartDateUtc)
                && (
                    ((c.LastAbandonedCartFollowUpNumber == null || c.LastAbandonedCartFollowUpNumber == 0) && followUp1Date != null && c.LastShoppingCartUpdateDateUtc < followUp1Date)
                    || (c.LastAbandonedCartFollowUpNumber == 1 && followUp2Date != null && c.LastAbandonedCartFollowUpDateUtc < followUp2Date)
                    || (c.LastAbandonedCartFollowUpNumber == 2 && followUp3Date != null && c.LastAbandonedCartFollowUpDateUtc < followUp3Date)
                )
                && (cartItem.ShoppingCartTypeId == (int)ShoppingCartType.ShoppingCart || cartItem.ShoppingCartTypeId == (int)ShoppingCartType.Stash)
            select new { Customer = c, CartItem = cartItem };

        //page by customer identifier rather than by skipping rows: followed up customers drop out of the query,
        //so skipping would pass over unprocessed ones, and a customer's cart must not be split across batches
        var lastCustomerId = 0;
        while (true)
        {
            var customerIds = customersWithItems
                .Select(item => item.Customer.Id)
                .Where(customerId => customerId > lastCustomerId)
                .Distinct()
                .OrderBy(customerId => customerId)
                .Take(NopReminderDefaults.ProcessingBatchSize)
                .ToList();

            if (!customerIds.Any())
                break;

            lastCustomerId = customerIds.Last();

            var customersWithItemsPaged = customersWithItems.Where(item => customerIds.Contains(item.Customer.Id)).ToList();

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

            if (customerIds.Count < NopReminderDefaults.ProcessingBatchSize)
                break;
        }
    }

    #endregion
}