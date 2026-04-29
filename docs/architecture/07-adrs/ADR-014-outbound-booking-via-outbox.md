# ADR-014 — Outbound Carrier Booking via Existing Outbox

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-29 |
| Produced by | ADD Iteration 4 — Step 6 (drivers: CON-22; ADR-004) |

## Context

Inbound carrier webhooks need a correlation key (`Shipment.ExternalShipmentId`, per ADR-012). That key is the carrier's tracking identifier, and it can only be obtained by calling the carrier — an outbound HTTP POST to WireMock when a shipment is created. Without this outbound call, the inbound webhook path established in ADR-011 has nothing to correlate against.

Two concerns shape the choice:
- CON-22: the admin UI must not block on WireMock latency. A synchronous booking call inside the admin "create shipment" flow puts a third-party HTTP call on the user-visible critical path.
- ADR-004 already established the **transactional Outbox** as the established pattern for guaranteed delivery of asynchronous integration events. Reuse is preferred to invention.

The outbound booking call writes back into nopCommerce's own database (`Shipment.ExternalShipmentId`, `ExternalCarrierCode`). This writeback is the deliberate counterpart to ADR-008's bridge: the OpenBoxes bridge has no writeback, so it lives outside; the carrier booking does have writeback, so it lives inside.

## Decision

**Trigger:** a new `IConsumer<ShipmentSentEvent>` inside the plugin (`ShipmentSentEventConsumer`) listens for the existing nopCommerce domain event. Its only job is to insert one row into the existing Iter 2 Outbox table with `EventType="carrier.booking.requested"` and a JSON payload containing `ShipmentId`, `OrderId`, the customer's shipping address, and the line items in carrier-readable form.

**Publish:** the existing `OutboxDispatcherTask` from Iter 2 (ADR-005) picks up the new event type on its next tick and publishes to a new RabbitMQ exchange `verdemart.carrier.booking` on routing key `carrier.booking.requested`. No new dispatcher, no new schedule task, no parallel outbox table — the existing Outbox is event-type-agnostic by design.

**Consume:** a new `CarrierBookingConsumer` (`IHostedService` in the same nopCommerce process) drains `verdemart.carrier.booking.requested`. For each message:
1. Look up the `Shipment` row. If `ExternalShipmentId` is already populated, ack — the booking ran on a previous delivery (idempotent under at-least-once redelivery).
2. Call `IWireMockClient.BookShipmentAsync(...)`. On `TransientFailure` (5xx, timeout), NACK with requeue. On `PermanentFailure` (4xx, malformed), NACK without requeue and the message routes to a DLQ symmetric to the inbound DLQ.
3. On success, in a DB transaction: update `Shipment.ExternalShipmentId` and `Shipment.ExternalCarrierCode`; commit; ack.

**Hosting:** in-process inside nopCommerce, *not* a separate deployable. The booking flow writes back into nopCommerce's own database, so cross-process coordination would require a second integration surface (HTTP API or queue) to do the writeback — net cost for no benefit.

## Rejected Alternatives

**Synchronous HTTP POST inside the admin "create shipment" flow.**
*Rejected:* (a) the admin UI now blocks on WireMock latency; (b) any WireMock outage propagates to a user-visible admin error and forces operators to retry manually; (c) the established pattern from ADR-004 already solves this; reinventing it is churn.

**New separate outbox table for carrier events.**
*Rejected:* the Iter 2 Outbox is event-type-agnostic by design — `EventType` is a column. Forking the table doubles the operational surface (two retention policies, two dispatchers, two failure modes to debug) for no architectural gain.

**Booking flow as a separate deployable service (mirroring ADR-008).**
*Rejected:* ADR-008's separation made sense for the OpenBoxes bridge because that bridge has no writeback into nopCommerce. The carrier booking flow does have writeback (`Shipment.ExternalShipmentId`); a separate deployable would need its own path back into nopCommerce — another HTTP API, another auth surface, another deploy artefact — for no benefit. The brief's "≥1 independently deployable subsystem" requirement is already satisfied by ADR-008.

**Direct RabbitMQ publish from the `IConsumer<ShipmentSentEvent>` (no Outbox).**
*Rejected:* the same reasoning that produced ADR-004 applies. If the broker is unreachable at the moment `ShipmentSentEvent` fires, the booking event is lost. The Outbox guarantees that "shipment dispatched" implies "booking will be requested," even across broker outages.

**Skip the booking call entirely; populate `ExternalShipmentId` by manual admin entry.**
*Rejected:* the integration would feel staged in the demo (operator types a value into a field) and provides no realistic carrier-API exercise. QAS-5 is testable end to end only when the booking flow actually establishes the correlation key.

## Consequences

- The admin UI returns immediately on `ShipmentSentEvent`. WireMock latency is invisible to the operator. WireMock outages mean the outbox row sits and the dispatcher retries on schedule — same guarantee as the OpenBoxes path.
- No new schedule task, no new dispatcher, no parallel outbox. The Iter 2 infrastructure is reused as designed.
- The booking consumer runs in the same process as nopCommerce, so the `Shipment` writeback is a local DB transaction — no cross-process coordination needed.
- The booking flow inherits ADR-004's at-least-once delivery semantics; the consumer's idempotency check (`if ExternalShipmentId is already set, ack`) makes redelivered messages safe.
- A new symmetric DLQ topology (`verdemart.carrier.booking.dlx` + `…dlq`) catches permanent booking failures (e.g. WireMock returning 4xx). Operators inspect, repair upstream, replay.
- The `IConsumer<ShipmentSentEvent>` registration uses the existing nopCommerce convention — discovered and invoked by the framework when the event fires, no special wiring required.
- WireMock contract details (request and response shape) are isolated behind `IWireMockClient`; substituting a real carrier later (DHL, UPS) changes only that client and the `IExternalStatusMapper` mapping.
- The booking call's bounded retry-with-backoff (3 in-call retries with jitter) absorbs short transient blips without triggering message redelivery; broader retries are owned by the broker. This separation mirrors ADR-009's defence-in-depth philosophy.
