using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Reminders;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Services.Orders;
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
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreService _storeService;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly RemindersSettings _remindersSettings;

    #endregion

    #region Ctor

    public ProcessAbandonedCartsTask(ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IMessageTemplateService messageTemplateService,
        IShoppingCartService shoppingCartService,
        IStoreService storeService,
        IWorkflowMessageService workflowMessageService,
        RemindersSettings remindersSettings)
    {
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _messageTemplateService = messageTemplateService;
        _shoppingCartService = shoppingCartService;
        _storeService = storeService;
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
        if (!_remindersSettings.AbandonedCartEnabled)
            return;

        foreach (var store in await _storeService.GetAllStoresAsync())
        {
            //get registered customers with abandoned carts
            var customersWithCart = await (await _customerService
                .GetCustomersWithShoppingCartsAsync(ShoppingCartType.ShoppingCart, storeId: store.Id))
                .WhereAwait(async customer => await _customerService.IsRegisteredAsync(customer))
                .ToListAsync();

            for (var followUpNumber = 1; followUpNumber <= RemindersDefaults.AbandonedCarts.FollowUpList.Length; followUpNumber++)
            {
                var followUpMessageName = RemindersDefaults.AbandonedCarts.FollowUpList[followUpNumber - 1];
                var followUpMessage = (await _messageTemplateService.GetMessageTemplatesByNameAsync(followUpMessageName))
                    .FirstOrDefault();

                if (followUpMessage is null || !followUpMessage.IsActive)
                    continue;

                if (followUpMessage.DelayBeforeSend is null)
                    continue;

                var timeToFollowUp = DateTime.UtcNow - TimeSpan.FromHours(followUpMessage.DelayPeriod.ToHours(followUpMessage.DelayBeforeSend.Value));

                foreach (var customer in customersWithCart)
                {
                    var lastFollowUp = await _genericAttributeService.GetAttributeAsync<int>(customer, RemindersDefaults.AbandonedCarts.FollowUpAttributeName);

                    if (lastFollowUp >= followUpNumber)
                        continue;

                    var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart);
                    if (!cart.Any())
                        continue;

                    if (cart.Max(ci => ci.CreatedOnUtc) < timeToFollowUp)
                    {
                        var emailIds = await _workflowMessageService.SendAbandonedCartFollowUpCustomerNotificationAsync(customer, cart, followUpMessageName);

                        if (emailIds.Any())
                            await _genericAttributeService.SaveAttributeAsync(customer, RemindersDefaults.AbandonedCarts.FollowUpAttributeName, followUpNumber);
                    }
                }
            }
        }
    }

    #endregion
}
