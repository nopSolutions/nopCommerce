# ADD Iteration 4 — Step 4: Instantiate Elements and Allocate Responsibilities

This iteration produces one nopCommerce plugin (`Nop.Plugin.Shipping.CarrierWebhook`) covering the full bidirectional carrier channel — outbound booking and inbound status polling.

---

## Schema Additions

**`Shipment` entity (four new nullable columns):** `ExternalShipmentId` (varchar 128), `ExternalCarrierCode` (varchar 32), `ExternalShippingStatus` (varchar 64), `LastStatusOccurredAtUtc`. Composite index on `(ExternalCarrierCode, ExternalShipmentId)` for the poller's correlation lookup.

---

## Components

| Component | Type | Responsibility |
| --- | --- | --- |
| `CarrierWebhookPlugin` | `BasePlugin` | Plugin entry; creates new topology on install |
| `CarrierStatusPollerTask` | `IScheduleTask` | Runs every 30 s; fetches current status from WireMock for each open shipment; on status change updates `Shipment.ExternalShippingStatus` and enqueues customer email in one DB transaction |
| `IExternalStatusMapper` | Singleton | Maps carrier vocabulary (`IN_TRANSIT`, `DELIVERED`, …) to nopCommerce `ShippingStatus`; returns `null` for values with no internal equivalent |
| `ShipmentSentEventConsumer` | `IConsumer<ShipmentSentEvent>` | Writes a `carrier.booking.requested` row to the existing Outbox on shipment dispatch |
| `CarrierBookingConsumer` | `IHostedService` | Drains `verdemart.carrier.booking.requested`; calls `IWireMockClient`; stores returned `ExternalShipmentId` on `Shipment`; idempotent (skips if `ExternalShipmentId` already set) |
| `IWireMockClient` | Typed HTTP client | `BookShipmentAsync` — calls WireMock booking endpoint; `GetShipmentStatusAsync` — polls current status by `ExternalShipmentId`; bounded retry-with-backoff for transient errors |
| `CarrierWebhookSettings` | `ISettings` | `WireMockBaseUrl`, `WireMockTimeoutMs`, `MaxBookingRedeliveries`, `CarrierCode` |
| `PluginNopStartup` | `INopStartup` | Registers all services; starts `CarrierBookingConsumer` as `IHostedService`; registers `CarrierStatusPollerTask` with the nopCommerce schedule task engine |

---

## RabbitMQ Topology Additions

| Element | Name | Properties |
| --- | --- | --- |
| Exchange | `verdemart.carrier.booking` | direct, durable |
| Queue | `verdemart.carrier.booking.requested` | durable, manual ack |
| DLQ | `verdemart.carrier.booking.dlq` | durable |

No inbound queue or dead-letter exchange is added. The inbound polling path uses HTTP, not AMQP.

---

## Component Relationships

```text
[ Outbound — shipment dispatch ]

ShipmentSentEvent
    ▼
ShipmentSentEventConsumer → INSERT Outbox (carrier.booking.requested)
    ▼ (OutboxDispatcherTask, Iter 2)
verdemart.carrier.booking
    ▼
CarrierBookingConsumer → IWireMockClient.BookShipmentAsync → WireMock
    └── on success: SET Shipment.ExternalShipmentId

[ Inbound — carrier status polling ]

CarrierStatusPollerTask (every 30 s)
    ▼
IWireMockClient.GetShipmentStatusAsync → WireMock
    ▼
for each shipment where returnedStatus ≠ ExternalShippingStatus:
    IExternalStatusMapper
    UPDATE Shipment (ExternalShippingStatus + LastStatusOccurredAtUtc)
    IWorkflowMessageService (enqueue customer email)
    — all inside one DB transaction per shipment
```

---

## What Step 5 Will Do

Step 5 defines the key interfaces — status mapper, carrier client, polling response shape, and the outbound message shape.
