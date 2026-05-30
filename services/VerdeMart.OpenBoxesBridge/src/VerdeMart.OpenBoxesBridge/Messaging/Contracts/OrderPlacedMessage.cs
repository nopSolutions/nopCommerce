namespace VerdeMart.OpenBoxesBridge.Messaging.Contracts;

public record OrderPlacedMessage(
    int OrderId,
    Guid OrderGuid,
    int CustomerId,
    decimal OrderTotal,
    DateTime CreatedOnUtc,
    IReadOnlyList<OrderItemMessage> Items,
    int Version = 2);
