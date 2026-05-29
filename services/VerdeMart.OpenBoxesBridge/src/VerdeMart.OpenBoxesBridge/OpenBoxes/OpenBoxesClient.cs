using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VerdeMart.OpenBoxesBridge.Messaging.Contracts;

namespace VerdeMart.OpenBoxesBridge.OpenBoxes;

public class OpenBoxesClient : IOpenBoxesClient
{
    private const string UserAgent = "VerdeMart-OpenBoxes-Bridge/1.0";

    private readonly HttpClient _http;
    private readonly BridgeSettings _settings;
    private readonly ILogger<OpenBoxesClient> _logger;
    private readonly SemaphoreSlim _loginLock = new(1, 1);
    private bool _loggedIn;
    private bool _locationChosen;
    private string? _resolvedDestinationLocationId;

    public OpenBoxesClient(HttpClient http, IOptions<BridgeSettings> options, ILogger<OpenBoxesClient> logger)
    {
        _http = http;
        _settings = options.Value;
        _logger = logger;

        var baseUrl = _settings.OpenBoxesBaseUrl.EndsWith('/')
            ? _settings.OpenBoxesBaseUrl
            : _settings.OpenBoxesBaseUrl + "/";
        _http.BaseAddress = new Uri(baseUrl);

        _http.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
    }

    public async Task<CreateFulfillmentResult> CreateFulfillmentAsync(OrderPlacedMessage message, CancellationToken ct)
    {
        var sendResult = await SendWithReloginAsync(
            () => BuildPostRequest("api/stockMovements", new
            {
                origin = new { id = _settings.OpenBoxesOriginLocationId },
                destination = new { id = _resolvedDestinationLocationId ?? _settings.OpenBoxesDestinationLocationId },
                requestedBy = new { id = _settings.OpenBoxesRequestedByPersonId },
                dateRequested = message.CreatedOnUtc.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                description = message.OrderGuid.ToString(),
                requestType = "STOCK"
            }),
            message.OrderGuid,
            ct);

        if (sendResult.TransportFailure is { } failure)
            return failure;

        var response = sendResult.Response!;
        var body = await response.Content.ReadAsStringAsync(ct);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            var dupId = TryReadFulfillmentId(body) ?? message.OrderGuid.ToString();
            return new CreateFulfillmentResult.Duplicate(dupId);
        }

        if (response.IsSuccessStatusCode)
        {
            var id = TryReadFulfillmentId(body);
            if (id is null)
            {
                _logger.LogWarning(
                    "OpenBoxes returned 2xx but no fulfillment id was found for OrderGuid={OrderGuid}. Body: {Body}",
                    message.OrderGuid, body);
                return new CreateFulfillmentResult.Failure("response had no id", Transient: false);
            }
            return new CreateFulfillmentResult.Success(id);
        }

