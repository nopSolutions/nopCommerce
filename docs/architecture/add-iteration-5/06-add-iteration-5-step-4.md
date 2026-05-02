# ADD Iteration 5 — Step 4: Instantiate Elements and Allocate Responsibilities

The polling task is added to the existing `Nop.Plugin.Inventory.AllocationGate` plugin. No new plugin is introduced — the task is a natural extension of the allocation gate's role as the warehouse-visibility boundary inside nopCommerce.

---

## New Components

| Component | Type | Responsibility |
|---|---|---|
| `OpenBoxesStatusPollerTask` | `IScheduleTask` | Polls OpenBoxes every 30 s for fulfillment orders in `ISSUED` state; for each match, creates a `Shipment` record, transitions the order to `Complete`, and writes a `carrier.booking.requested` outbox row — all in one DB transaction; idempotent; never throws |
| `IOpenBoxesClient` (extended) | Typed HTTP client | Gains `GetIssuedFulfillmentOrdersAsync()` — calls `GET /api/generic/shipment?status=ISSUED`; returns correlation records keyed on `OrderGuid` |
| `OpenBoxesFulfillmentOrder` | DTO | Correlation record from OpenBoxes: `FulfillmentId`, `OrderGuid`, `Status`, `IssuedAtUtc` |

**Existing dependencies injected into the poller:** `IOrderService`, `IOrderProcessingService`, `IShipmentService` (to create the `Shipment` row before the carrier booking consumer needs it), `IOutboxRepository`, `AllocationSettings`.

**Settings extension (`AllocationSettings`):** four new fields — `OpenBoxesBaseUrl`, `OpenBoxesApiKey`, `PollerIntervalSeconds` (default 30), `PollerBatchSize` (default 50).

---

## What Step 5 Will Do

Step 5 defines the `IOpenBoxesClient` extension, the DTO shape, and the OpenBoxes API contract.
