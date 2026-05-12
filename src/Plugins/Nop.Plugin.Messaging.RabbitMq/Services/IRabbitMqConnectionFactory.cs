using RabbitMQ.Client;

namespace Nop.Plugin.Messaging.RabbitMq.Services;

public interface IRabbitMqConnectionFactory : IAsyncDisposable
{
    Task<IChannel> CreateChannelAsync();
}
