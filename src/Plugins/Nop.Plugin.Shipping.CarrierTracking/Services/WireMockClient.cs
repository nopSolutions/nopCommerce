using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nop.Services.Configuration;

namespace Nop.Plugin.Shipping.CarrierTracking.Services;

public class WireMockClient : IWireMockClient
{
    private readonly HttpClient _httpClient;
    private readonly ISettingService _settingService;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public WireMockClient(HttpClient httpClient, ISettingService settingService)
    {
        _httpClient = httpClient;
        _settingService = settingService;
    }

    public async Task<BookingResult> BookShipmentAsync(int shipmentId, CancellationToken ct = default)
    {
        var settings = await _settingService.LoadSettingAsync<CarrierTrackingSettings>();
        _httpClient.BaseAddress = new Uri(settings.WireMockBaseUrl);
        _httpClient.Timeout = TimeSpan.FromMilliseconds(settings.WireMockTimeoutMs);

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/api/shipments",
                new { shipmentId, carrierCode = settings.CarrierCode },
                ct);

            if (!response.IsSuccessStatusCode)
                return new BookingResult.PermanentFailure($"WireMock returned {(int)response.StatusCode}");

            var body = await response.Content.ReadFromJsonAsync<BookingResponse>(JsonOptions, ct);
            if (body?.ExternalShipmentId is null)
                return new BookingResult.PermanentFailure("WireMock response missing externalShipmentId");

            return new BookingResult.Success(body.ExternalShipmentId);
        }
        catch (TaskCanceledException)
        {
            return new BookingResult.TransientFailure("Request timed out");
        }
        catch (HttpRequestException ex)
        {
            return new BookingResult.TransientFailure(ex.Message);
        }
    }

    public async Task<ShipmentStatusResult> GetShipmentStatusAsync(string externalShipmentId, CancellationToken ct = default)
    {
        var settings = await _settingService.LoadSettingAsync<CarrierTrackingSettings>();
        _httpClient.BaseAddress = new Uri(settings.WireMockBaseUrl);
        _httpClient.Timeout = TimeSpan.FromMilliseconds(settings.WireMockTimeoutMs);

        try
        {
            var response = await _httpClient.GetAsync($"/api/shipments/{externalShipmentId}/status", ct);

            if (!response.IsSuccessStatusCode)
                return ShipmentStatusResult.Unreachable(externalShipmentId);

            var body = await response.Content.ReadFromJsonAsync<StatusResponse>(JsonOptions, ct);
            if (body?.Status is null)
                return ShipmentStatusResult.Unreachable(externalShipmentId);

            return new ShipmentStatusResult(externalShipmentId, body.Status, body.OccurredAtUtc ?? DateTime.UtcNow, IsUnreachable: false);
        }
        catch (TaskCanceledException)
        {
            return ShipmentStatusResult.Unreachable(externalShipmentId);
        }
        catch (HttpRequestException)
        {
            return ShipmentStatusResult.Unreachable(externalShipmentId);
        }
    }

    private record BookingResponse(
        [property: JsonPropertyName("externalShipmentId")] string? ExternalShipmentId);

    private record StatusResponse(
        [property: JsonPropertyName("status")] string? Status,
        [property: JsonPropertyName("occurredAtUtc")] DateTime? OccurredAtUtc);
}
