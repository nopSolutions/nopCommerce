using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace OutboxSpike;

public sealed class RabbitPublisher : IDisposable
{
    private const string ExchangeName = "verdemart.orders";
    private const string QueueName = "verdemart.orders.openboxes";
    private const string RoutingKey = "order.placed";

    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitPublisher(string host = "localhost", int port = 5672)
    {
        _factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = "guest",
            Password = "guest",
            RequestedConnectionTimeout = TimeSpan.FromSeconds(3),
            SocketReadTimeout = TimeSpan.FromSeconds(3),
            SocketWriteTimeout = TimeSpan.FromSeconds(3),
        };
    }

    // Opens a fresh connection + channel. Throws BrokerUnreachableException if broker is down.
    private void EnsureConnected()
    {
        if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
            return;

        _channel?.Dispose();
        _connection?.Dispose();

        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: true);

        // Declare with DLQ args — matches the topology already present in the broker
        // (ADR-003: durable queues + dead-letter handling)
        var args = new Dictionary<string, object>
        {
            ["x-dead-letter-exchange"]     = "",
            ["x-dead-letter-routing-key"]  = $"{QueueName}.dlq",
        };
        _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: args);
        _channel.QueueBind(QueueName, ExchangeName, RoutingKey);
    }

    public void Publish(string id, string payload)
    {
        EnsureConnected();

        var props = _channel!.CreateBasicProperties();
        props.Persistent = true;   // delivery mode 2 — survives broker restart
        props.MessageId = id;
        props.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        var body = Encoding.UTF8.GetBytes(payload);
        _channel.BasicPublish(ExchangeName, RoutingKey, props, body);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
