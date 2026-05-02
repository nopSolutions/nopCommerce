# ADD Iteration 4 — Step 5: Define Interfaces

## Key Interfaces

**`ICarrierWebhookAuditService`** — `RecordReceiptAsync(rawPayload, remoteIp, eventId, outcome, notes)` returns the inserted audit row `Id` so the controller can pass it to the consumer for outcome amendment; `AmendOutcomeAsync(auditId, outcome, notes)` is idempotent.

**`IProcessedCarrierEventRepository`** — `HasProcessedAsync(eventId)` and `RecordProcessedAsync(eventId, shipmentId)`; the insert enlists in the ambient transaction; the unique PK on `EventId` defends against concurrent races.

**`IExternalStatusMapper`** — `MapToInternal(externalStatus)` returns `ShippingStatus?`; returns `null` for values with no internal equivalent (`EXCEPTION`, `RETURNED`, `IN_TRANSIT`, `OUT_FOR_DELIVERY`); singleton and pure.

**`IWireMockClient`** — `BookShipmentAsync(request)` returns a `BookingResult` with status (`Booked / TransientFailure / PermanentFailure`) and `CarrierTrackingId`; owns bounded retry-with-backoff (3 attempts with jitter) for `TransientFailure`; broader retry is owned by message redelivery.

---

## HTTP Contract — `POST /api/carrier/webhook`

| | |
|---|---|
| Auth | Bearer token (`Authorization` header) |
| Request fields | `eventId`, `carrierTrackingId`, `carrierCode`, `status`, `statusDescription`, `occurredAtUtc`, `location?` |
| `200 OK` | Authenticated, persisted to audit log and queue; no body |
| `400` | Missing required fields or parse failure |
| `401` | Auth failure — still recorded in audit log |
| `500` | Audit insert or queue publish failed; carrier should retry |

The 200 is returned before processing completes — the carrier's delivery guarantee ends at "received and queued".

---

## Message Shapes (logical, not code)

**`CarrierStatusReceivedMessage`** (published to `verdemart.carrier.status`) — carries all inbound fields plus `ReceivedAtUtc`, `AuditRowId`, and `Version = 1`.

**`CarrierBookingRequestedMessage`** (written to existing Outbox, `EventType = carrier.booking.requested`) — carries `ShipmentId`, `OrderId`, shipping address, line items, and `Version = 1`.

Both follow the tolerant-reader versioning policy established in Iteration 3.

---

## Settings

**`CarrierWebhookSettings`** (`ISettings`) — `InboundBearerToken`, `WireMockBaseUrl`, `WireMockTimeoutMs` (3000), `MaxStatusRedeliveries` (5), `MaxBookingRedeliveries` (5), `CarrierCode` (`"WIREMOCK"`).

---

## Dependency Map

```
[ Outbound ]

ShipmentSentEventConsumer
    depends on → IRepository<OutboxMessage>  (Iter 2)

CarrierBookingConsumer
    depends on → IShipmentService
    depends on → IWireMockClient

[ Inbound ]

CarrierWebhookController
    depends on → ICarrierWebhookAuditService
    publishes  → verdemart.carrier.status

CarrierStatusConsumer
    depends on → IShipmentService
    depends on → IProcessedCarrierEventRepository
    depends on → IExternalStatusMapper
    depends on → ICarrierWebhookAuditService
    depends on → IWorkflowMessageService  (existing nopCommerce)
```

---

## What Step 6 Will Do

Step 6 produces the updated component view and sequence diagrams for the outbound booking and inbound status-update flows, and records the architectural decision this iteration produced.
