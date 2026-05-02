# ADD Iteration 5 — Step 6: Sketch Views and Record Design Decisions

## Updated Component View

```text
[ nopCommerce process ]

┌─────────────────────────────────────────────────────────────────┐
│  Nop.Plugin.Inventory.AllocationGate (extended)                  │
│                                                                  │
│  AllocationGate + AllocationApiController  (unchanged — Iter 3) │
│  ReleaseExpiredReservationsTask            (unchanged — Iter 3) │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  OpenBoxesStatusPollerTask  (NEW)                         │   │
│  │   ├── IOpenBoxesClient.GetIssuedFulfillmentOrdersAsync() │   │
│  │   ├── IOrderService.GetOrderByGuidAsync()                │   │
│  │   ├── IShipmentService.InsertShipmentAsync()             │   │
│  │   ├── IOrderProcessingService (status transition)        │   │
│  │   └── IOutboxRepository (carrier booking trigger)        │   │
│  └──────────────────────────────────────────────────────────┘   │
└──────────────────────────────┬──────────────────────────────────┘
                               │ GET /api/generic/shipment?status=ISSUED
                               │ (every 30 s)
                               ▼
                          ┌──────────┐
                          │ OpenBoxes │
                          └──────────┘
```

## Sequence: Fulfillment State Detected

```text
OpenBoxesStatusPollerTask
    │ (every 30 s)
    │ GET /api/generic/shipment?status=ISSUED
    ▼
OpenBoxes REST API
    │ returns [ { referenceNumber: OrderGuid, status: ISSUED } ]
    ▼
OpenBoxesStatusPollerTask
    │ GetOrderByGuidAsync(OrderGuid)
    │ if Shipment exists + ExternalShipmentId set → skip
    │ else → BEGIN TRANSACTION
    │           InsertShipmentAsync (+ ShipmentItem per order item)
    │             fires ShipmentCreatedEvent
    │           SetOrderStatus(Complete)
    │           write outbox row (carrier.booking.requested, ShipmentId in payload)
    │        COMMIT  (rollback on any failure → next tick retries)
    ▼
OutboxDispatcherTask (existing — Iter 2)
    │ publishes carrier.booking.requested
    ▼
CarrierBookingConsumer (existing — Iter 4)
    │ calls WireMock booking endpoint
    │ writes ExternalShipmentId back to Shipment row
    ▼
Shipment.ExternalShipmentId populated
    │
    ▼ (later)
CarrierStatusConsumer (existing — Iter 4)
    receives webhook → customer sees tracking status
```

---

## Decision Recorded

- [ADR-015 — OpenBoxes Fulfillment State via Scheduled Polling](../07-adrs/ADR-015-openboxes-status-polling.md)

---

## What This Iteration Produced

| Artefact | Description |
| --- | --- |
| `OpenBoxesStatusPollerTask` | New `IScheduleTask` inside `Nop.Plugin.Inventory.AllocationGate` |
| `IOpenBoxesClient` extension | New `GetIssuedFulfillmentOrdersAsync` method |
| `OpenBoxesFulfillmentOrder` DTO | Correlation record from OpenBoxes API |
| `AllocationSettings` extension | Four new fields for OpenBoxes URL, API key, interval, batch size |
| ADR-015 | Polling over webhook — deliberate choice for reliability and unidirectional dependency |

Step 7 verifies the design against QAS-5's warehouse visibility clause and closes the iteration.
