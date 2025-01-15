using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Messages;

namespace Nop.Plugin.Misc.Brevo.Services;

/// <summary>
/// Represents event consumer
/// </summary>
public class EventConsumer :
    IConsumer<EmailUnsubscribedEvent>,
    IConsumer<EmailSubscribedEvent>,
    IConsumer<EntityInsertedEvent<ShoppingCartItem>>,
    IConsumer<EntityUpdatedEvent<ShoppingCartItem>>,
    IConsumer<EntityDeletedEvent<ShoppingCartItem>>,
    IConsumer<OrderPaidEvent>,
    IConsumer<OrderPlacedEvent>,
    IConsumer<EntityTokensAddedEvent<Store, Token>>,
    IConsumer<EntityTokensAddedEvent<Customer, Token>>
{
    #region Fields

    protected readonly BrevoManager _brevoEmailManager;
    protected readonly BrevoSettings _brevoSettings;
    protected readonly MarketingAutomationManager _marketingAutomationManager;

    #endregion

    #region Ctor

    public EventConsumer(BrevoManager brevoEmailManager,
        BrevoSettings brevoSettings,
        MarketingAutomationManager marketingAutomationManager)
    {
        _brevoEmailManager = brevoEmailManager;
        _brevoSettings = brevoSettings;
        _marketingAutomationManager = marketingAutomationManager;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle the email unsubscribed event.
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EmailUnsubscribedEvent eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return;

        //unsubscribe contact
        await _brevoEmailManager.UnsubscribeAsync(eventMessage.Subscription);
    }

    /// <summary>
    /// Handle the email subscribed event.
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EmailSubscribedEvent eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return;

        //subscribe contact
        await _brevoEmailManager.SubscribeAsync(eventMessage.Subscription);
    }

    /// <summary>
    /// Handle the add shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return;

        //handle event
        await _marketingAutomationManager.HandleShoppingCartChangedEventAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle the update shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return;

        //handle event
        await _marketingAutomationManager.HandleShoppingCartChangedEventAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle the delete shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<ShoppingCartItem> eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return;

        //handle event
        await _marketingAutomationManager.HandleShoppingCartChangedEventAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle the order paid event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPaidEvent eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return;

        //handle event
        await _marketingAutomationManager.HandleOrderCompletedEventAsync(eventMessage.Order);
        await _brevoEmailManager.UpdateContactAfterCompletingOrderAsync(eventMessage.Order);
    }

    /// <summary>
    /// Handle the order placed event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return;

        //handle event
        await _marketingAutomationManager.HandleOrderPlacedEventAsync(eventMessage.Order);
    }

    /// <summary>
    /// Handle the store tokens added event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task HandleEventAsync(EntityTokensAddedEvent<Store, Token> eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return Task.CompletedTask;

        //handle event
        eventMessage.Tokens.Add(new Token("Store.Id", eventMessage.Entity.Id));

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handle the customer tokens added event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task HandleEventAsync(EntityTokensAddedEvent<Customer, Token> eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return Task.CompletedTask;

        //handle event
        eventMessage.Tokens.Add(new Token("Customer.PhoneNumber", eventMessage.Entity.Phone));

        return Task.CompletedTask;
    }

    #endregion
}