using System.Text.Json;
using System.Text.Json.Serialization;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Logging;

namespace Nop.Plugin.Inventory.AllocationGate.OpenBoxes;

public class OpenBoxesClient : IOpenBoxesClient
{
    private readonly HttpClient _http;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly ILogger _logger;

    public OpenBoxesClient(
        HttpClient http,
        ISettingService settingService,
        IStoreContext storeContext,
        ILogger logger)
    {
        _http = http;
        _settingService = settingService;
        _storeContext = storeContext;
        _logger = logger;
    }

    public async Task<IReadOnlyList<OpenBoxesFulfillmentOrder>> GetIssuedFulfillmentOrdersAsync(
        int batchSize, CancellationToken ct)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var settings = await _settingService.LoadSettingAsync<AllocationSettings>(store.Id);

        if (string.IsNullOrWhiteSpace(settings.OpenBoxesBaseUrl))
        {
            await _logger.WarningAsync("[OpenBoxesClient] OpenBoxesBaseUrl is not configured; skipping poll");
            return Array.Empty<OpenBoxesFulfillmentOrder>();
        }

        var baseUrl = settings.OpenBoxesBaseUrl.TrimEnd('/');
        var url = $"{baseUrl}/api/generic/shipment?status=ISSUED&max={batchSize}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (!string.IsNullOrWhiteSpace(settings.OpenBoxesApiKey))
            request.Headers.Add("X-Api-Key", settings.OpenBoxesApiKey);

        HttpResponseMessage response;
        try
        {
            response = await _http.SendAsync(request, ct);
        }
        catch (HttpRequestException ex)
        {
            await _logger.WarningAsync("[OpenBoxesClient] OpenBoxes unreachable when polling for ISSUED fulfillments", ex);
            return Array.Empty<OpenBoxesFulfillmentOrder>();
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            await _logger.WarningAsync("[OpenBoxesClient] OpenBoxes call timed out when polling for ISSUED fulfillments", ex);
            return Array.Empty<OpenBoxesFulfillmentOrder>();
        }

        if (!response.IsSuccessStatusCode)
        {
            await _logger.WarningAsync($"[OpenBoxesClient] OpenBoxes returned {(int)response.StatusCode} when polling for ISSUED fulfillments");
            return Array.Empty<OpenBoxesFulfillmentOrder>();
        }

        var body = await response.Content.ReadAsStringAsync(ct);

        List<OpenBoxesShipmentDto>? dtos;
        try
        {
            dtos = JsonSerializer.Deserialize<List<OpenBoxesShipmentDto>>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
            await _logger.WarningAsync("[OpenBoxesClient] Failed to parse OpenBoxes response", ex);
            return Array.Empty<OpenBoxesFulfillmentOrder>();
        }

        if (dtos is null || dtos.Count == 0)
            return Array.Empty<OpenBoxesFulfillmentOrder>();

        var result = new List<OpenBoxesFulfillmentOrder>(dtos.Count);
        foreach (var dto in dtos)
        {
            if (!Guid.TryParse(dto.ReferenceNumber, out var orderGuid))
                continue;

            result.Add(new OpenBoxesFulfillmentOrder(
                FulfillmentId: dto.Id ?? string.Empty,
                OrderGuid: orderGuid,
                Status: dto.Status ?? string.Empty,
                IssuedAtUtc: dto.LastUpdated ?? DateTime.UtcNow));
        }

        return result;
    }

    private sealed class OpenBoxesShipmentDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("referenceNumber")]
        public string? ReferenceNumber { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("lastUpdated")]
        public DateTime? LastUpdated { get; set; }
    }
}
