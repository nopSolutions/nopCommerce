namespace Omnichannel.Worker;

/// <summary>
/// Bound from configuration (env vars / appsettings). All values have demo
/// defaults so the worker boots inside docker-compose with no extra config.
/// </summary>
public sealed class WorkerOptions
{
    public const string SectionName = "Worker";

    /// <summary>RabbitMQ connection (amqp URI).</summary>
    public string RabbitMqUri { get; set; } = "amqp://guest:guest@rabbitmq:5672/";

    /// <summary>Base URL of the WMS simulator.</summary>
    public string WmsBaseUrl { get; set; } = "http://wms-sim:8080";

    /// <summary>WMS fulfillment endpoint path.</summary>
    public string WmsFulfillmentPath { get; set; } = "/fulfillments";

    /// <summary>
    /// nopCommerce plugin callback base URL (where fulfillment.status.changed.v1
    /// is posted in Phase 2 — HTTP, not MQ, mirroring the POS callback shape).
    /// </summary>
    public string NopCommerceBaseUrl { get; set; } = "http://nopcommerce";

    /// <summary>Demo token sent to the plugin callback (X-Demo-Token).</summary>
    public string DemoToken { get; set; } = "omni-demo-token";

    /// <summary>Polly retry attempts on the WMS call before DLQ.</summary>
    public int MaxRetryAttempts { get; set; } = 5;

    /// <summary>Circuit-breaker: consecutive failures before the breaker opens.</summary>
    public int CircuitBreakerFailureThreshold { get; set; } = 5;

    /// <summary>Circuit-breaker: seconds the breaker stays open before half-open probe.</summary>
    public int CircuitBreakerBreakSeconds { get; set; } = 30;
}
