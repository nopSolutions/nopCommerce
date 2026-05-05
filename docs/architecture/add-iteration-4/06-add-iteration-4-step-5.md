# ADD Iteration 4 — Step 5: Define Interfaces

## Key Interfaces

**`IExternalStatusMapper`** — `MapToInternal(externalStatus)` returns `ShippingStatus?`; returns `null` for values with no internal equivalent (`EXCEPTION`, `RETURNED`, `IN_TRANSIT`, `OUT_FOR_DELIVERY`); singleton and pure.

**`IWireMockClient`** — two methods:

- `BookShipmentAsync(request)` returns a `BookingResult` with status (`Booked / TransientFailure / PermanentFailure`) and `CarrierTrackingId`; owns bounded retry-with-backoff (3 attempts with jitter) for `TransientFailure`; broader retry is owned by message redelivery.
- `GetShipmentStatusAsync(externalShipmentId)` returns a `ShipmentStatusResult` with the current `Status` string and `OccurredAtUtc`; returns a typed `Unreachable` result on timeout or connection failure so the poller can log and skip without throwing.

---

## Polling Response Shape

**`ShipmentStatusResult`** (returned by `IWireMockClient.GetShipmentStatusAsync`) — `ExternalShipmentId`, `Status` (string, carrier vocabulary), `OccurredAtUtc`, `IsUnreachable` (bool).

WireMock exposes `GET /api/shipments/{id}/status` returning:

```json
{
  "externalShipmentId": "WIRE-12345",
  "status": "IN_TRANSIT",
  "occurredAtUtc": "2026-05-04T10:00:00Z"
}
```

---

## Outbound Message Shape

**`CarrierBookingRequestedMessage`** (written to existing Outbox, `EventType = carrier.booking.requested`) — carries `ShipmentId`, `OrderId`, shipping address, line items, and `Version = 1`. Follows the tolerant-reader versioning policy established in Iteration 3.

---

## Settings

**`CarrierWebhookSettings`** (`ISettings`) — `WireMockBaseUrl`, `WireMockTimeoutMs` (3000), `MaxBookingRedeliveries` (5), `CarrierCode` (`"WIREMOCK"`).

---

## Dependency Map

```text
[ Outbound ]

ShipmentSentEventConsumer
    depends on → IRepository<OutboxMessage>  (Iter 2)

CarrierBookingConsumer
    depends on → IShipmentService
    depends on → IWireMockClient

[ Inbound ]

CarrierStatusPollerTask
    depends on → IShipmentService
    depends on → IWireMockClient
    depends on → IExternalStatusMapper
    depends on → IWorkflowMessageService  (existing nopCommerce)
```

---

## What Step 6 Will Do

Step 6 produces the updated component view and sequence diagrams for the outbound booking and inbound polling flows, and records the architectural decision this iteration produced.
