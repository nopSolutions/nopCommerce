using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Logging;

namespace Nop.Plugin.Inventory.AllocationGate.OpenBoxes;

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
        var settings = await _settingService.LoadSettingAsync<AllocationSettings>(store.Id);

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
        AllocationSettings settings,
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

    private async Task EnsureSessionAsync(AllocationSettings settings, CancellationToken ct)
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

    private async Task LoginAsync(AllocationSettings settings, CancellationToken ct)
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

    private async Task ChooseLocationAsync(AllocationSettings settings, CancellationToken ct)
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
}
