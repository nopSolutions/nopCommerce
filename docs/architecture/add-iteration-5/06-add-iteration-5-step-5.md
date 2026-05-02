# ADD Iteration 5 — Step 5: Define Interfaces

## `IOpenBoxesClient` — Extended

Gains one new method: `GetIssuedFulfillmentOrdersAsync()` — returns a list of `OpenBoxesFulfillmentOrder` records currently in `ISSUED` state. The existing `CreateFulfillmentAsync` method is unchanged.

**`OpenBoxesFulfillmentOrder`** — fields: `FulfillmentId` (OpenBoxes internal id), `OrderGuid` (correlation key written by the bridge at creation, read from the `referenceNumber` field in the OpenBoxes response), `Status`, `IssuedAtUtc`.

---

## OpenBoxes API Contract

| | |
|---|---|
| Endpoint | `GET /api/generic/shipment?status=ISSUED` |
| Auth | Basic auth (`OpenBoxesApiKey`) |
| Response | JSON array; each object contains `id`, `referenceNumber` (carries `OrderGuid`), `status`, `lastUpdated` |

The `referenceNumber` → `OrderGuid` mapping was confirmed by reviewing the OpenBoxes API against a local instance.

---

## Settings

**`AllocationSettings`** (four new fields) — `OpenBoxesBaseUrl`, `OpenBoxesApiKey`, `PollerIntervalSeconds` (30), `PollerBatchSize` (50). All configurable without redeployment.

---

## What Step 6 Will Do

Step 6 sketches the updated component view and records the polling design decision.
