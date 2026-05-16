using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VerdeMart.OpenBoxesBridge.Messaging.Contracts;

namespace VerdeMart.OpenBoxesBridge.OpenBoxes;

public class OpenBoxesClient : IOpenBoxesClient
{
    private readonly HttpClient _http;
    private readonly ILogger<OpenBoxesClient> _logger;

    public OpenBoxesClient(HttpClient http, IOptions<BridgeSettings> options, ILogger<OpenBoxesClient> logger)
    {
        _http = http;
        _logger = logger;

        var settings = options.Value;
        _http.BaseAddress = new Uri(settings.OpenBoxesBaseUrl);
        if (!string.IsNullOrWhiteSpace(settings.OpenBoxesApiKey))
            _http.DefaultRequestHeaders.Add("X-Api-Key", settings.OpenBoxesApiKey);
    }

    public async Task<CreateFulfillmentResult> CreateFulfillmentAsync(OrderPlacedMessage message, CancellationToken ct)
    {
        var payload = new
        {
            orderGuid = message.OrderGuid,
            customerId = message.CustomerId,
            createdOnUtc = message.CreatedOnUtc,
            items = message.Items.Select(i => new
            {
                productId = i.ProductId,
                quantity = i.Quantity
            })
        };

        HttpResponseMessage response;
        try
        {
            response = await _http.PostAsJsonAsync("api/generic/order", payload, ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "OpenBoxes unreachable for OrderGuid={OrderGuid}", message.OrderGuid);
            return new CreateFulfillmentResult.Failure(ex.Message, Transient: true);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "OpenBoxes call timed out for OrderGuid={OrderGuid}", message.OrderGuid);
            return new CreateFulfillmentResult.Failure("timeout", Transient: true);
        }

        var body = await response.Content.ReadAsStringAsync(ct);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            var dupId = TryReadFulfillmentId(body) ?? message.OrderGuid.ToString();
            return new CreateFulfillmentResult.Duplicate(dupId);
        }

        if (response.IsSuccessStatusCode)
        {
            var id = TryReadFulfillmentId(body) ?? message.OrderGuid.ToString();
            return new CreateFulfillmentResult.Success(id);
        }

        var transient = (int)response.StatusCode >= 500;
        return new CreateFulfillmentResult.Failure(
            $"http {(int)response.StatusCode}: {body}",
            Transient: transient);
    }

    private static string? TryReadFulfillmentId(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("fulfillmentId", out var prop) && prop.ValueKind == JsonValueKind.String)
                return prop.GetString();
            if (doc.RootElement.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String)
                return idProp.GetString();
        }
        catch (JsonException)
        {
        }

        return null;
    }
}
