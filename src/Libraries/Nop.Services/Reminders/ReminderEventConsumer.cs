using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Orders;

namespace Nop.Services.Reminders;

/// <summary>
/// Represents a reminder event consumer
/// </summary>
public partial class ReminderEventConsumer :
    IConsumer<CustomerActivatedEvent>,
    IConsumer<OrderStatusChangedEvent>
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IOrderService _orderService;

    #endregion

    #region Ctor

    public ReminderEventConsumer(ICustomerService customerService,
        IOrderService orderService)
    {
        _customerService = customerService;
        _orderService = orderService;
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
        if (eventMessage?.Customer?.Id == null || eventMessage.Customer.Id == 0)
            return;

        var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Customer.Id);
        customer.LastRegistrationFollowUpNumber = null;
        customer.LastRegistrationFollowUpDateUtc = null;
        await _customerService.UpdateCustomerAsync(customer);
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
        if (eventMessage?.Order is null || eventMessage.Order.CustomerId <= 0)
            return;

        if (eventMessage.Order.OrderStatus == OrderStatus.Complete || eventMessage.Order.OrderStatus == OrderStatus.Cancelled)
        {
            eventMessage.Order.LastPendingOrderFollowUpNumber = null;
            eventMessage.Order.LastPendingOrderFollowUpDateUtc = null;
            await _orderService.UpdateOrderAsync(eventMessage.Order);
        }
    }

    #endregion
}