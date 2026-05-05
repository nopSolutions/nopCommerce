# ADD Iteration 4 — Step 6: Sketch Views and Record Design Decisions

## What This Step Does

Step 6 produces the updated views (component + sequence) for the two new flows — the outbound booking via Outbox and the inbound status polling — and records the architectural decisions this iteration produced as ADRs in `07-adrs/`.

---

## Decisions Recorded

This iteration produced one architectural decision. Its full text lives in `07-adrs/`:

- [ADR-008 — Carrier Status via Scheduled Polling](../07-adrs/ADR-008-carrier-status-polling.md)

---

## What This Iteration Produced

| Artefact | Description |
| --- | --- |
| nopCommerce plugin | `Nop.Plugin.Shipping.CarrierWebhook` with 8 named components |
| `Shipment` columns | `ExternalShipmentId`, `ExternalCarrierCode`, `ExternalShippingStatus`, `LastStatusOccurredAtUtc` |
| Schedule task | `CarrierStatusPollerTask` — polls WireMock every 30 s per open shipment |
| Background service | `CarrierBookingConsumer` (`IHostedService`) |
| Domain-event consumer | `ShipmentSentEventConsumer` writes to existing Outbox |
| Typed HTTP client | `IWireMockClient` — `BookShipmentAsync` + `GetShipmentStatusAsync` |
| RabbitMQ topology | exchange `verdemart.carrier.booking` + queue `…requested` + DLQ |
| Wire contracts | `CarrierBookingRequestedMessage`, `BookingRequest`/`BookingResult`, `ShipmentStatusResult` (all carry `Version=1` following the tolerant-reader versioning policy) |
| Email template | `ShipmentStatusUpdated.CustomerNotification` plus two new tokens |
| ADR-008 | Carrier status via scheduled polling |

---

## What Step 7 Will Do

Step 7 verifies the design against QAS-5's carrier-half response measure (poll tick → customer-visible state + queued email within 30 s), names the warehouse visibility half as carried to Iteration 5, and closes the iteration.
