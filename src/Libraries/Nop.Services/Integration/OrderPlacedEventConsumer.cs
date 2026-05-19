using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using Nop.Services.Orders;

namespace Nop.Services.Integration;

/// <summary>
/// On OrderPlacedEvent, writes an `order.placed` IntegrationEvent to the outbox
/// so the Order Integration Service can pick it up via RabbitMQ.
/// </summary>
public partial class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IOutboxService _outboxService;
    private readonly IOrderService _orderService;

    public OrderPlacedEventConsumer(
        IOutboxService outboxService,
        IOrderService orderService)
    {
        _outboxService = outboxService;
        _orderService = orderService;
    }

    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        var order = eventMessage.Order;
        var items = await _orderService.GetOrderItemsAsync(order.Id);

        var payload = new
        {
            eventId = Guid.NewGuid().ToString(),
            orderId = order.Id,
            orderGuid = order.OrderGuid,
            customerId = order.CustomerId,
            storeId = order.StoreId,
            total = order.OrderTotal,
            currency = order.CustomerCurrencyCode,
            createdOnUtc = order.CreatedOnUtc,
            items = items.Select(i => new
            {
                productId = i.ProductId,
                quantity = i.Quantity,
                unitPrice = i.UnitPriceInclTax
            })
        };

        await _outboxService.WriteEventAsync("order.placed", payload);
    }
}
