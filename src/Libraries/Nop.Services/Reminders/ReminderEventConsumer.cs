using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;

namespace Nop.Services.Reminders;

/// <summary>
/// Represents a reminder event consumer
/// </summary>
public partial class ReminderEventConsumer :
    IConsumer<EntityDeletedEvent<ShoppingCartItem>>,
    IConsumer<EntityInsertedEvent<ShoppingCartItem>>,
    IConsumer<EntityUpdatedEvent<ShoppingCartItem>>,
    IConsumer<CustomerActivatedEvent>,
    IConsumer<OrderStatusChangedEvent>
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;

    #endregion

    #region Ctor

    public ReminderEventConsumer(ICustomerService customerService,
        IGenericAttributeService genericAttributeService)
    {
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Remove attribute
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entity">Entity entry</param>
    /// <param name="attributeName">Attribute name</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task RemoveFollowUpAttributeAsync<TEntity>(TEntity entity, string attributeName, int storeId = 0) where TEntity : BaseEntity
    {
        if (entity is null)
            return;

        await _genericAttributeService.SaveAttributeAsync<int?>(entity, attributeName, null, storeId);
    }

    #endregion

    #region Shopping cart

    /// <summary>
    /// Handle the delete shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityDeletedEvent<ShoppingCartItem> eventMessage)
    {
        var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Entity.CustomerId);
        await RemoveFollowUpAttributeAsync(customer, NopReminderDefaults.AbandonedCarts.FollowUpAttributeName, eventMessage.Entity.StoreId);
    }

    /// <summary>
    /// Handle the insert shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
    {
        var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Entity.CustomerId);
        await RemoveFollowUpAttributeAsync(customer, NopReminderDefaults.AbandonedCarts.FollowUpAttributeName, eventMessage.Entity.StoreId);
    }

    /// <summary>
    /// Handle the update shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
    {
        var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Entity.CustomerId);
        await RemoveFollowUpAttributeAsync(customer, NopReminderDefaults.AbandonedCarts.FollowUpAttributeName, eventMessage.Entity.StoreId);
    }

    #endregion

    #region Registration

    /// <summary>
    /// Handle customer activated event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(CustomerActivatedEvent eventMessage)
    {
        var customer = eventMessage?.Customer;
        await RemoveFollowUpAttributeAsync(customer, NopReminderDefaults.IncompleteRegistrations.FollowUpAttributeName, customer?.RegisteredInStoreId ?? 0);
    }

    #endregion

    #region Orders

    /// <summary>
    /// Handle change order status event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(OrderStatusChangedEvent eventMessage)
    {
        if (eventMessage?.Order is null)
            return;

        if (eventMessage.Order.OrderStatus != OrderStatus.Complete && eventMessage.Order.OrderStatus != OrderStatus.Processing)
            return;

        await RemoveFollowUpAttributeAsync(eventMessage.Order, NopReminderDefaults.PendingOrders.FollowUpAttributeName);
    }

    #endregion
}