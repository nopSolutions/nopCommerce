# ADD Iteration 4 — Step 3: Identify Design Concepts

## What This Step Does

Step 3 selects the architectural tactics and patterns that the carrier integration channel applies. Each concept is paired with the alternative considered and the reason for rejection — the traceability that justifies each decision in the ADRs that close the iteration.

Six concepts are selected. The first two address the **inbound polling path** and the QAS-5 response measure directly; the next two address the **outbound booking path** that establishes the correlation key; the last two close cross-cutting concerns (status vocabulary and concurrent update safety).

---

## Inbound Phase — Carrier Status Polling

### 1. Scheduled Polling via `IScheduleTask`; nopCommerce Owns the Detection Cadence

**Driver:** QAS-5 — visible to customer within 30 s; reliability guarantee must be unconditional and not dependent on the carrier sending anything.

**Concept:** A new `CarrierStatusPollerTask` implements `IScheduleTask` and runs every 30 seconds. On each tick it fetches the current status for every `Shipment` that has an `ExternalShipmentId` and is not in a terminal state (`Delivered`, `ShippingNotRequired`). For each shipment, it compares the returned status string against `Shipment.ExternalShippingStatus`. If they differ, it updates the shipment and enqueues a customer notification email inside one DB transaction. If WireMock is unreachable, the tick logs the error and returns — no state is lost; the next tick retries the same reads.

Polling reads current state on every tick. After any outage of any duration, the next tick recovers the correct state without any dependency on WireMock having retained a pending notification.

**Rejected alternatives:**

- *Inbound webhook from WireMock* — Rejected because WireMock does not implement webhook retry logic. A single failed delivery leaves nopCommerce silently diverged from carrier state with no self-healing path. QAS-5's guarantee cannot be satisfied by a mechanism the sender can abandon. Polling gives nopCommerce full ownership of the detection cadence.

- *`IHostedService` polling loop instead of `IScheduleTask`* — Rejected because `IScheduleTask` is the nopCommerce-native scheduling primitive, already used for outbox dispatch, queue drain, and similar periodic work. It is registered, monitored, and configured through the existing admin schedule-tasks UI with no additional infrastructure. A raw `BackgroundService` loop would replicate the scheduling and logging that `IScheduleTask` provides for free.

---

### 2. Last-Write-Wins on `ExternalShippingStatus`; No Dedup Table Required

**Driver:** CON-19 — concurrent poll ticks in a future multi-node deployment could both attempt to write the same status transition; the update must be safe under concurrency.

**Concept:** The poller applies a status update only when the value returned by WireMock differs from the value currently stored in `Shipment.ExternalShippingStatus`. The check and the update run inside one DB transaction; the unique correlation key (`ExternalShipmentId`) serialises concurrent writers at the row level. A concurrent tick that reads the same status as what was just committed will find no difference and skip — no duplicate email is enqueued. No separate dedup table is needed because the `Shipment` row itself is the dedup state.

**Rejected alternatives:**

- *Timestamp-based out-of-order guard* — Not applicable: polling always reads the current carrier state, not an event stream. There is no sequence of events that could arrive out of order — each tick returns one value representing the present state. A timestamp guard adds complexity for a problem that doesn't exist in the polling model.

- *Separate dedup table keyed on `(ExternalShipmentId, status)`* — Rejected because the `Shipment` row already holds `ExternalShippingStatus`; maintaining a separate store for the same fact is redundant and adds a migration without benefit.

---

## Outbound Phase — Booking via the Outbox

### 3. Outbound Booking Reuses Iter 2's Outbox

**Driver:** CON-22 — admin UI must not block on WireMock; ADR-004 — Outbox is the established pattern for guaranteed cross-process delivery; constraint from Step 1 — reuse, don't duplicate.

**Concept:** A new `IConsumer<ShipmentSentEvent>` inside the plugin runs at the natural trigger point. Its only job is to insert a row into the existing Outbox table with `EventType = "carrier.booking.requested"` and a JSON payload containing `ShipmentId`, `OrderId`, the customer's shipping address, and the line items in carrier-readable form. The existing `OutboxDispatcherTask` from Iter 2 picks it up on its next tick and publishes to a new RabbitMQ exchange (`verdemart.carrier.booking`) on routing key `carrier.booking.requested`. A new consumer (`CarrierBookingConsumer`) — running in the same nopCommerce process as a hosted service — drains the queue, POSTs to WireMock, and writes back the returned `ExternalShipmentId` and `ExternalCarrierCode` onto the `Shipment` row.

The admin UI returns as soon as `ShipmentSentEvent` fires; WireMock's latency is invisible to the operator. If WireMock is down, the outbox row sits and the dispatcher retries on schedule — same guarantee as the OpenBoxes path.

**Rejected alternatives:**

- *Synchronous HTTP POST inside the admin "create shipment" flow* — Rejected because (a) the admin UI now blocks on WireMock latency; (b) any WireMock outage propagates to a user-visible admin error and forces operators to retry manually; (c) the established pattern from ADR-004 already solves this; reinventing it is churn.

- *New separate outbox table for carrier events* — Rejected because the Iter 2 outbox is event-type-agnostic by design — `EventType` is a column. Forking the table doubles the operational surface (two retention policies, two dispatchers) for no architectural gain.