        var transient = (int)response.StatusCode >= 500;
        return new CreateFulfillmentResult.Failure(
            $"http {(int)response.StatusCode}: {body}",
            Transient: transient);
    }

    private static HttpRequestMessage BuildPostRequest(string relativeUrl, object payload)
    {
        return new HttpRequestMessage(HttpMethod.Post, relativeUrl)
        {
            Content = JsonContent.Create(payload)
        };
    }

    private async Task<SendOutcome> SendWithReloginAsync(
        Func<HttpRequestMessage> buildRequest, Guid orderGuid, CancellationToken ct)
    {
        try
        {
            await EnsureSessionAsync(ct);
            var response = await _http.SendAsync(buildRequest(), ct);

            if (IsRedirectToLogin(response))
            {
                _logger.LogInformation("OpenBoxes session appears invalid; re-establishing and retrying");
                response.Dispose();
                InvalidateSession();
                await EnsureSessionAsync(ct);
                response = await _http.SendAsync(buildRequest(), ct);
            }

            return new SendOutcome(response, null);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "OpenBoxes unreachable for OrderGuid={OrderGuid}", orderGuid);
            return new SendOutcome(null, new CreateFulfillmentResult.Failure(ex.Message, Transient: true));
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "OpenBoxes call timed out for OrderGuid={OrderGuid}", orderGuid);
            return new SendOutcome(null, new CreateFulfillmentResult.Failure("timeout", Transient: true));
        }
    }

    private void InvalidateSession()
    {
        _loggedIn = false;
        _locationChosen = false;
        _resolvedDestinationLocationId = null;
    }

    private async Task EnsureSessionAsync(CancellationToken ct)
    {
        var useNameResolution = string.IsNullOrEmpty(_settings.OpenBoxesDestinationLocationId);

        if (_loggedIn && _locationChosen && (!useNameResolution || _resolvedDestinationLocationId is not null)) return;

        await _loginLock.WaitAsync(ct);
        try
        {
            if (!_loggedIn) await LoginAsync(ct);
            if (!_locationChosen) await ChooseLocationAsync(ct);
            if (useNameResolution && _resolvedDestinationLocationId is null)
                _resolvedDestinationLocationId = await EnsureDestinationLocationAsync(ct);
        }
        finally
        {
            _loginLock.Release();
        }
    }

    private async Task LoginAsync(CancellationToken ct)
    {
        using var form = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("username", _settings.OpenBoxesUsername),
            new KeyValuePair<string, string>("password", _settings.OpenBoxesPassword),
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
            {
                _logger.LogError("OpenBoxes login rejected: redirect to {Location}", locationHeader);
                throw new HttpRequestException($"OpenBoxes login rejected (redirected to {locationHeader})");
            }

            _loggedIn = true;
            _logger.LogInformation("Logged into OpenBoxes as {User} (post-login redirect: {Location})",
                _settings.OpenBoxesUsername, locationHeader);
            return;
        }

        if (response.StatusCode == HttpStatusCode.OK)
        {
            _logger.LogError("OpenBoxes login returned 200 — credentials likely rejected (form re-rendered)");
            throw new HttpRequestException("OpenBoxes login rejected (200 OK)");
        }

        throw new HttpRequestException(
            $"OpenBoxes login returned unexpected status {(int)response.StatusCode}");
    }

    private async Task ChooseLocationAsync(CancellationToken ct)
    {
        var url = $"dashboard/chooseLocation?id={_settings.OpenBoxesOriginLocationId}";
        using var response = await _http.GetAsync(url, ct);

        // chooseLocation redirects to dashboard on success. Any other status means trouble.
        if (response.StatusCode is HttpStatusCode.Found or HttpStatusCode.SeeOther or HttpStatusCode.OK)
        {
            _locationChosen = true;
            _logger.LogInformation("Chose OpenBoxes location id={LocationId}",
                _settings.OpenBoxesOriginLocationId);
            return;
        }

        var body = await response.Content.ReadAsStringAsync(ct);
        throw new HttpRequestException(
            $"OpenBoxes chooseLocation returned {(int)response.StatusCode}: {Truncate(body, 200)}");
    }

    private async Task<string> EnsureDestinationLocationAsync(CancellationToken ct)
    {
        var name = _settings.OpenBoxesDestinationLocationName;
        var encodedName = Uri.EscapeDataString(name);

        using var listResponse = await _http.GetAsync($"api/locations?name={encodedName}&max=1", ct);
        var listBody = await listResponse.Content.ReadAsStringAsync(ct);

        if (listResponse.IsSuccessStatusCode)
        {
            var existing = TryReadLocationId(listBody);
            if (existing is not null)
            {
                _logger.LogInformation(
                    "Found existing destination location '{Name}' → {Id}", name, existing);
                return existing;
            }
        }

        var createPayload = new { name, locationType = new { id = "2" }, organization = new { id = "1" } };
        using var createResponse = await _http.SendAsync(
            BuildPostRequest("api/locations", createPayload), ct);
        var createBody = await createResponse.Content.ReadAsStringAsync(ct);

        if (!createResponse.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"Failed to create destination location '{name}': http {(int)createResponse.StatusCode}: {Truncate(createBody, 200)}");

        var created = TryReadLocationId(createBody)
            ?? throw new HttpRequestException(
                $"Created destination location '{name}' but response contained no id. Body: {Truncate(createBody, 200)}");

        _logger.LogInformation(
            "Created new destination location '{Name}' → {Id}", name, created);
        return created;
    }

    private static string? TryReadLocationId(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(body);

            // List response: { "data": [ { "id": "..." } ] }
            if (doc.RootElement.TryGetProperty("data", out var data))
            {
                if (data.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in data.EnumerateArray())
                    {
                        if (item.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String)
                            return idProp.GetString();
                    }
                    return null;
                }

                // Single-object response: { "data": { "id": "..." } }
                if (data.ValueKind == JsonValueKind.Object
                    && data.TryGetProperty("id", out var nestedId)
                    && nestedId.ValueKind == JsonValueKind.String)
                    return nestedId.GetString();
            }

            // Flat response: { "id": "..." }
            if (doc.RootElement.TryGetProperty("id", out var rootId) && rootId.ValueKind == JsonValueKind.String)
                return rootId.GetString();
        }
        catch (JsonException)
        {
        }

        return null;
    }

    private static string Truncate(string s, int max) =>
        string.IsNullOrEmpty(s) || s.Length <= max ? s : s[..max] + "...";

    private static bool IsRedirectToLogin(HttpResponseMessage response)
    {
        if ((int)response.StatusCode is not (301 or 302 or 303 or 307 or 308))
            return false;
        var location = response.Headers.Location?.ToString() ?? "";
        return location.Contains("handleUnauthorized", StringComparison.OrdinalIgnoreCase)
            || location.Contains("auth/login", StringComparison.OrdinalIgnoreCase)
            || location.Contains("chooseLocation", StringComparison.OrdinalIgnoreCase);
    }

    private sealed record SendOutcome(
        HttpResponseMessage? Response,
        CreateFulfillmentResult.Failure? TransportFailure);

    private static string? TryReadFulfillmentId(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(body);

            var root = doc.RootElement.TryGetProperty("data", out var dataElement)
                ? dataElement
                : doc.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
                return null;

            if (root.TryGetProperty("fulfillmentId", out var prop) && prop.ValueKind == JsonValueKind.String)
                return prop.GetString();
            if (root.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String)
                return idProp.GetString();
        }
        catch (JsonException)
        {
        }

        return null;
    }
}
