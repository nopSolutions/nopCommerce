namespace VerdeMart.OpenBoxesBridge.Messaging.Contracts;

public record OrderItemMessage(
    int ProductId,
    string Sku,
    string Name,
    int Quantity,
    decimal UnitPriceInclTax);
