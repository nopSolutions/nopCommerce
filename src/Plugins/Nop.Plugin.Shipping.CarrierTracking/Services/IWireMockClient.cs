namespace Nop.Plugin.Shipping.CarrierTracking.Services;

public interface IWireMockClient
{
    Task<BookingResult> BookShipmentAsync(int shipmentId, CancellationToken ct = default);
}

public abstract record BookingResult
{
    public record Success(string CarrierTrackingId) : BookingResult;
    public record TransientFailure(string Reason) : BookingResult;
    public record PermanentFailure(string Reason) : BookingResult;
}
