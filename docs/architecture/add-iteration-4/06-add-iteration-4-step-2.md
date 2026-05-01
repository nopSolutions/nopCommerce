# ADD Iteration 4 — Step 2: Choose Element to Decompose

## What This Step Does

Step 2 selects the element of the system to design in this iteration. Everything from Step 3 onwards applies to this element only.

---

## Selected Element: The Carrier Integration Channel

The element to decompose is the **bidirectional channel between nopCommerce and the carrier (WireMock)**: outbound, the path from `ShipmentSentEvent` to a booking call that returns the carrier's tracking identifier; inbound, the path from a `shipment.status.updated` webhook to a customer-visible state change and a queued notification email.

This channel has two phases that are designed together but execute on different threads:

1. **Outbound phase — booking via the Outbox.** When `ShipmentSentEvent` fires, an event consumer writes a `carrier.booking.requested` row to the existing Outbox table. The Iter 2 dispatcher delivers it to a new consumer that POSTs to WireMock, receives the carrier tracking identifier, and stores it on `Shipment.ExternalShipmentId`. The admin UI does not block on WireMock.
2. **Inbound phase — webhook ingestion.** WireMock POSTs to a new endpoint exposed by the plugin. The endpoint authenticates, writes the raw payload to an audit table, publishes the event to a local queue, and returns 200 immediately. An async consumer dedups by event id, correlates by `ExternalShipmentId`, applies an out-of-order guard, updates state inside one DB transaction, and enqueues the customer email.

Both phases share the `Shipment` entity as their artifact and the carrier tracking identifier as their correlation key. Decomposing one without the other leaves QAS-5 unsatisfiable: the inbound webhook has nothing to correlate against without the outbound booking, and the outbound booking is purposeless without an inbound update.

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
                                       (some time later)
                                                       │
                                       │ POST /api/carrier/webhook
                                       │ (status update)
                                       ▼
┌────────────────────────────────────────────┐
│  CarrierWebhookController                  │   ← INBOUND, NEW
│  authenticate → audit-log → publish to     │
│  internal queue → 200 OK                    │
└────────────────────────────────────────────┘
   │
   ▼
┌────────────────────────────────────────────┐
│  CarrierStatusConsumer                     │   ← INBOUND, NEW
│  dedup by EventId → lookup Shipment by      │
│  ExternalShipmentId → out-of-order guard    │
│  → tx { update status, enqueue email }      │
└────────────────────────────────────────────┘
   │
   ▼
   Order detail page reflects the new ExternalShippingStatus
   QueuedEmail row inserted for the customer
```

The two boxes labelled "INBOUND, NEW" close QAS-5's response measure. The two outbound boxes establish the correlation key the inbound path needs.

---

## Responsibilities In Scope for This Iteration

| Responsibility | In scope |
| --- |---|
| Add `ExternalShipmentId`, `ExternalCarrierCode`, `ExternalShippingStatus`, `LastStatusOccurredAtUtc` columns to `Shipment` (CON-21) | Yes |
| Implement `IConsumer<ShipmentSentEvent>` that writes `carrier.booking.requested` to the Outbox (CON-22) | Yes |
| Implement `CarrierBookingConsumer` that drains the new Outbox event type and POSTs to WireMock | Yes |
| Implement webhook endpoint, authenticate it, persist the raw payload to an audit table (CON-20, CON-23) | Yes |
| Internal queue between webhook controller and processing consumer (CON-18, CON-24) | Yes |
| Dedup webhook events by carrier-supplied `eventId` (CON-18) | Yes |
| Out-of-order guard using carrier-supplied `occurredAtUtc` (CON-19) | Yes |
| Map external carrier vocabulary to internal `ShippingStatus` without modifying the existing enum (CON-21) | Yes |
| Hard-coded mapping table inside the plugin (CON-21) | Yes |
| Reuse `IWorkflowMessageService` to enqueue customer notification (no new email infrastructure) | Yes |
| DLQ for poison or unmatchable webhooks after redelivery threshold (CON-17, CON-18) | Yes |
| FluentMigrator migration adding the new columns + audit table | Yes |

---

## Responsibilities Explicitly Outside This Iteration

| Responsibility | Owner |
| --- |---|
| HMAC payload signing | Production hardening — bearer suffices for QAS-5 (CON-20) |
| Token rotation infrastructure | Operational secrets management — out of scope |
| Tracking URL, location, full carrier event history on the order page | UX polish — QAS-5 only requires status text visible (`04-qas.md:81`) |
| DLQ replay UI / CLI | Operational tooling — also pending from Iter 3 |
| Multi-carrier routing (different carriers, different APIs, per-shipment carrier selection) | One carrier (WireMock) is enough for QAS-5; `ExternalCarrierCode` is added pre-emptively to keep the door open |
| Returns-flow webhooks (`RETURNED` status triggering a refund workflow) | Out of scope; status is recorded but no downstream action is driven |
| Authenticated tenant-aware webhooks (multi-store) | Single-store demo; not architectural |
| Carrier rate-shopping or label printing | Outside the QAS-5 scope; would be a separate channel |
| Spike resolution for OpenBoxes API capability and empirical timing of QAS-1/QAS-2/QAS-4 | Carried from Iter 3; tracked separately |
| Cleanup of rejected-order DB rows | Carried operational residual from Iter 3 |

---

## Why This Element

- It is the smallest element that can satisfy QAS-5 — without both halves, the customer-visible state change cannot occur.
- The element is **bounded on both sides by existing patterns**: upstream by `ShipmentSentEvent` and the Iter 2 Outbox; downstream by `IWorkflowMessageService` and the existing order detail page rendering. Nothing between those boundaries is touched in core nopCommerce.
- The seam where shipping state lives in nopCommerce is named in `02-current-state.md`: the `Shipment` entity. Adding columns there is the minimal change that makes external correlation possible, and the new columns are scoped to "external" so existing internal logic is unaffected.
- Choosing this element forces the three structural questions deferred from Step 1 — webhook auth (CON-20), external-vocabulary preservation (CON-21), and out-of-order handling (CON-19) — into Step 3, where they are evaluated against named alternatives.
- The internal queue between webhook controller and processing consumer is the response to CON-24 (10-second budget). Returning 200 from the controller before processing means the carrier never sees a slow response and never retries because of nopCommerce-internal latency.

---

## What Step 3 Will Do

Step 3 identifies the design concepts and tactics this element applies, each paired with the alternative considered and the reason for rejection. The open decisions Step 3 must close include:

- Whether the webhook controller processes inline or hands off to an internal queue
- The authentication mechanism for the inbound endpoint
- How carrier vocabulary is preserved without coarsening (`ShippingStatus` extension vs. parallel field)
- The dedup primitive — `eventId` vs. `(shipmentId, status, occurredAtUtc)` vs. last-write-wins
- Whether the outbound booking rides the existing Outbox or uses a separate path
- The DLQ topology for unmatchable inbound webhooks
