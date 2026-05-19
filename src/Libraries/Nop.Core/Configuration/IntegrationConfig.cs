namespace Nop.Core.Configuration;

/// <summary>
/// Configuration for the omnichannel integration layer (RabbitMQ outbox / consumers)
/// </summary>
public partial class IntegrationConfig : IConfig
{
    public string RabbitMqHostname { get; protected set; } = "rabbitmq";

    public int RabbitMqPort { get; protected set; } = 5672;

    public string RabbitMqUsername { get; protected set; } = "guest";

    public string RabbitMqPassword { get; protected set; } = "guest";

    public string EventsExchange { get; protected set; } = "verdemart.events";

    public string StockUpdatedQueue { get; protected set; } = "nopcommerce.stock-updated";

    public string StockUpdatedRoutingKey { get; protected set; } = "stock.updated";

    /// <summary>
    /// Max age (seconds) of last successful outbox publish before /integration/health reports degraded
    /// </summary>
    public int HealthDegradedAfterSeconds { get; protected set; } = 60;
}
