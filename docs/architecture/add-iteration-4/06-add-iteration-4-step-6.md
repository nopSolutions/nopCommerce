# ADD Iteration 4 — Step 6: Sketch Views and Record Design Decisions

## What This Step Does

Step 6 produces the updated views (component + sequence) for the two new flows — the outbound booking via Outbox and the inbound status update via webhook — and records the architectural decisions this iteration produced as ADRs in `07-adrs/`.

---

## Component View (Updated)

```
┌──────────────────────────────────────────────────────────────────────────────┐
│  nopCommerce process                                                          │
│                                                                               │
│  ┌────────────────────────────────────────────────────────────────────────┐  │
│  │  Nop.Plugin.Shipping.CarrierWebhook  (NEW)                             │  │
│  │                                                                        │  │
│  │  [ Outbound path ]                                                     │  │
│  │  ShipmentSentEventConsumer                                             │  │
│  │       │ on ShipmentSentEvent: INSERT Outbox (carrier.booking.requested)│  │
│  │       ▼                                                                │  │
│  │  OutboxDispatcherTask (Iter 2 — unchanged)                             │  │
│  │       │ publishes → verdemart.carrier.booking                          │  │
│  │       ▼                                                                │  │
│  │  CarrierBookingConsumer  (IHostedService)                              │  │
│  │       │ BookShipmentAsync → WireMock                                   │  │
│  │       │ on success: SET Shipment.ExternalShipmentId                    │  │
│  │                                                                        │  │
│  │  [ Inbound path ]                                                      │  │
│  │  POST /api/carrier/webhook ──▶ CarrierWebhookController                │  │
│  │       │ bearer auth                                                    │  │
│  │       │ ICarrierWebhookAuditService → INSERT CarrierWebhookEvent       │  │
│  │       │ publish → verdemart.carrier.status                             │  │
│  │       └── return 200 immediately                                       │  │
│  │       ▼                                                                │  │
│  │  CarrierStatusConsumer  (IHostedService)                               │  │
│  │       │ IProcessedCarrierEventRepository (dedup by EventId)            │  │
│  │       │ IExternalStatusMapper (carrier vocab → ShippingStatus)         │  │
│  │       │ out-of-order guard (OccurredAtUtc)                             │  │
│  │       │ DB TX: UPDATE Shipment + IWorkflowMessageService (email)       │  │
│  │       └── Ack / Nack → DLQ after threshold                            │  │
│  └────────────────────────────────────────────────────────────────────────┘  │
│                                                                               │
│  ┌────────────────────────────────────────────────────────────────────────┐  │
│  │  Existing — Iter 1 + 2 + 3  (UNCHANGED)                                │  │
│  │  Outbox, OutboxDispatcherTask, AllocationGate, RabbitMqConnectionFactory│  │
│  └────────────────────────────────────────────────────────────────────────┘  │
└──────────────────────┬──────────────────────────────────────┬─────────────────┘
                       │                                      │
          ┌────────────▼─────────────────────┐     ┌─────────▼──────┐
          │  RabbitMQ                         │     │  WireMock      │
          │                                   │     │  (carrier)     │
          │  verdemart.carrier.booking        │     │                │
          │    queue: …requested (durable)    │     │  POST /booking │
          │    dlq:   …booking.dlq            │     └────────────────┘
          │                                   │
          │  verdemart.carrier.status         │
          │    queue: …received (durable)     │
          │    dlq:   …status.dlq             │
          └───────────────────────────────────┘
```

---

## Decisions Recorded

This iteration produced one architectural decision. Its full text lives in `07-adrs/`:

- [ADR-008 — Webhook Ingestion via Plugin with Async Internal Queue Handoff](../07-adrs/ADR-008-webhook-ingestion-async-handoff.md)

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
| Wire contracts | `CarrierStatusPayload`, `CarrierStatusReceivedMessage`, `CarrierBookingRequestedMessage`, `BookingRequest`/`BookingResult` (all carry `Version=1` following the tolerant-reader versioning policy) |
| Email template | `ShipmentStatusUpdated.CustomerNotification` plus two new tokens |
| ADR-008 | Webhook ingestion via plugin + async queue handoff |

---

## What Step 7 Will Do

Step 7 verifies the design against QAS-5's carrier-half response measure (webhook receipt → customer-visible state + queued email within 10 s), names the warehouse visibility half as carried to Iteration 5, and closes the iteration.
