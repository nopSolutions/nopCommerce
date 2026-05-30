using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nop.Core;
using Nop.Plugin.Fulfillment.OpenBoxes.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;

namespace Nop.Plugin.Fulfillment.OpenBoxes.Services;

public class OpenBoxesClient : IOpenBoxesClient
{
    private const string UserAgent = "VerdeMart-NopCommerce-Poller/1.0";

    private readonly HttpClient _http;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly ILogger _logger;
    private readonly SemaphoreSlim _loginLock = new(1, 1);
    private bool _loggedIn;
    private bool _locationChosen;

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

        _http.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
    }

    public async Task<IReadOnlyList<OpenBoxesFulfillmentOrder>> GetIssuedFulfillmentOrdersAsync(
        int batchSize, CancellationToken ct)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var settings = await _settingService.LoadSettingAsync<OpenBoxesSettings>(store.Id);

        if (string.IsNullOrWhiteSpace(settings.OpenBoxesBaseUrl))
        {
            await _logger.WarningAsync("[OpenBoxesClient] OpenBoxesBaseUrl is not configured; skipping poll");
            return Array.Empty<OpenBoxesFulfillmentOrder>();
        }

        EnsureBaseAddress(settings.OpenBoxesBaseUrl);

        var sendResult = await SendWithReloginAsync(
            settings,
            () => new HttpRequestMessage(HttpMethod.Get,
                $"api/generic/shipment?status=ISSUED&max={batchSize}"),
            ct);

        if (sendResult.Failed)
            return Array.Empty<OpenBoxesFulfillmentOrder>();

        using var response = sendResult.Response!;
        if (!response.IsSuccessStatusCode)
        {
            await _logger.WarningAsync(
                $"[OpenBoxesClient] OpenBoxes returned {(int)response.StatusCode} when polling for ISSUED fulfillments");
            return Array.Empty<OpenBoxesFulfillmentOrder>();
        }

        var body = await response.Content.ReadAsStringAsync(ct);

        OpenBoxesListResponse<OpenBoxesShipmentDto>? envelope;
        try
        {
            envelope = JsonSerializer.Deserialize<OpenBoxesListResponse<OpenBoxesShipmentDto>>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
            await _logger.WarningAsync("[OpenBoxesClient] Failed to parse OpenBoxes response", ex);
            return Array.Empty<OpenBoxesFulfillmentOrder>();
        }

        var dtos = envelope?.Data;
        if (dtos is null || dtos.Count == 0)
            return Array.Empty<OpenBoxesFulfillmentOrder>();

        var result = new List<OpenBoxesFulfillmentOrder>(dtos.Count);
        foreach (var dto in dtos)
        {
            var orderGuid = TryExtractOrderGuidFromName(dto.Name);
            if (orderGuid is null)
            {
                await _logger.WarningAsync(
                    $"[OpenBoxesClient] Could not extract OrderGuid from shipment name '{dto.Name}'; skipping");
                continue;
            }

            result.Add(new OpenBoxesFulfillmentOrder(
                FulfillmentId: dto.Id ?? string.Empty,
                OrderGuid: orderGuid.Value,
                Status: dto.Status ?? string.Empty,
                IssuedAtUtc: ParseOpenBoxesDate(dto.ActualShippingDate) ?? DateTime.UtcNow));
        }

        return result;
    }

    public async Task ReceiveFulfillmentAsync(string fulfillmentId, CancellationToken ct)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var settings = await _settingService.LoadSettingAsync<OpenBoxesSettings>(store.Id);

        if (string.IsNullOrWhiteSpace(settings.OpenBoxesBaseUrl))
        {
            await _logger.WarningAsync("[OpenBoxesClient] OpenBoxesBaseUrl is not configured; skipping receive");
            return;
        }

        EnsureBaseAddress(settings.OpenBoxesBaseUrl);

        var getResult = await SendWithReloginAsync(
            settings,
            () => new HttpRequestMessage(HttpMethod.Get, $"api/partialReceiving/{fulfillmentId}"),
            ct);

        if (getResult.Failed)
        {
            await _logger.WarningAsync($"[OpenBoxesClient] Failed to fetch partial receiving data for FulfillmentId={fulfillmentId}");
            return;
        }

        using var getResponse = getResult.Response!;
        if (!getResponse.IsSuccessStatusCode)
        {
            await _logger.WarningAsync($"[OpenBoxesClient] OpenBoxes returned {(int)getResponse.StatusCode} fetching partial receiving for FulfillmentId={fulfillmentId}");
            return;
        }

        var getBody = await getResponse.Content.ReadAsStringAsync(ct);
        PartialReceivingDto? receipt;
        try
        {
            var envelope = JsonSerializer.Deserialize<PartialReceivingEnvelope>(getBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            receipt = envelope?.Data;
        }
        catch (JsonException ex)
        {
            await _logger.WarningAsync($"[OpenBoxesClient] Failed to parse partial receiving response for FulfillmentId={fulfillmentId}", ex);
            return;
        }

        if (receipt is null)
        {
            await _logger.WarningAsync($"[OpenBoxesClient] Empty partial receiving response for FulfillmentId={fulfillmentId}");
            return;
        }

        var dateDelivered = DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm 'Z'", CultureInfo.InvariantCulture);

        // Step 1: POST PENDING to create the receipt and get back receiptItemIds
        var pendingReceipt = await PostPartialReceivingAsync(settings, fulfillmentId, receipt, "PENDING", dateDelivered, ct);
        if (pendingReceipt is null)
        {
            await _logger.WarningAsync($"[OpenBoxesClient] Failed to save PENDING receipt for FulfillmentId={fulfillmentId}");
            return;
        }

        // Step 2: POST COMPLETED using the receiptItemIds from the PENDING response
        var completed = await PostPartialReceivingAsync(settings, fulfillmentId, pendingReceipt, "COMPLETED", dateDelivered, ct);
        if (completed is null)
        {
            await _logger.WarningAsync($"[OpenBoxesClient] Failed to complete receipt for FulfillmentId={fulfillmentId}");
            return;
        }

        await _logger.InformationAsync($"[OpenBoxesClient] FulfillmentId={fulfillmentId} marked as RECEIVED in OpenBoxes");
    }

    private async Task<PartialReceivingDto?> PostPartialReceivingAsync(
        OpenBoxesSettings settings,
        string fulfillmentId,
        PartialReceivingDto receipt,
        string receiptStatus,
        string dateDelivered,
        CancellationToken ct)
    {
        var body = new Dictionary<string, object?>
        {
            ["shipment.id"] = fulfillmentId,
            ["receiptStatus"] = receiptStatus,
            ["dateDelivered"] = dateDelivered,
            ["containers"] = receipt.Containers?.Select(c => new Dictionary<string, object?>
            {
                ["container.id"] = c.ContainerId,
                ["shipmentItems"] = c.ShipmentItems?.Select(i => new Dictionary<string, object?>
                {
                    ["receiptItemId"] = i.ReceiptItemId,
                    ["shipmentItemId"] = i.ShipmentItemId,
                    ["quantityReceiving"] = (i.QuantityReceiving ?? 0) > 0 ? i.QuantityReceiving!.Value : i.QuantityRemaining,
                    ["cancelRemaining"] = false
                }).ToList()
            }).ToList()
        };

        var json = JsonSerializer.Serialize(body);
        var result = await SendWithReloginAsync(
            settings,
            () => new HttpRequestMessage(HttpMethod.Post, $"api/partialReceiving/{fulfillmentId}")
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            },
            ct);

        if (result.Failed)
            return null;

        using var response = result.Response!;
        if (!response.IsSuccessStatusCode)
        {
            await _logger.WarningAsync(
                $"[OpenBoxesClient] OpenBoxes returned {(int)response.StatusCode} on {receiptStatus} receipt POST for FulfillmentId={fulfillmentId}");
            return null;
        }

        var responseBody = await response.Content.ReadAsStringAsync(ct);
        try
        {
            var envelope = JsonSerializer.Deserialize<PartialReceivingEnvelope>(responseBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return envelope?.Data;
        }
        catch (JsonException ex)
        {
            await _logger.WarningAsync(
                $"[OpenBoxesClient] Failed to parse {receiptStatus} receipt response for FulfillmentId={fulfillmentId}", ex);
            return null;
        }
    }

    private void EnsureBaseAddress(string url)
    {
        if (_http.BaseAddress is not null) return;
        var withSlash = url.EndsWith('/') ? url : url + "/";
        _http.BaseAddress = new Uri(withSlash);
    }

    private const int NameSegmentsBeforeDescription = 3;

    private static Guid? TryExtractOrderGuidFromName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var segments = name.Split('-');
        if (segments.Length <= NameSegmentsBeforeDescription)
            return null;

        var description = string.Join('-', segments, NameSegmentsBeforeDescription,
            segments.Length - NameSegmentsBeforeDescription);

        return Guid.TryParse(description, out var guid) ? guid : null;
    }

    private static DateTime? ParseOpenBoxesDate(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        return DateTime.TryParseExact(
            raw,
            "MM/dd/yyyy HH:mm 'Z'",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var dt)
            ? dt
            : null;
    }

    private async Task<SendResult> SendWithReloginAsync(
        OpenBoxesSettings settings,
        Func<HttpRequestMessage> buildRequest,
        CancellationToken ct)
    {
        try
        {
            await EnsureSessionAsync(settings, ct);
            var response = await _http.SendAsync(buildRequest(), ct);

            if (IsRedirectToLogin(response))
            {
                response.Dispose();
                InvalidateSession();
                await EnsureSessionAsync(settings, ct);
                response = await _http.SendAsync(buildRequest(), ct);
            }

            return new SendResult(response, false);
        }
        catch (HttpRequestException ex)
        {
            await _logger.WarningAsync("[OpenBoxesClient] OpenBoxes call failed", ex);
            return new SendResult(null, true);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            await _logger.WarningAsync("[OpenBoxesClient] OpenBoxes call timed out", ex);
            return new SendResult(null, true);
        }
    }

    private void InvalidateSession()
    {
        _loggedIn = false;
        _locationChosen = false;
    }

    private async Task EnsureSessionAsync(OpenBoxesSettings settings, CancellationToken ct)
    {
        if (_loggedIn && _locationChosen) return;

        await _loginLock.WaitAsync(ct);
        try
        {
            if (!_loggedIn) await LoginAsync(settings, ct);
            if (!_locationChosen) await ChooseLocationAsync(settings, ct);
        }
        finally
        {
            _loginLock.Release();
        }
    }

    private async Task LoginAsync(OpenBoxesSettings settings, CancellationToken ct)
    {
        using var form = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("username", settings.OpenBoxesUsername),
            new KeyValuePair<string, string>("password", settings.OpenBoxesPassword),
            new KeyValuePair<string, string>("targetUri", "")
        });

        using var response = await _http.PostAsync("auth/handleLogin", form, ct);
        var locationHeader = response.Headers.Location?.ToString() ?? "";

        if (response.StatusCode is HttpStatusCode.Found or HttpStatusCode.SeeOther)
        {
            var looksRejected = locationHeader.Contains("authfail", StringComparison.OrdinalIgnoreCase)
                || locationHeader.Contains("/auth/login", StringComparison.OrdinalIgnoreCase)
                || locationHeader.Contains("/auth/handleLogin", StringComparison.OrdinalIgnoreCase);

            if (looksRejected)
                throw new HttpRequestException($"OpenBoxes login rejected (redirected to {locationHeader})");

            _loggedIn = true;
            await _logger.InformationAsync(
                $"[OpenBoxesClient] Logged into OpenBoxes as {settings.OpenBoxesUsername}");
            return;
        }

        if (response.StatusCode == HttpStatusCode.OK)
            throw new HttpRequestException("OpenBoxes login rejected (200 OK — form re-rendered)");

        throw new HttpRequestException(
            $"OpenBoxes login returned unexpected status {(int)response.StatusCode}");
    }

    private async Task ChooseLocationAsync(OpenBoxesSettings settings, CancellationToken ct)
    {
        var url = $"dashboard/chooseLocation?id={settings.OpenBoxesOriginLocationId}";
        using var response = await _http.GetAsync(url, ct);

        if (response.StatusCode is HttpStatusCode.Found or HttpStatusCode.SeeOther or HttpStatusCode.OK)
        {
            _locationChosen = true;
            return;
        }

        throw new HttpRequestException(
            $"OpenBoxes chooseLocation returned {(int)response.StatusCode}");
    }

    private static bool IsRedirectToLogin(HttpResponseMessage response)
    {
        if ((int)response.StatusCode is not (301 or 302 or 303 or 307 or 308))
            return false;
        var location = response.Headers.Location?.ToString() ?? "";
        return location.Contains("handleUnauthorized", StringComparison.OrdinalIgnoreCase)
            || location.Contains("auth/login", StringComparison.OrdinalIgnoreCase)
            || location.Contains("chooseLocation", StringComparison.OrdinalIgnoreCase);
    }

    private sealed record SendResult(HttpResponseMessage? Response, bool Failed);

    private sealed class OpenBoxesListResponse<T>
    {
        [JsonPropertyName("data")]
        public List<T>? Data { get; set; }
    }

    private sealed class OpenBoxesShipmentDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("actualShippingDate")]
        public string? ActualShippingDate { get; set; }
    }

    private sealed class PartialReceivingEnvelope
    {
        [JsonPropertyName("data")]
        public PartialReceivingDto? Data { get; set; }
    }

    private sealed class PartialReceivingDto
    {
        [JsonPropertyName("containers")]
        public List<PartialReceivingContainerDto>? Containers { get; set; }
    }

    private sealed class PartialReceivingContainerDto
    {
        [JsonPropertyName("container.id")]
        public string? ContainerId { get; set; }

        [JsonPropertyName("shipmentItems")]
        public List<PartialReceivingItemDto>? ShipmentItems { get; set; }
    }

    private sealed class PartialReceivingItemDto
    {
        [JsonPropertyName("receiptItemId")]
        public string? ReceiptItemId { get; set; }

        [JsonPropertyName("shipmentItemId")]
        public string? ShipmentItemId { get; set; }

        [JsonPropertyName("quantityReceiving")]
        public int? QuantityReceiving { get; set; }

        [JsonPropertyName("quantityRemaining")]
        public int QuantityRemaining { get; set; }
    }
}
