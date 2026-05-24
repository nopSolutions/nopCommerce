namespace Nop.Plugin.Fulfillment.OpenBoxes.Models;

public record CarrierBookingRequestedPayload(
    int ShipmentId,
    int OrderId,
    ShippingAddressPayload? ShippingAddress,
    IReadOnlyList<CarrierBookingLineItemPayload> Items,
    int Version = 1);

public record ShippingAddressPayload(
    string? FirstName,
    string? LastName,
    string? Address1,
    string? Address2,
    string? City,
    string? ZipPostalCode,
    int? CountryId);

public record CarrierBookingLineItemPayload(
    int ProductId,
    int Quantity);
