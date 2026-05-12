using System.Text;
using System.Text.Json;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Plugin.Messaging.RabbitMq.Models;
using RabbitMQ.Client;

namespace Nop.Plugin.Messaging.RabbitMq.Services;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly RabbitMqSettings _settings;
    private readonly IOrderService _orderService;
    private readonly ILogger _logger;

    public OrderPlacedConsumer(
        IRabbitMqConnectionFactory connectionFactory,
        RabbitMqSettings settings,
        IOrderService orderService,
        ILogger logger)
    {
        _connectionFactory = connectionFactory;
        _settings = settings;
        _orderService = orderService;
        _logger = logger;
    }

    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        var order = eventMessage.Order;

        var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

        var message = new OrderPlacedMessage(
            OrderId: order.Id,
            OrderGuid: order.OrderGuid,
            CustomerId: order.CustomerId,
            OrderTotal: order.OrderTotal,
            CreatedOnUtc: order.CreatedOnUtc,
            Items: orderItems.Select(i => new OrderItemMessage(
                ProductId: i.ProductId,
                Quantity: i.Quantity,
                UnitPriceInclTax: i.UnitPriceInclTax)).ToList());

        var body = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            MessageId = order.OrderGuid.ToString()
        };

        await using var channel = await _connectionFactory.CreateChannelAsync();

        await channel.BasicPublishAsync(
            exchange: _settings.ExchangeName,
            routingKey: "order.placed",
            mandatory: false,
            basicProperties: properties,
            body: body);

        await _logger.InformationAsync($"[RabbitMq] Published order.placed for OrderGuid={order.OrderGuid}");
    }
}
