namespace Omnichannel.Contracts;

/// <summary>
/// Shared integration message envelope. Every message that crosses a boundary
/// (plugin → bus → worker → WMS, POS → plugin) carries these four fields so the
/// whole path is traceable by <see cref="MessageId"/> and <see cref="CorrelationId"/>.
///
/// This mirrors the canonical samples in docs/evidence/sample-*-v1.json and the
/// fields persisted by OmniOutboxMessage / OmniInboxMessage in the plugin.
///
/// Per plan.md this envelope is the Pair-A/Pair-B boundary contract that is
/// frozen at the end of Phase 2. Change it only via a cross-pair review.
/// </summary>
public record MessageEnvelope
{
    /// <summary>Unique id of THIS message. Basis for inbox idempotency.</summary>
    public Guid MessageId { get; init; }

    /// <summary>Id shared by all messages in one business flow (one order).</summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>Versioned event type, e.g. <c>commerce.order.placed.v1</c>.</summary>
    public string EventType { get; init; } = string.Empty;

    /// <summary>When the event occurred (UTC).</summary>
    public DateTime OccurredOnUtc { get; init; }

    /// <summary>Originating system, e.g. <c>nopcommerce</c>, <c>wms-sim</c>.</summary>
    public string Source { get; init; } = string.Empty;
}

/// <summary>
/// Envelope + typed payload. Producers serialize this to JSON; consumers
/// deserialize and branch on <see cref="MessageEnvelope.EventType"/>.
/// </summary>
/// <typeparam name="TPayload">One of the records in Events.cs.</typeparam>
public record IntegrationMessage<TPayload> : MessageEnvelope
{
    public TPayload Payload { get; init; } = default!;
}
