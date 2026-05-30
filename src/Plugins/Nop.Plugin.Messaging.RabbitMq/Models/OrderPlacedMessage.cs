namespace Nop.Plugin.Messaging.RabbitMq.Models;

public record OrderPlacedMessage(
    int OrderId,
    Guid OrderGuid,
    int CustomerId,
    decimal OrderTotal,
    DateTime CreatedOnUtc,
    IReadOnlyList<OrderItemMessage> Items,
    int Version = 2);

public record OrderItemMessage(
    int ProductId,
    string Sku,
    string Name,
    int Quantity,
    decimal UnitPriceInclTax);
