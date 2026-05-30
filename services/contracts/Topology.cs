namespace Omnichannel.Contracts;

/// <summary>
/// RabbitMQ topology names shared by the plugin outbox publisher and the worker.
/// Documented in services/worker/README.md. Centralised here so producer and
/// consumer cannot drift on exchange / queue / routing-key spelling.
///
/// Topology (see README for the diagram):
///   exchange "commerce" (topic)
///     rk "commerce.order.placed.v1"      -> queue "wms.order.placed"
///   exchange "fulfillment" (topic)
///     rk "fulfillment.status.changed.v1" -> consumed by plugin callback (HTTP, not MQ in Phase 2)
///   dead-letter exchange "commerce.dlx" (topic) -> queue "wms.order.placed.dlq"
/// </summary>
public static class Topology
{
    public const string CommerceExchange = "commerce";
    public const string FulfillmentExchange = "fulfillment";
    public const string DeadLetterExchange = "commerce.dlx";

    public const string OrderPlacedRoutingKey = EventTypes.CommerceOrderPlaced;
    public const string FulfillmentRoutingKey = EventTypes.FulfillmentStatusChanged;

    public const string OrderPlacedQueue = "wms.order.placed";
    public const string OrderPlacedDeadLetterQueue = "wms.order.placed.dlq";
}
