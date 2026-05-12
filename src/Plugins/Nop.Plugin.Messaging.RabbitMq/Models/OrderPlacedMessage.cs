namespace Nop.Plugin.Messaging.RabbitMq.Models;

public record OrderPlacedMessage(
    int OrderId,
    Guid OrderGuid,
    int CustomerId,
    decimal OrderTotal,
    DateTime CreatedOnUtc,
    IReadOnlyList<OrderItemMessage> Items);

public record OrderItemMessage(
    int ProductId,
    int Quantity,
    decimal UnitPriceInclTax);
