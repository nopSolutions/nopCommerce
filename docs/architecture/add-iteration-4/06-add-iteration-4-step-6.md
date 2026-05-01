# ADD Iteration 4 — Step 6: Sketch Views and Record Design Decisions

## What This Step Does

Step 6 produces the updated views (component + sequence) for the two new flows — the outbound booking via Outbox and the inbound status update via webhook — and records the architectural decisions this iteration produced as ADRs in `07-adrs/`.

---

## Decisions Recorded

This iteration produced four architectural decisions. Their full text lives in `07-adrs/`:

- [ADR-011 — Webhook Ingestion via Plugin with Async Internal Queue Handoff](../07-adrs/ADR-011-webhook-ingestion-async-handoff.md)
- [ADR-012 — External Shipment Correlation via `ExternalShipmentId` on `Shipment`](../07-adrs/ADR-012-external-shipment-correlation.md)
- [ADR-013 — External Status Preserved as String; Internal Enum Untouched](../07-adrs/ADR-013-external-status-preserved-as-string.md)
- [ADR-014 — Outbound Carrier Booking via Existing Outbox](../07-adrs/ADR-014-outbound-booking-via-outbox.md)

---

## What This Iteration Produced

| Artefact | Description |
| --- |---|
| nopCommerce plugin | `Nop.Plugin.Shipping.CarrierWebhook` with 13 named components |
| `Shipment` columns | `ExternalShipmentId`, `ExternalCarrierCode`, `ExternalShippingStatus`, `LastStatusOccurredAtUtc` |
| New tables | `CarrierWebhookEvent` (audit), `ProcessedCarrierEvent` (dedup) |
| HTTP endpoint | `POST /api/carrier/webhook` (bearer-token auth) |
| Background services | `CarrierStatusConsumer`, `CarrierBookingConsumer` (both `IHostedService`) |
| Domain-event consumer | `ShipmentSentEventConsumer` writes to existing Outbox |
| Typed HTTP client | `IWireMockClient` for outbound booking |
| RabbitMQ topology — outbound | exchange `verdemart.carrier.booking` + queue `…requested` |
| RabbitMQ topology — inbound | exchange `verdemart.carrier.status` + queue `…received` + DLX/DLQ |
| Wire contracts | `CarrierStatusPayload`, `CarrierStatusReceivedMessage`, `CarrierBookingRequestedMessage`, `BookingRequest`/`BookingResult` (all carry `Version=1` per ADR-010) |
| Email template | `ShipmentStatusUpdated.CustomerNotification` plus two new tokens |
| ADR-011 | Webhook ingestion via plugin + async queue handoff |
| ADR-012 | External shipment correlation via `ExternalShipmentId` |
| ADR-013 | External status preserved as string; internal enum untouched |
| ADR-014 | Outbound carrier booking via existing Outbox |

---

## What Step 7 Will Do

Step 7 verifies the design against QAS-5's carrier-half response measure (webhook receipt → customer-visible state + queued email within 10 s), names the warehouse visibility half as carried to Iteration 5, and closes the iteration.
