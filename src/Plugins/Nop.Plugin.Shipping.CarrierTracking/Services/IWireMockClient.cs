namespace Nop.Plugin.Shipping.CarrierTracking.Services;

public interface IWireMockClient
{
    Task<BookingResult> BookShipmentAsync(int shipmentId, CancellationToken ct = default);
    Task<ShipmentStatusResult> GetShipmentStatusAsync(string externalShipmentId, CancellationToken ct = default);
}

public record ShipmentStatusResult(
    string ExternalShipmentId,
    string Status,
    DateTime OccurredAtUtc,
    bool IsUnreachable)
{
    public static ShipmentStatusResult Unreachable(string externalShipmentId) =>
        new(externalShipmentId, string.Empty, DateTime.UtcNow, IsUnreachable: true);
}

public abstract record BookingResult
{
    public record Success(string CarrierTrackingId) : BookingResult;
    public record TransientFailure(string Reason) : BookingResult;
    public record PermanentFailure(string Reason) : BookingResult;
}
