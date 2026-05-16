namespace Nop.Plugin.Messaging.RabbitMq.Models;

public record OrderPlacedMessage(
    int OrderId,
    Guid OrderGuid,
    int CustomerId,
    decimal OrderTotal,
    DateTime CreatedOnUtc,
    IReadOnlyList<OrderItemMessage> Items,
    int Version = 1);

public record OrderItemMessage(
    int ProductId,
    int Quantity,
    decimal UnitPriceInclTax);