---

### 4. Booking Consumer Hosted In-Process Inside nopCommerce

**Driver:** CON-22 — no new deployable required for QAS-5; the brief's "≥1 independently deployable subsystem" requirement is already met by ADR-007.

**Concept:** `CarrierBookingConsumer` is a `BackgroundService` registered through `INopStartup` and runs in the same process as the nopCommerce web app. It connects to RabbitMQ on startup, subscribes to the carrier-booking queue, and processes messages with manual ack. Because the booking call writes back into nopCommerce's own database, in-process hosting avoids cross-process coordination on the writeback path.

This is the deliberate counterpart to ADR-007's bridge: the OpenBoxes bridge talks to an external system and writes nothing back into nopCommerce, so it lives outside; the carrier booking consumer talks to an external system and must update nopCommerce's `Shipment` row, so it lives inside.

**Rejected alternative:**

- *Separate deployable service for the carrier booking* — Rejected because it would need a separate path back into nopCommerce to update the `Shipment` row (another HTTP API, another auth surface, another deploy artefact) for no benefit. ADR-007's separation made sense for the OpenBoxes bridge because that bridge has no writeback; the carrier flow does.

---

## Cross-Cutting

### 5. External Status Preserved as a String Field; `ShippingStatus` Enum Untouched

**Driver:** CON-21 — coarsening loses information, extending core enum violates the plugin boundary; QAS-5 response — "tracking status visible to the customer" implies the customer sees the carrier vocabulary, not a coarsened summary.

**Concept:** A new `ExternalShippingStatus` column (varchar) on `Shipment` holds the raw carrier vocabulary verbatim — `IN_TRANSIT`, `OUT_FOR_DELIVERY`, etc. The existing `ShippingStatus` enum in `Nop.Core.Domain.Shipping` is left untouched and continues to serve nopCommerce-internal logic at its existing granularity. A small hard-coded mapper inside the plugin (`IExternalStatusMapper`) translates carrier vocab into the coarse internal enum only when the new external status crosses one of the internal enum's thresholds — e.g. `DELIVERED` triggers a `ShippingStatus.Delivered` transition; `IN_TRANSIT` and `OUT_FOR_DELIVERY` both leave `ShippingStatus.Shipped` unchanged. The order detail page renders `ExternalShippingStatus` when present and falls back to the internal enum otherwise.

**Rejected alternatives:**

- *Extend the `ShippingStatus` enum with new members* — Rejected because the enum lives in `Nop.Core` and is consumed by every shipping-related service in the framework. Adding members forces existing `switch` statements to handle new cases or break; the change is invasive and out of plugin scope.

- *Many-to-few mapping only, no external string field* — Rejected because the customer loses visibility of "Out for Delivery" — exactly the visibility QAS-5 set out to give them.

- *Introduce a parallel plugin-local enum and store the integer* — Rejected as more brittle than a string: a string survives carrier vocabulary additions without code change, while a new enum member forces a plugin redeploy for every carrier-side schema bump.

---

### 6. Hard-Coded Mapping Table Inside the Plugin

**Driver:** CON-21 — mapping policy must exist somewhere; simplicity over admin-configurability for a stable mapping; the carrier vocabulary is stable for the demo.

**Concept:** The mapping from carrier vocabulary → internal `ShippingStatus` lives inside `IExternalStatusMapper` as a `switch` expression. Adding a new carrier value requires a small code change and plugin redeploy; this is acceptable because carrier vocabularies are rarely revised and the change has obvious code-review semantics.

**Rejected alternative:**

- *DB-backed mapping table editable in the admin UI* — Rejected because it adds a CRUD UI, a migration, a settings surface, and an admin-procedure document for a mapping that changes maybe once a year. The simpler concept can be promoted to admin-configurable later if the operational reality demands it.

---

## Summary: Concepts → Drivers → Rejected Alternatives

| # | Concept | Driver(s) satisfied | Rejected alternative |
| --- | --- | --- | --- |
| 1 | Scheduled polling via `IScheduleTask` | QAS-5 | Inbound webhook; raw `BackgroundService` loop |
| 2 | Last-write-wins on `ExternalShippingStatus` | CON-19 | Timestamp guard; separate dedup table |
| 3 | Outbound booking via existing Outbox | CON-22 + ADR-004 | Sync admin call; new outbox table |
| 4 | Booking consumer in-process | CON-22 | Separate deployable |
| 5 | External status as string; enum untouched | CON-21 + QAS-5 | Extend enum; coarsen-only; new plugin enum |
| 6 | Hard-coded mapping in plugin | CON-21 | DB-configurable mapping |

CON-17 (race between poll tick and dispatch event committing `ExternalShipmentId`) is handled by concept 1: the poller skips any shipment where `ExternalShipmentId` is null. By the time the booking consumer writes the identifier, the next poll tick will pick it up.

---

## What Step 4 Will Do

Step 4 turns these concepts into named components: classes, files, schemas, registration points, and the precise shape of the Outbox event payload and polling response. The plugin layout, the `Shipment` schema migration, the new RabbitMQ exchange and queue, and the poller task are all defined there with enough precision for Step 5 to specify interfaces ready for implementation.
