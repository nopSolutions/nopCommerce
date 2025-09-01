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
public partial class RemindersEventConsumer :
    IConsumer<EntityDeletedEvent<ShoppingCartItem>>,
    IConsumer<EntityInsertedEvent<ShoppingCartItem>>,
    IConsumer<EntityUpdatedEvent<ShoppingCartItem>>,
    IConsumer<CustomerRegisteredEvent>,
    IConsumer<OrderStatusChangedEvent>
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;

    #endregion

    #region Ctor

    public RemindersEventConsumer(ICustomerService customerService, IGenericAttributeService genericAttributeService)
    {
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
    }

    #endregion

    #region Utilities

    protected virtual async Task RemoveFollowUpAttribute(Customer customer, string followUpAttributeName)
    {
        if (customer is not null)
            await _genericAttributeService.SaveAttributeAsync<int?>(customer, followUpAttributeName, null);
    }

    #endregion

    #region Shopping cart

    /// <summary>
    /// Handle the delete shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<ShoppingCartItem> eventMessage)
    {
        var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Entity.CustomerId);
        await RemoveFollowUpAttribute(customer, RemindersDefaults.AbandonedCarts.FollowUpAttributeName);
    }

    /// <summary>
    /// Handle the insert shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
    {
        var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Entity.CustomerId);
        await RemoveFollowUpAttribute(customer, RemindersDefaults.AbandonedCarts.FollowUpAttributeName);
    }

    /// <summary>
    /// Handle the update shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
    {
        var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Entity.CustomerId);
        await RemoveFollowUpAttribute(customer, RemindersDefaults.AbandonedCarts.FollowUpAttributeName);
    }

    #endregion

    #region Registration

    /// <summary>
    /// Handle customer registered event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(CustomerRegisteredEvent eventMessage)
    {
        await RemoveFollowUpAttribute(eventMessage?.Customer, RemindersDefaults.IncompleteRegistrations.FollowUpAttributeName);
    }

    #endregion

    #region Orders

    /// <summary>
    /// Handle change order status event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderStatusChangedEvent eventMessage)
    {
        if (eventMessage?.Order == null)
            return;

        if (eventMessage.Order.OrderStatus != OrderStatus.Complete)
            return;

        var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Order.CustomerId);
        await RemoveFollowUpAttribute(customer, RemindersDefaults.PendingOrders.FollowUpAttributeName);
    }

    #endregion
}
