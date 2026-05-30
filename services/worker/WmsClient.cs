using System.Net.Http.Json;
using Omnichannel.Contracts;
using Polly;
using Omnichannel.Worker.Resilience;

namespace Omnichannel.Worker;

/// <summary>
/// Calls the WMS simulator's POST /fulfillments and maps the response to a
/// <see cref="FulfillmentStatusChanged"/> payload. The WMS call is wrapped in
/// the retry + circuit-breaker pipeline so the worker degrades gracefully when
/// the WMS is slow/unavailable (QA-1).
///
/// SCAFFOLD: the happy path is implemented; degraded mapping (breaker-open →
/// status "pending"/"degraded") is stubbed and must be finished in Phase 3.
/// </summary>
public sealed class WmsClient
{
    private readonly HttpClient _httpClient;
    private readonly WorkerOptions _options;
    private readonly ResiliencePipeline _pipeline;
    private readonly ILogger<WmsClient> _logger;

    public WmsClient(HttpClient httpClient, WorkerOptions options, ILogger<WmsClient> logger)
    {
        _httpClient = httpClient;
        _options = options;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(_options.WmsBaseUrl.TrimEnd('/'));
        _pipeline = WmsResiliencePipeline.Build(_options, logger);
    }

    public async Task<FulfillmentStatusChanged> RequestFulfillmentAsync(
        IntegrationMessage<CommerceOrderPlaced> message,
        CancellationToken cancellationToken)
    {
        var order = message.Payload;

        // The WMS expects the order-placed contract (see wms-sim/app/schemas.py).
        var wmsRequest = new
        {
            messageId = message.MessageId,
            correlationId = message.CorrelationId,
            eventType = message.EventType,
            occurredOnUtc = message.OccurredOnUtc,
            orderGuid = order.OrderGuid,
            orderId = order.OrderId,
            storeId = order.StoreId,
            items = order.Lines.Select(l => new
            {
                orderItemId = 0,
                productId = l.ProductId,
                sku = l.Sku,
                quantity = l.Quantity,
                warehouseId = 1
            })
        };

        var response = await _pipeline.ExecuteAsync(async ct =>
        {
            var http = await _httpClient.PostAsJsonAsync(_options.WmsFulfillmentPath, wmsRequest, ct);
            http.EnsureSuccessStatusCode();
            return await http.Content.ReadFromJsonAsync<WmsFulfillmentResponse>(ct)
                   ?? throw new InvalidOperationException("Empty WMS response");
        }, cancellationToken);

        _logger.LogInformation(
            "WMS accepted order_guid={OrderGuid} message_id={MessageId} external_request_id={ExternalRequestId}",
            order.OrderGuid, message.MessageId, response.ExternalRequestId);

        return new FulfillmentStatusChanged
        {
            OrderGuid = order.OrderGuid,
            ExternalRequestId = response.ExternalRequestId,
            Status = response.Status,
            Reason = null
        };
    }

    private sealed record WmsFulfillmentResponse
    {
        public string ExternalRequestId { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public Guid OrderGuid { get; init; }
        public Guid MessageId { get; init; }
    }
}
