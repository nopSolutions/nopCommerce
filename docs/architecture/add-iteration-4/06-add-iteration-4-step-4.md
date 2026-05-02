# ADD Iteration 4 — Step 4: Instantiate Elements and Allocate Responsibilities

This iteration produces one nopCommerce plugin (`Nop.Plugin.Shipping.CarrierWebhook`) covering the full bidirectional carrier channel — outbound booking and inbound status updates.

---

## Schema Additions

**`Shipment` entity (four new nullable columns):** `ExternalShipmentId` (varchar 128), `ExternalCarrierCode` (varchar 32), `ExternalShippingStatus` (varchar 64), `LastStatusOccurredAtUtc`. Composite index on `(ExternalCarrierCode, ExternalShipmentId)` for the inbound consumer's correlation lookup.

**New table `CarrierWebhookEvent`:** audit log — every inbound receipt (including rejected and malformed ones) is recorded before any processing. Columns: `EventId`, `ReceivedAtUtc`, `RawPayload`, `RemoteIp`, `Outcome`, `Notes`.

**New table `ProcessedCarrierEvent`:** dedup store — unique key on `EventId`; checked before applying any status update.

---

## Components

| Component | Type | Responsibility |
|---|---|---|
| `CarrierWebhookPlugin` | `BasePlugin` | Plugin entry; creates new tables and topology on install |
| `CarrierWebhookController` | ASP.NET Core controller | `POST /api/carrier/webhook`; authenticates (bearer token); writes audit row; publishes to `verdemart.carrier.status`; returns 200 before processing starts |
| `ICarrierWebhookAuditService` | Scoped service | Writes to `CarrierWebhookEvent` on receipt; amends outcome after consumer processing |
| `IProcessedCarrierEventRepository` | Data access | Dedup check and insert on `ProcessedCarrierEvent`; unique constraint guards against concurrent races |
| `IExternalStatusMapper` | Singleton | Maps carrier vocabulary (`IN_TRANSIT`, `DELIVERED`, …) to nopCommerce `ShippingStatus`; returns `null` for values with no internal equivalent |
| `CarrierStatusConsumer` | `IHostedService` | Drains `verdemart.carrier.status`; deduplicates by `EventId`; applies out-of-order guard (`OccurredAtUtc`); updates `Shipment` + enqueues customer email in one DB transaction; NACKs to DLQ after threshold |
| `ShipmentSentEventConsumer` | `IConsumer<ShipmentSentEvent>` | Writes a `carrier.booking.requested` row to the existing Outbox on shipment dispatch |
| `CarrierBookingConsumer` | `IHostedService` | Drains `verdemart.carrier.booking.requested`; calls `IWireMockClient`; stores returned `ExternalShipmentId` on `Shipment`; idempotent (skips if `ExternalShipmentId` already set) |
| `IWireMockClient` | Typed HTTP client | `BookShipmentAsync` — calls WireMock booking endpoint; bounded retry-with-backoff for transient errors; returns structured result (`Booked / TransientFailure / PermanentFailure`) |
| `CarrierWebhookSettings` | `ISettings` | `InboundBearerToken`, `WireMockBaseUrl`, `WireMockTimeoutMs`, `MaxStatusRedeliveries`, `MaxBookingRedeliveries`, `CarrierCode` |
| `PluginNopStartup` | `INopStartup` | Registers all services; starts both background consumers as `IHostedService` |

---

## RabbitMQ Topology Additions

| Element | Name | Properties |
|---|---|---|
| Exchange | `verdemart.carrier.booking` | direct, durable |
| Queue | `verdemart.carrier.booking.requested` | durable, manual ack |
| Exchange | `verdemart.carrier.status` | direct, durable |
| Queue | `verdemart.carrier.status.received` | durable, manual ack, dead-letter → `verdemart.carrier.status.dlx` |
| DLQ | `verdemart.carrier.status.dlq` | durable |
| DLQ (booking) | `verdemart.carrier.booking.dlq` | durable (symmetric pattern) |

---

## Component Relationships

```
[ Outbound — shipment dispatch ]

ShipmentSentEvent
    ▼
ShipmentSentEventConsumer → INSERT Outbox (carrier.booking.requested)
    ▼ (OutboxDispatcherTask, Iter 2)
verdemart.carrier.booking
    ▼
CarrierBookingConsumer → IWireMockClient → WireMock
    └── on success: SET Shipment.ExternalShipmentId

[ Inbound — carrier status update ]

POST /api/carrier/webhook
    ▼
CarrierWebhookController
    ├── ICarrierWebhookAuditService (write audit row)
    └── publish → verdemart.carrier.status
    ▼
CarrierStatusConsumer
    ├── IProcessedCarrierEventRepository (dedup)
    ├── IExternalStatusMapper
    ├── UPDATE Shipment (status + timestamp)
    └── IWorkflowMessageService (enqueue customer email)
```

---

## What Step 5 Will Do

Step 5 defines the key interfaces — audit service, dedup repository, status mapper, carrier client, HTTP contract, and the inbound/outbound message shapes.
