namespace VerdeMart.OpenBoxesBridge;

public class BridgeSettings
{
    public string RabbitMqHost { get; set; } = "rabbitmq";
    public int RabbitMqPort { get; set; } = 5672;
    public string RabbitMqUsername { get; set; } = "guest";
    public string RabbitMqPassword { get; set; } = "guest";

    public string OrderQueueName { get; set; } = "verdemart.orders.openboxes";
    public string DeadLetterExchange { get; set; } = "verdemart.orders.dlx";
    public string DeadLetterQueue { get; set; } = "verdemart.orders.openboxes.dlq";

    public int MaxRedeliveryAttempts { get; set; } = 5;
    public ushort PrefetchCount { get; set; } = 10;

    public string OpenBoxesBaseUrl { get; set; } = "http://openboxes:8080/openboxes/";
    public string OpenBoxesUsername { get; set; } = "admin";
    public string OpenBoxesPassword { get; set; } = "password";

    public string OpenBoxesOriginLocationId { get; set; } = "1";
    public string OpenBoxesDestinationLocationName { get; set; } = "VerdeMart Store";
    // Explicit ID override — when set, skips name-based lookup/creation.
    public string OpenBoxesDestinationLocationId { get; set; } = "";
    public string OpenBoxesRequestedByPersonId { get; set; } = "1";

    public string DedupConnectionString { get; set; } = "Data Source=/data/processed_orders.db";
}
