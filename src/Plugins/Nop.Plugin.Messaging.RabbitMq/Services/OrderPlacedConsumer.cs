using System.Text.Json;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Messaging.RabbitMq.Domain;
using Nop.Plugin.Messaging.RabbitMq.Models;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Orders;

namespace Nop.Plugin.Messaging.RabbitMq.Services;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;
    private readonly ILogger _logger;

    public OrderPlacedConsumer(
        IOutboxRepository outboxRepository,
        IOrderService orderService,
        IProductService productService,
        ILogger logger)
    {
        _outboxRepository = outboxRepository;
        _orderService = orderService;
        _productService = productService;
        _logger = logger;
    }

    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        var order = eventMessage.Order;

        var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

        var products = await _productService.GetProductsByIdsAsync(
            orderItems.Select(i => i.ProductId).Distinct().ToArray());
        var productById = products.ToDictionary(p => p.Id);

        var message = new OrderPlacedMessage(
            OrderId: order.Id,
            OrderGuid: order.OrderGuid,
            CustomerId: order.CustomerId,
            OrderTotal: order.OrderTotal,
            CreatedOnUtc: order.CreatedOnUtc,
            Items: orderItems.Select(i =>
            {
                productById.TryGetValue(i.ProductId, out var product);
                return new OrderItemMessage(
                    ProductId: i.ProductId,
                    Sku: product?.Sku ?? string.Empty,
                    Name: product?.Name ?? string.Empty,
                    Quantity: i.Quantity,
                    UnitPriceInclTax: i.UnitPriceInclTax);
            }).ToList());

        var outboxMessage = new OutboxMessage
        {
            AggregateId = order.OrderGuid.ToString(),
            EventType = "order.placed",
            Payload = JsonSerializer.Serialize(message),
            Status = (int)OutboxMessageStatus.Pending,
            AttemptCount = 0,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _outboxRepository.InsertAsync(outboxMessage);

        await _logger.InformationAsync($"[RabbitMq] Queued order.placed for OrderGuid={order.OrderGuid}");
    }
}
