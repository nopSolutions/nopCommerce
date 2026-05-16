namespace VerdeMart.OpenBoxesBridge.Messaging.Contracts;

public record OrderItemMessage(
    int ProductId,
    int Quantity,
    decimal UnitPriceInclTax);
