using System.Text;
using RabbitMQ.Client;
using Nop.Services.Logging;

namespace Nop.Services.Integration.RabbitMQ;

/// <summary>
/// Represents a RabbitMQ message publisher implementation
/// </summary>
public partial class RabbitMqPublisher : IRabbitMqPublisher
{
    #region Fields

    private readonly ILogger _logger;
    private readonly string _hostname;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;

    #endregion

    #region Ctor

    public RabbitMqPublisher(ILogger logger)
    {
        _logger = logger;

        // TODO: Move to appsettings.json in full implementation
        _hostname = "rabbitmq";
        _port = 5672;
        _username = "guest";
        _password = "guest";
    }

    #endregion

    #region Methods

    /// <summary>
    /// Publishes a message to the specified exchange
    /// </summary>
    public async Task PublishAsync(string exchange, string routingKey, string messageBody)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                Port = _port,
                UserName = _username,
                Password = _password
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );

            var body = Encoding.UTF8.GetBytes(messageBody);

            channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: null,
                body: body
            );

            await _logger.InformationAsync($"Spike: Published message to RabbitMQ: exchange={exchange}, routingKey={routingKey}");
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Spike: Failed to publish message to RabbitMQ: {ex.Message}", ex);
            throw;
        }
    }

    #endregion
}
