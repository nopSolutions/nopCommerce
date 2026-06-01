namespace Nop.Plugin.Shipping.CarrierTracking.Models;

public record CarrierBookingRequestedMessage(
    int ShipmentId,
    int OrderId,
    ShippingAddressMessage? ShippingAddress,
    IReadOnlyList<CarrierBookingLineItemMessage> Items,
    int Version = 1
);

public record ShippingAddressMessage(
    string? FirstName,
    string? LastName,
    string? Address1,
    string? Address2,
    string? City,
    string? ZipPostalCode,
    int? CountryId
);

public record CarrierBookingLineItemMessage(
    int ProductId,
    int Quantity
);
