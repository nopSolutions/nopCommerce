# ADD Iteration 4 — Step 1: Review Inputs

## Iteration Goal

Establish nopCommerce as **visible to the customer for cross-channel fulfillment progress** by addressing the carrier half of QAS-5. This iteration builds the carrier integration end to end: both the outbound booking call that establishes the carrier's tracking identifier on a `Shipment`, and the inbound polling path that turns carrier status changes into customer-visible state changes within 30 seconds of detection.

The two halves are the same integration, viewed from each direction. The outbound half exists to make the inbound half meaningful: without `ExternalShipmentId` on `Shipment`, the polling task has no correlation key to query against. Both are produced as one structural change.

---

## Inputs

### Primary Driver: QAS-5 (Visibility)

Inherited from the Step 7 verdict of Iteration 3: *"Iteration 4 begins with QAS-5 and the carrier integration via WireMock."*

| Field | Value |
| --- |---|
| Quality attribute | Visibility (cross-channel state propagation) |
| Stimulus | The carrier (WireMock) updates a shipment's tracking status |
| Source | Carrier system (WireMock) |
| Environment | Normal operation; shipment has been dispatched from OpenBoxes |
| Artifact | The order detail page in nopCommerce and the customer notification email |
| Response | nopCommerce detects the status change on the next poll tick; the tracking status is updated in the order record; a notification email is queued |
| Response measure | Order tracking status visible to the customer within 30 seconds of the state change occurring in the carrier system; email queued within the same window |

---

### Secondary Driver: Closing the Pressure Point on Inbound Integration

`02-current-state.md:84-90` named pressure point #7: *"no webhook ingestion or outbound integration pattern in the framework"*. Iteration 4 delivers the first concrete realisation of both halves of the carrier channel — outbound booking and inbound status detection — though the inbound half is implemented via scheduled polling (ADR-008) rather than webhook ingestion. The plugin produced here is a candidate template for any future carrier integration, but only **as a side effect**; QAS-5 alone does not justify generalisation, so this iteration delivers a single fit-for-purpose plugin.

---

### Inherited from Iteration 3 Step 7

| Inherited input | Source |
| --- |---|
| Primary driver: QAS-5 | Iter 3 Step 7 — next-iteration inputs |
| Surrounding system: WireMock as carrier simulator | `01-scenario.md:26`; `03-bounded-contexts.md` Shipping Context |
| New mechanism needed: inbound carrier state detection | `02-current-state.md:84-90` — pressure point #7 |
| Schema addition: `ExternalShipmentId` on `Shipment` | `04-qas.md` — explicitly named in QAS-5's design-decision-forced clause |
| Open spike: OpenBoxes API capability | Iter 3 Step 7 — out of scope for this iteration but tracked |
| Open spike: empirical timing of QAS-1, QAS-2, QAS-4 | Iter 3 Step 7 — out of scope for this iteration |
| Carry-over residual: rejected-order DB pollution cleanup | Iter 3 — operational, deferred |

---

### Constraints

| Constraint | Source |
| --- |---|
| nopCommerce remains the fixed commerce core | Carried from Iterations 1–3 |
| Integration code lives inside plugins | ADR-002 |
| RabbitMQ topology is fixed (`verdemart.orders` exchange) — new exchanges/queues for unrelated flows are allowed | ADR-001, ADR-003 |
| Outbox pattern exists and must be reused for outbound coordination, not duplicated | ADR-004 |
| Consumer-side idempotency is mandatory under at-least-once delivery | ADR-003 |
| `Shipment` is an existing nopCommerce entity in `Nop.Core.Domain.Shipping`; schema additions go through FluentMigrator | `Nop.Data` migration convention |
| WireMock's request/response shape is configurable but kept stable for the demo | `01-scenario.md` |
| Customer-visible response time bounded at 30 s end to end | QAS-5 response measure |

---

### Architectural Concerns

| Concern | Description |
| --- |---|
| CON-17 | Poll tick can race the dispatch event. The poller may read the carrier API before `CarrierBookingConsumer` has committed `ExternalShipmentId` to the `Shipment` row. The poll tick must skip shipments where `ExternalShipmentId` is null |
| CON-19 | Carrier status values are not ordered by arrival — the poller always reads the current state, so out-of-order is structurally impossible. However, concurrent poll ticks (future multi-node deployment) could both read and attempt to write the same status transition |
| CON-21 | `ShippingStatus` (the existing nopCommerce enum in `Nop.Core`) is coarse: `NotYetShipped, Shipped, Delivered, ShippingNotRequired`. Carrier vocabularies are richer (`PICKED_UP, IN_TRANSIT, OUT_FOR_DELIVERY, EXCEPTION, RETURNED`). Mapping many-to-few loses information; extending the enum violates the plugin boundary |
| CON-22 | The outbound booking call adds latency to the admin "create shipment" flow if invoked synchronously, and risks blocking the admin UI when WireMock is slow. The Outbox pattern from Iter 2 is the obvious reuse target |

---

### Relevant Existing Structures

| Element | Role |
| --- |---|
| `Shipment` entity (`Nop.Core.Domain.Shipping.Shipment`) | Existing entity. Will gain `ExternalShipmentId`, `ExternalCarrierCode`, `ExternalShippingStatus`, `LastStatusOccurredAtUtc` columns |
| `ShipmentSentEvent` | Existing domain event; the natural trigger point for outbound carrier booking |
| `ShippingStatus` enum (`Nop.Core.Domain.Shipping`) | Coarse internal status. Step 3 must decide whether to map carrier vocab into it or keep external status orthogonal |
| `IWorkflowMessageService` / `IQueuedEmailService` | Existing nopCommerce email-template infrastructure. Reused; no new mechanism needed |
| `OrderShipped.CustomerNotification` template | Existing template. Step 4 decides whether a new template is needed or this one is parameterised |
| Outbox table + `OutboxDispatcherTask` (Iter 2) | Reused for outbound carrier booking — a new event type rides the same dispatcher |
| RabbitMQ topology (`verdemart.orders` exchange) | Independent of this iteration's flows; this iteration adds its own exchange and queue for the outbound booking path only |
| ADR-002 plugin boundary | The carrier-integration plugin is the canonical example of inbound + outbound symmetry under ADR-002 |

---

## What Step 1 Establishes

- The iteration has a unified theme — **carrier integration as a complete bidirectional channel** — even though the QAS measures only the inbound half.
- QAS-5 is the primary driver; pressure point #7 is the secondary motivator and produces a reusable pattern as a side effect.
- The candidate concept inherited from Iter 3 (Outbox reuse for the outbound path) is the obvious starting point but Step 3 evaluates it against alternatives.
- Two new structural questions surface: how external carrier vocabulary is preserved without coarsening, and how concurrent poll ticks are handled safely.
- The brief's "≥1 independently deployable subsystem" requirement was already met by ADR-007 in Iter 3; nothing in Iteration 4 disturbs that, and this iteration deliberately keeps all new code inside a nopCommerce plugin.

Step 2 selects the element to decompose.
