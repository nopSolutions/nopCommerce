using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VerdeMart.OpenBoxesBridge.Dedup;
using VerdeMart.OpenBoxesBridge.Messaging;

namespace VerdeMart.OpenBoxesBridge.Workers;

public class BridgeWorker : BackgroundService
{
    private readonly BridgeSettings _settings;
    private readonly IDedupRepository _dedup;
    private readonly RabbitMqTopology _topology;
    private readonly CircuitBreaker _circuit;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BridgeWorker> _logger;

    public BridgeWorker(
        IOptions<BridgeSettings> options,
        IDedupRepository dedup,
        RabbitMqTopology topology,
        CircuitBreaker circuit,
        IServiceScopeFactory scopeFactory,
        ILogger<BridgeWorker> logger)
    {
        _settings = options.Value;
        _dedup = dedup;
        _topology = topology;
        _circuit = circuit;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("VerdeMart.OpenBoxesBridge starting");

        await _dedup.EnsureSchemaAsync(stoppingToken);
        _logger.LogInformation("Dedup schema ready ({ConnectionString})", _settings.DedupConnectionString);

        var factory = new ConnectionFactory
        {
            HostName = _settings.RabbitMqHost,
            Port = _settings.RabbitMqPort,
            UserName = _settings.RabbitMqUsername,
            Password = _settings.RabbitMqPassword,
            AutomaticRecoveryEnabled = true
        };

        await using var connection = await OpenConnectionWithRetryAsync(factory, stoppingToken);
        _logger.LogInformation("Connected to RabbitMQ at {Host}:{Port}", _settings.RabbitMqHost, _settings.RabbitMqPort);

        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _topology.EnsureAsync(channel, stoppingToken);

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: _settings.PrefetchCount, global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, args) =>
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var messageConsumer = scope.ServiceProvider.GetRequiredService<OrderPlacedMessageConsumer>();
            try
            {
                await messageConsumer.HandleAsync(channel, args, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in consumer dispatch");
            }
        };

        // Circuit breaker loop: start consuming, pause when circuit opens, resume when half-open.
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumerTag = await channel.BasicConsumeAsync(
                queue: _settings.OrderQueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation("Consuming queue {Queue} (circuit {State})",
                _settings.OrderQueueName, _circuit.State);

            // Wait while the circuit is healthy (Closed or HalfOpen).
            while (_circuit.ShouldAttempt() && !stoppingToken.IsCancellationRequested)
                await Task.Delay(200, stoppingToken);

            if (stoppingToken.IsCancellationRequested)
                break;

            // Circuit opened — stop accepting new messages. In-flight messages complete
            // normally; they will nack with requeue=true back to the queue.
            try
            {
                await channel.BasicCancelAsync(consumerTag, cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "BasicCancelAsync failed — channel may be recovering");
            }

            _logger.LogInformation("Consumer paused — waiting for circuit to reach half-open state");

            // Wait until the circuit timeout elapses and ShouldAttempt returns true again.
            while (!_circuit.ShouldAttempt() && !stoppingToken.IsCancellationRequested)
                await Task.Delay(1000, stoppingToken);

            _logger.LogInformation("Circuit half-open — resuming consumer for probe");
        }

        _logger.LogInformation("VerdeMart.OpenBoxesBridge stopping");
    }

    private async Task<IConnection> OpenConnectionWithRetryAsync(ConnectionFactory factory, CancellationToken ct)
    {
        var delay = TimeSpan.FromSeconds(2);
        var maxDelay = TimeSpan.FromSeconds(30);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                return await factory.CreateConnectionAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RabbitMQ not reachable yet — retrying in {Delay}s", delay.TotalSeconds);
                await Task.Delay(delay, ct);
                delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, maxDelay.TotalSeconds));
            }
        }

        throw new OperationCanceledException(ct);
    }
}
