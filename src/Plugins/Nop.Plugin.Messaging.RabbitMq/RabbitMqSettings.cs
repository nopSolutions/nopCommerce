using Nop.Core.Configuration;

namespace Nop.Plugin.Messaging.RabbitMq;

public class RabbitMqSettings : ISettings
{
    public string Host { get; set; } = "rabbitmq";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "verdemart.orders";
    public string OrderPlacedQueueName { get; set; } = "verdemart.orders.openboxes";
}
