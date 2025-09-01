using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Messages;

namespace Nop.Services.Customers;

/// <summary>
/// Represents a customer event consumer
/// </summary>
public partial class CustomerEventConsumer : IConsumer<CustomerChangeWorkingLanguageEvent>,
    IConsumer<EntityDeletedEvent<ShoppingCartItem>>,
    IConsumer<EntityInsertedEvent<ShoppingCartItem>>,
    IConsumer<EntityUpdatedEvent<ShoppingCartItem>>
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public CustomerEventConsumer(ICustomerService customerService,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        IStoreContext storeContext)
    {
        _customerService = customerService;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _storeContext = storeContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Updates the customer's last shopping cart activity date and time (UTC).
    /// </summary>
    /// <param name="customerId">The customer identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task UpdateShoppingCartLastActivityDateAsync(int customerId)
    {
        if (customerId < 0)
            return;

        var customer = await _customerService.GetCustomerByIdAsync(customerId);

        customer.LastAbandonedCartFollowUpNumber = null;
        customer.LastShoppingCartUpdateDateUtc = DateTime.UtcNow;

        await _customerService.UpdateCustomerAsync(customer);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle working language changed event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(CustomerChangeWorkingLanguageEvent eventMessage)
    {
        if (eventMessage.Customer is not Customer customer)
            return;

        if (await _customerService.IsGuestAsync(customer))
            return;

        //change language for all customer subscriptions
        var store = await _storeContext.GetCurrentStoreAsync();
        var subscriptions = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByEmailAsync(customer.Email, storeId: store.Id);
        foreach (var subscription in subscriptions)
        {
            if (subscription.LanguageId == customer.LanguageId)
                continue;

            subscription.LanguageId = customer.LanguageId ?? 0;
            await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
        }
    }

    #region Shopping cart

    /// <summary>
    /// Handle the delete shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityDeletedEvent<ShoppingCartItem> eventMessage)
    {
        await UpdateShoppingCartLastActivityDateAsync(eventMessage.Entity.CustomerId);
    }

    /// <summary>
    /// Handle the insert shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
    {
        await UpdateShoppingCartLastActivityDateAsync(eventMessage.Entity.CustomerId);
    }

    /// <summary>
    /// Handle the update shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
    {
        await UpdateShoppingCartLastActivityDateAsync(eventMessage.Entity.CustomerId);
    }

    #endregion

    #endregion
}
