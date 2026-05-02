# ADD Iteration 4 — Step 1: Review Inputs

## Iteration Goal

Establish nopCommerce as **visible to the customer for cross-channel fulfillment progress** by addressing the carrier half of QAS-5. This iteration builds the carrier integration end to end: both the outbound booking call that establishes the carrier's tracking identifier on a `Shipment`, and the inbound webhook path that turns carrier status updates into customer-visible state changes within ten seconds of receipt.

The two halves are the same integration, viewed from each direction. The outbound half exists to make the inbound half meaningful: without `ExternalShipmentId` on `Shipment`, an inbound `shipment.status.updated` webhook has nothing to correlate against. Both are produced as one structural change.

This iteration is also the first one that introduces an **inbound** integration channel. Iterations 1 through 3 produced a publish path, an outbox, an allocation gate, and a consumer — all push-out from nopCommerce or pull-in from a queue under nopCommerce's control. A carrier webhook arriving as an inbound HTTP POST from an external system is a new mechanism entirely; pressure point #7 in `02-current-state.md` named the gap explicitly.

---

## Inputs

### Primary Driver: QAS-5 (Visibility)

Inherited from the Step 7 verdict of Iteration 3: *"Iteration 4 begins with QAS-5 and the carrier integration via WireMock."*

| Field | Value |
| --- |---|
| Quality attribute | Visibility (cross-channel state propagation) |
| Stimulus | WireMock (carrier) sends a `shipment.status.updated` webhook — status changes to "In Transit" |
| Source | Carrier system (WireMock) |
| Environment | Normal operation; shipment has been dispatched from OpenBoxes |
| Artifact | The order detail page in nopCommerce and the customer notification email |
| Response | nopCommerce receives and processes the webhook; the tracking status is updated in the order record; a notification email is queued |
| Response measure | Order tracking status visible to the customer within 10 seconds of the webhook being received; email queued within the same window |

---

### Secondary Driver: Closing the Pressure Point on Inbound Integration

`02-current-state.md:84-90` named pressure point #7: *"no webhook ingestion or outbound integration pattern in the framework"*. Iteration 4 produces the first concrete realisation of both halves of that pattern. The plugin produced here is a candidate template for any future inbound integration, but only **as a side effect**; QAS-5 alone does not justify generalisation, so this iteration delivers a single fit-for-purpose plugin.

---

### Inherited from Iteration 3 Step 7

| Inherited input | Source |
| --- |---|
| Primary driver: QAS-5 | Iter 3 Step 7 — next-iteration inputs |
| Surrounding system: WireMock as carrier simulator | `01-scenario.md:26`; `03-bounded-contexts.md` Shipping Context |
| New mechanism needed: inbound webhook ingestion | `02-current-state.md:84-90` — pressure point #7 |
| Schema addition: `ExternalShipmentId` on `Shipment` | `04-qas.md:83` — explicitly named in QAS-5's design-decision-forced clause |
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
| Customer-visible response time bounded at 10 s end to end | QAS-5 response measure |
| Any inbound HTTP endpoint exposed to the public internet must authenticate | Operational baseline |

---

### Architectural Concerns

| Concern | Description |
| --- |---|
| CON-17 | Webhook arrival can race the dispatch event. The carrier may emit `shipment.status.updated` before nopCommerce has committed the local dispatch record, leaving the webhook with no correlation target |
| CON-18 | Carriers retry webhooks on 5xx and on timeout. The handler must be idempotent under repeated delivery of the same event |
| CON-19 | Out-of-order delivery is real: webhook A ("In Transit") may arrive after webhook B ("Delivered") because of carrier retry windows, NAT timeouts, or transient routing issues. The handler must reach the correct final state regardless |
| CON-20 | Webhook authentication must be simple enough to wire up against WireMock yet realistic. Bearer token suffices for the demo; HMAC payload signing is the production-grade alternative |
| CON-21 | `ShippingStatus` (the existing nopCommerce enum in `Nop.Core`) is coarse: `NotYetShipped, Shipped, Delivered, ShippingNotRequired`. Carrier vocabularies are richer (`PICKED_UP, IN_TRANSIT, OUT_FOR_DELIVERY, EXCEPTION, RETURNED`). Mapping many-to-few loses information; extending the enum violates the plugin boundary |
| CON-22 | The outbound booking call adds latency to the admin "create shipment" flow if invoked synchronously, and risks blocking the admin UI when WireMock is slow. The Outbox pattern from Iter 2 is the obvious reuse target |
| CON-23 | Audit and dispute resolution: webhook-driven state changes can be contested by the customer. An audit table that records every receipt — including malformed payloads and rejections — is operationally valuable |
| CON-24 | The 10-second end-to-end budget covers webhook receipt, processing, status persistence, and email enqueue. Any synchronous external call inside that path eats into the budget |

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
| RabbitMQ topology (`verdemart.orders` exchange) | Independent of this iteration's flows; this iteration adds its own exchange and queues for the inbound webhook path |
| Dedup table + DLQ pattern (Iteration 3 bridge) | Reused as the template for inbound webhook idempotency and poison handling |
| ADR-002 plugin boundary | The carrier-integration plugin is the canonical example of inbound + outbound symmetry under ADR-002 |

---

## What Step 1 Establishes

- The iteration has a unified theme — **carrier integration as a complete bidirectional channel** — even though the QAS measures only the inbound half.
- QAS-5 is the primary driver; pressure point #7 is the secondary motivator and produces a reusable pattern as a side effect.
- The candidate concept inherited from Iter 3 (Outbox reuse for the outbound path) is the obvious starting point but Step 3 evaluates it against alternatives.
- Three new structural questions surface: how the inbound webhook is authenticated, how the external carrier vocabulary is preserved without coarsening, and how out-of-order webhooks reach the correct final state.
- The DLQ + idempotency pattern from Iteration 3's OpenBoxes bridge is reused, not reinvented — same shape, different queue.
- The brief's "≥1 independently deployable subsystem" requirement was already met by ADR-007 in Iter 3; nothing in Iteration 4 disturbs that, and this iteration deliberately keeps all new code inside a nopCommerce plugin.

Step 2 selects the element to decompose.
