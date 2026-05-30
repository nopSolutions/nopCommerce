namespace Omnichannel.Contracts;

/// <summary>
/// Canonical event type strings. Kept here so plugin, worker and simulators
/// agree on exact wire values. Mirrors OmnichannelCoreDefaults in the plugin.
/// </summary>
public static class EventTypes
{
    public const string CommerceOrderPlaced = "commerce.order.placed.v1";
    public const string FulfillmentStatusChanged = "fulfillment.status.changed.v1";
    public const string PosStockChanged = "pos.stock.changed.v1";
}

/// <summary>
/// Payload for <see cref="EventTypes.CommerceOrderPlaced"/>.
/// Produced by the plugin outbox, consumed by the worker.
/// Shape mirrors docs/evidence/sample-commerce-order-placed-v1.json.
/// </summary>
public record CommerceOrderPlaced
{
    public int OrderId { get; init; }
    public Guid OrderGuid { get; init; }
    public int StoreId { get; init; }
    public int CustomerId { get; init; }
    public string Currency { get; init; } = "EUR";
    public decimal TotalAmount { get; init; }
    public IReadOnlyList<OrderLine> Lines { get; init; } = Array.Empty<OrderLine>();
}

public record OrderLine
{
    public int ProductId { get; init; }
    public string Sku { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}

/// <summary>
/// Payload for <see cref="EventTypes.FulfillmentStatusChanged"/>.
/// Produced by the worker (after the WMS call), consumed by the plugin callback.
/// Shape mirrors docs/evidence/sample-fulfillment-status-changed-v1.json.
/// </summary>
public record FulfillmentStatusChanged
{
    public Guid OrderGuid { get; init; }
    public string ExternalRequestId { get; init; } = string.Empty;

    /// <summary>accepted | rejected | pending | failed</summary>
    public string Status { get; init; } = string.Empty;
    public string? Reason { get; init; }
}

/// <summary>
/// Payload for <see cref="EventTypes.PosStockChanged"/>.
/// Produced by the POS simulator, posted directly to the plugin callback.
/// Shape mirrors docs/evidence/sample-pos-stock-changed-v1.json and the
/// plugin's PosStockChangedRequest model.
/// </summary>
public record PosStockChanged
{
    public long SourceVersion { get; init; }
    public int ProductId { get; init; }
    public string Sku { get; init; } = string.Empty;
    public int WarehouseId { get; init; }
    public int QuantityOnHand { get; init; }
}
