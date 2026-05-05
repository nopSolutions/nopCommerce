# ADD Iteration 4 — Step 2: Choose Element to Decompose

## What This Step Does

Step 2 selects the element of the system to design in this iteration. Everything from Step 3 onwards applies to this element only.

---

## Selected Element: The Carrier Integration Channel

The element to decompose is the **bidirectional channel between nopCommerce and the carrier (WireMock)**: outbound, the path from `ShipmentSentEvent` to a booking call that returns the carrier's tracking identifier; inbound, the path from a scheduled poll of the carrier API to a customer-visible state change and a queued notification email.

This channel has two phases that are designed together but execute on different threads:

1. **Outbound phase — booking via the Outbox.** When `ShipmentSentEvent` fires, an event consumer writes a `carrier.booking.requested` row to the existing Outbox table. The Iter 2 dispatcher delivers it to a new consumer that POSTs to WireMock, receives the carrier tracking identifier, and stores it on `Shipment.ExternalShipmentId`. The admin UI does not block on WireMock.
2. **Inbound phase — carrier status polling.** A scheduled `IScheduleTask` runs every 30 seconds. On each tick it queries WireMock for the current status of every shipment that has an `ExternalShipmentId` and is not in a terminal state. When a status change is detected, the task updates `Shipment.ExternalShippingStatus` and enqueues a customer notification email in a single DB transaction.

Both phases share the `Shipment` entity as their artifact and the carrier tracking identifier as their correlation key. Decomposing one without the other leaves QAS-5 unsatisfiable: the inbound poll has nothing to correlate against without the outbound booking, and the outbound booking is purposeless without an inbound detection loop.

---

## Where It Lives in the System

```
ADMIN — create shipment                           CARRIER (WireMock)
   │                                                   │
   │ ShipmentSentEvent (existing)                       │
   ▼                                                   │
┌────────────────────────────────────────────┐         │
│  IConsumer<ShipmentSentEvent>              │         │
│  (new — inside the plugin)                  │         │
│  writes to Outbox: carrier.booking.requested│         │
└────────────────────────────────────────────┘         │
   │                                                   │
   │ Outbox dispatcher (Iter 2 — unchanged)             │
   ▼                                                   │
┌────────────────────────────────────────────┐         │
│  CarrierBookingConsumer                    │         │
│  (new — drains carrier.booking.requested)  │         │
│  HTTP POST → WireMock /shipments            │ ───────▶│
│  on response: store ExternalShipmentId      │ ◀───────│
└────────────────────────────────────────────┘         │
                                                       │
                                       (every 30 s)
                                                       │
┌────────────────────────────────────────────┐         │
│  CarrierStatusPollerTask                   │ ───────▶│ GET /api/shipments/{id}/status
│  (new — IScheduleTask, every 30 s)         │ ◀───────│
│  for each open shipment with ExternalShipmentId:     │
│    compare returned status to stored status          │
│    on change: UPDATE Shipment.ExternalShippingStatus │
│    on change: enqueue customer email                 │
└────────────────────────────────────────────┘
   │
   ▼
   Order detail page reflects the new ExternalShippingStatus
   QueuedEmail row inserted for the customer
```

---

## Responsibilities In Scope for This Iteration

| Responsibility | In scope |
| --- |---|
| Add `ExternalShipmentId`, `ExternalCarrierCode`, `ExternalShippingStatus`, `LastStatusOccurredAtUtc` columns to `Shipment` (CON-21) | Yes |
| Implement `IConsumer<ShipmentSentEvent>` that writes `carrier.booking.requested` to the Outbox (CON-22) | Yes |
| Implement `CarrierBookingConsumer` that drains the new Outbox event type and POSTs to WireMock | Yes |
| Implement `CarrierStatusPollerTask` that polls WireMock every 30 s and reflects status changes | Yes |
| Map external carrier vocabulary to internal `ShippingStatus` without modifying the existing enum (CON-21) | Yes |
| Hard-coded mapping table inside the plugin (CON-21) | Yes |
| Reuse `IWorkflowMessageService` to enqueue customer notification (no new email infrastructure) | Yes |
| FluentMigrator migration adding the new columns | Yes |

---

## Responsibilities Explicitly Outside This Iteration

| Responsibility | Owner |
| --- |---|
| Tracking URL, location, full carrier event history on the order page | UX polish — QAS-5 only requires status text visible |
| Multi-carrier routing (different carriers, different APIs, per-shipment carrier selection) | One carrier (WireMock) is enough for QAS-5; `ExternalCarrierCode` is added pre-emptively to keep the door open |
| Returns-flow handling (`RETURNED` status triggering a refund workflow) | Out of scope; status is recorded but no downstream action is driven |
| Authenticated tenant-aware requests (multi-store) | Single-store demo; not architectural |
| Carrier rate-shopping or label printing | Outside the QAS-5 scope; would be a separate channel |
| Spike resolution for OpenBoxes API capability and empirical timing of QAS-1/QAS-2/QAS-4 | Carried from Iter 3; tracked separately |
| Cleanup of rejected-order DB rows | Carried operational residual from Iter 3 |

---

## Why This Element

- It is the smallest element that can satisfy QAS-5 — without both halves, the customer-visible state change cannot occur.
- The element is **bounded on both sides by existing patterns**: upstream by `ShipmentSentEvent` and the Iter 2 Outbox; downstream by `IWorkflowMessageService` and the existing order detail page rendering. Nothing between those boundaries is touched in core nopCommerce.
- The seam where shipping state lives in nopCommerce is named in `02-current-state.md`: the `Shipment` entity. Adding columns there is the minimal change that makes external correlation possible, and the new columns are scoped to "external" so existing internal logic is unaffected.
- Choosing this element forces the structural questions deferred from Step 1 — external-vocabulary preservation (CON-21) and concurrent poll safety (CON-19) — into Step 3, where they are evaluated against named alternatives.

---

## What Step 3 Will Do

Step 3 identifies the design concepts and tactics this element applies, each paired with the alternative considered and the reason for rejection. The open decisions Step 3 must close include:

- Whether outbound booking rides the existing Outbox or uses a separate path
- How carrier vocabulary is preserved without coarsening (`ShippingStatus` extension vs. parallel field)
- The dedup primitive for the poller — last-write-wins vs. timestamp guard
- The polling hosting model — `IScheduleTask` vs. a separate `BackgroundService`
