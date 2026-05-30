# Omnichannel.Contracts

Shared message-envelope library for the omnichannel integration path. Referenced
by the worker (and, conceptually, by the plugin — the plugin currently keeps its
own copies of these shapes in `Models/Callbacks/` and `OmnichannelCoreDefaults`).

This is the **boundary contract** that `plan.md` freezes at the end of Phase 2.
Change it only through a cross-pair review (envelope, event types, topology).

## Contents

| File | Purpose |
|------|---------|
| `Envelope.cs` | `MessageEnvelope` (messageId, correlationId, eventType, occurredOnUtc, source) and `IntegrationMessage<T>`. |
| `Events.cs` | `EventTypes` constants + payload records (`CommerceOrderPlaced`, `FulfillmentStatusChanged`, `PosStockChanged`). |
| `Topology.cs` | RabbitMQ exchange / queue / routing-key / DLX names. |

The records mirror the canonical samples in `docs/evidence/sample-*-v1.json`. Keep
all three in sync.

## Status

Scaffold (Phase 2 boundary work). The library compiles and is referenced by
`services/worker`. The plugin has not yet been switched over to consume it.
