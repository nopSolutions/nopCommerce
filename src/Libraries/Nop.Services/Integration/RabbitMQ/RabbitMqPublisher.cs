using System.Text;
using Nop.Core.Configuration;
using Nop.Services.Logging;
using RabbitMQ.Client;

namespace Nop.Services.Integration.RabbitMQ;

public partial class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly ILogger _logger;
    private readonly IntegrationConfig _integrationConfig;

    public RabbitMqPublisher(ILogger logger, IntegrationConfig integrationConfig)
    {
        _logger = logger;
        _integrationConfig = integrationConfig;
    }

    public async Task PublishAsync(string exchange, string routingKey, string messageBody)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _integrationConfig.RabbitMqHostname,
                Port = _integrationConfig.RabbitMqPort,
                UserName = _integrationConfig.RabbitMqUsername,
                Password = _integrationConfig.RabbitMqPassword
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            var body = Encoding.UTF8.GetBytes(messageBody);

            channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"RabbitMQ publish failed (exchange={exchange}, routingKey={routingKey}): {ex.Message}", ex);
            throw;
        }
    }
}
