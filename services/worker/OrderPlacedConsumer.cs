using System.Text;
using System.Text.Json;
using Omnichannel.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Omnichannel.Worker;

/// <summary>
/// Background service: consumes <c>commerce.order.placed.v1</c> from RabbitMQ,
/// calls the WMS via <see cref="WmsClient"/>, and (Phase 2) posts
/// <c>fulfillment.status.changed.v1</c> back to the plugin callback.
///
/// Declares the topology in <see cref="Topology"/> (idempotent), including the
/// dead-letter exchange/queue so poison messages land in the DLQ instead of
/// looping. This is the async workflow + DLQ reliability decision the assignment
/// requires.
///
/// SCAFFOLD: consume + WMS call + ack/nack are wired. Posting the result back to
/// the plugin callback is stubbed (logs only) and must be completed in Phase 2,
/// together with the plugin-side callback for fulfillment status.
/// </summary>
public sealed class OrderPlacedConsumer : BackgroundService
{
    private readonly WorkerOptions _options;
    private readonly WmsClient _wmsClient;
    private readonly ILogger<OrderPlacedConsumer> _logger;

    private IConnection? _connection;
    private IChannel? _channel;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public OrderPlacedConsumer(WorkerOptions options, WmsClient wmsClient, ILogger<OrderPlacedConsumer> logger)
    {
        _options = options;
        _wmsClient = wmsClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { Uri = new Uri(_options.RabbitMqUri) };
        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await DeclareTopologyAsync(_channel, stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageAsync;

        await _channel.BasicConsumeAsync(
            queue: Topology.OrderPlacedQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("Worker consuming {Queue}", Topology.OrderPlacedQueue);

        // Keep the service alive until shutdown.
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private static async Task DeclareTopologyAsync(IChannel channel, CancellationToken ct)
    {
        await channel.ExchangeDeclareAsync(Topology.CommerceExchange, ExchangeType.Topic, durable: true, cancellationToken: ct);
        await channel.ExchangeDeclareAsync(Topology.DeadLetterExchange, ExchangeType.Topic, durable: true, cancellationToken: ct);

        await channel.QueueDeclareAsync(
            Topology.OrderPlacedDeadLetterQueue,
            durable: true, exclusive: false, autoDelete: false, cancellationToken: ct);
        await channel.QueueBindAsync(
            Topology.OrderPlacedDeadLetterQueue, Topology.DeadLetterExchange, Topology.OrderPlacedRoutingKey, cancellationToken: ct);

        var mainQueueArgs = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = Topology.DeadLetterExchange,
            ["x-dead-letter-routing-key"] = Topology.OrderPlacedRoutingKey
        };
        await channel.QueueDeclareAsync(
            Topology.OrderPlacedQueue,
            durable: true, exclusive: false, autoDelete: false, arguments: mainQueueArgs, cancellationToken: ct);
        await channel.QueueBindAsync(
            Topology.OrderPlacedQueue, Topology.CommerceExchange, Topology.OrderPlacedRoutingKey, cancellationToken: ct);
    }

    private async Task OnMessageAsync(object sender, BasicDeliverEventArgs args)
    {
        var channel = _channel!;
        try
        {
            var json = Encoding.UTF8.GetString(args.Body.Span);
            var message = JsonSerializer.Deserialize<IntegrationMessage<CommerceOrderPlaced>>(json, JsonOptions)
                          ?? throw new InvalidOperationException("Unparseable order.placed message");

            var fulfillment = await _wmsClient.RequestFulfillmentAsync(message, CancellationToken.None);

            // TODO Phase 2: POST fulfillment.status.changed.v1 to the plugin callback
            //   (services/worker -> nopCommerce /omnichannel/callbacks/fulfillment/status-changed)
            //   with X-Demo-Token auth and the standard envelope.
            _logger.LogInformation(
                "Fulfillment result order_guid={OrderGuid} status={Status} (callback post: TODO Phase 2)",
                fulfillment.OrderGuid, fulfillment.Status);

            await channel.BasicAckAsync(args.DeliveryTag, multiple: false);
        }
        catch (Exception exception)
        {
            // requeue:false → routed to DLQ via the queue's dead-letter args.
            _logger.LogError(exception, "Order.placed handling failed; dead-lettering message");
            await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
            await _channel.CloseAsync(cancellationToken);
        if (_connection is not null)
            await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
