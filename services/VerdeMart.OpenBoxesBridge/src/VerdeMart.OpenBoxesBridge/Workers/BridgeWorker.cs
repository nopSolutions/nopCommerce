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
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BridgeWorker> _logger;

    public BridgeWorker(
        IOptions<BridgeSettings> options,
        IDedupRepository dedup,
        RabbitMqTopology topology,
        IServiceScopeFactory scopeFactory,
        ILogger<BridgeWorker> logger)
    {
        _settings = options.Value;
        _dedup = dedup;
        _topology = topology;
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
            // Per-message scope: each message gets its own OrderPlacedMessageConsumer instance
            // Aligns with Step 6 sequence diagram ("open per-message scope")
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

        await channel.BasicConsumeAsync(
            queue: _settings.OrderQueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("Consuming queue {Queue} (manual ack, prefetch={Prefetch})",
            _settings.OrderQueueName, _settings.PrefetchCount);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("VerdeMart.OpenBoxesBridge stopping");
        }
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
