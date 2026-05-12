#nullable enable
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Nop.Plugin.Messaging.RabbitMq.Services;

public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private IConnection? _connection;
    private RabbitMqSettings? _settings;
    private bool _topologyDeclared;

    public RabbitMqConnectionFactory(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<IChannel> CreateChannelAsync()
    {
        await EnsureConnectedAsync();
        var channel = await _connection!.CreateChannelAsync();

        if (!_topologyDeclared)
        {
            await _lock.WaitAsync();
            try
            {
                if (!_topologyDeclared)
                    await DeclareTopologyAsync(channel);
            }
            finally
            {
                _lock.Release();
            }
        }

        return channel;
    }

    private async Task EnsureConnectedAsync()
    {
        if (_connection is { IsOpen: true })
            return;

        await _lock.WaitAsync();
        try
        {
            if (_connection is { IsOpen: true })
                return;

            _settings ??= LoadSettings();

            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            _connection = await factory.CreateConnectionAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    private RabbitMqSettings LoadSettings()
    {
        using var scope = _scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<RabbitMqSettings>();
    }

    private async Task DeclareTopologyAsync(IChannel channel)
    {
        await channel.ExchangeDeclareAsync(
            exchange: _settings!.ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        await channel.QueueDeclareAsync(
            queue: _settings.OrderPlacedQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: _settings.OrderPlacedQueueName,
            exchange: _settings.ExchangeName,
            routingKey: "order.placed");

        _topologyDeclared = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}
