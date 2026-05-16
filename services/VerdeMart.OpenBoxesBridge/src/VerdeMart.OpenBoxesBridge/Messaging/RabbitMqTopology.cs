using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace VerdeMart.OpenBoxesBridge.Messaging;

public class RabbitMqTopology
{
    private readonly BridgeSettings _settings;
    private readonly ILogger<RabbitMqTopology> _logger;

    public RabbitMqTopology(IOptions<BridgeSettings> options, ILogger<RabbitMqTopology> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task EnsureAsync(IChannel channel, CancellationToken ct)
    {
        // Declare the full topology the bridge depends on. RabbitMQ declarations are idempotent
        // when arguments match — declaring the same exchange/queue from both the bridge and the
        // nopCommerce plugin is safe. The bridge must be self-sufficient: the plugin declares
        // topology lazily (on first publish), so the queue may not exist at bridge startup.

        // Main exchange (matches RabbitMqSettings.ExchangeName in the nopCommerce plugin)
        await channel.ExchangeDeclareAsync(
            exchange: "verdemart.orders",
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: ct);

        // DLX — must exist before the main queue references it
        await channel.ExchangeDeclareAsync(
            exchange: _settings.DeadLetterExchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: ct);

        // DLQ
        await channel.QueueDeclareAsync(
            queue: _settings.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);

        await channel.QueueBindAsync(
            queue: _settings.DeadLetterQueue,
            exchange: _settings.DeadLetterExchange,
            routingKey: "order.placed",
            cancellationToken: ct);

        // Main queue — must include x-dead-letter-exchange so NACKs route to DLQ.
        // This must match exactly what the nopCommerce plugin declares (same args),
        // which is why RabbitMqConnectionFactory was also updated to pass the same argument.
        var queueArgs = new Dictionary<string, object?> { ["x-dead-letter-exchange"] = _settings.DeadLetterExchange };
        await channel.QueueDeclareAsync(
            queue: _settings.OrderQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArgs,
            cancellationToken: ct);

        await channel.QueueBindAsync(
            queue: _settings.OrderQueueName,
            exchange: "verdemart.orders",
            routingKey: "order.placed",
            cancellationToken: ct);

        _logger.LogInformation(
            "Topology ready: {Exchange} -> {Queue} (DLX={Dlx} -> DLQ={Dlq})",
            "verdemart.orders", _settings.OrderQueueName,
            _settings.DeadLetterExchange, _settings.DeadLetterQueue);
    }
}
