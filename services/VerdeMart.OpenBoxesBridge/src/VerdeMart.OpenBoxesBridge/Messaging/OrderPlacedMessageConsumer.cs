using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VerdeMart.OpenBoxesBridge.Dedup;
using VerdeMart.OpenBoxesBridge.Messaging.Contracts;
using VerdeMart.OpenBoxesBridge.OpenBoxes;

namespace VerdeMart.OpenBoxesBridge.Messaging;

public class OrderPlacedMessageConsumer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IDedupRepository _dedup;
    private readonly IOpenBoxesClient _openBoxes;
    private readonly RetryCounter _retryCounter;
    private readonly BridgeSettings _settings;
    private readonly ILogger<OrderPlacedMessageConsumer> _logger;

    public OrderPlacedMessageConsumer(
        IDedupRepository dedup,
        IOpenBoxesClient openBoxes,
        RetryCounter retryCounter,
        IOptions<BridgeSettings> options,
        ILogger<OrderPlacedMessageConsumer> logger)
    {
        _dedup = dedup;
        _openBoxes = openBoxes;
        _retryCounter = retryCounter;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task HandleAsync(IChannel channel, BasicDeliverEventArgs args, CancellationToken ct)
    {
        OrderPlacedMessage? message;
        try
        {
            var json = Encoding.UTF8.GetString(args.Body.Span);
            message = JsonSerializer.Deserialize<OrderPlacedMessage>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse OrderPlacedMessage — sending to DLQ");
            await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false, cancellationToken: ct);
            return;
        }

        if (message is null)
        {
            _logger.LogError("Deserialised OrderPlacedMessage was null — sending to DLQ");
            await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false, cancellationToken: ct);
            return;
        }

        if (await _dedup.HasProcessedAsync(message.OrderGuid, ct))
        {
            _logger.LogInformation("OrderGuid={OrderGuid} already processed — ack and skip", message.OrderGuid);
            await channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken: ct);
            return;
        }

        CreateFulfillmentResult result;
        try
        {
            result = await _openBoxes.CreateFulfillmentAsync(message, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error calling OpenBoxes for OrderGuid={OrderGuid} — requeue", message.OrderGuid);
            await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: true, cancellationToken: ct);
            return;
        }

        switch (result)
        {
            case CreateFulfillmentResult.Success success:
                _retryCounter.Remove(message.OrderGuid);
                await _dedup.RecordProcessedAsync(message.OrderGuid, success.FulfillmentId, ct);
                await channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken: ct);
                _logger.LogInformation(
                    "Fulfillment created for OrderGuid={OrderGuid} FulfillmentId={FulfillmentId}",
                    message.OrderGuid, success.FulfillmentId);
                break;

            case CreateFulfillmentResult.Duplicate duplicate:
                _retryCounter.Remove(message.OrderGuid);
                await _dedup.RecordProcessedAsync(message.OrderGuid, duplicate.FulfillmentId, ct);
                await channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken: ct);
                _logger.LogInformation(
                    "OpenBoxes reported duplicate for OrderGuid={OrderGuid} FulfillmentId={FulfillmentId} — recorded locally",
                    message.OrderGuid, duplicate.FulfillmentId);
                break;

            case CreateFulfillmentResult.Failure failure:
                var attempt = _retryCounter.Increment(message.OrderGuid);
                if (!failure.Transient || attempt >= _settings.MaxRedeliveryAttempts)
                {
                    _retryCounter.Remove(message.OrderGuid);
                    _logger.LogError(
                        "OpenBoxes failure for OrderGuid={OrderGuid} (attempt {Attempt}/{Max}, transient={Transient}): {Reason} — sending to DLQ",
                        message.OrderGuid, attempt, _settings.MaxRedeliveryAttempts, failure.Transient, failure.Reason);
                    await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false, cancellationToken: ct);
                }
                else
                {
                    // Exponential backoff: 2s, 4s, 8s, 16s before requeue
                    var delay = TimeSpan.FromSeconds(Math.Min(30, Math.Pow(2, attempt)));
                    _logger.LogWarning(
                        "Transient OpenBoxes failure for OrderGuid={OrderGuid} (attempt {Attempt}/{Max}): {Reason} — retrying in {Delay}s",
                        message.OrderGuid, attempt, _settings.MaxRedeliveryAttempts, failure.Reason, delay.TotalSeconds);
                    await Task.Delay(delay, ct);
                    await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: true, cancellationToken: ct);
                }
                break;
        }
    }

}
