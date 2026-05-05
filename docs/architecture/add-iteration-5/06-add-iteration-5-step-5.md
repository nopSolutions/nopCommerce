# ADD Iteration 5 — Step 5: Define Interfaces

## `IOpenBoxesClient` — Extended

Gains one new method: `GetIssuedFulfillmentOrdersAsync()` — returns a list of `OpenBoxesFulfillmentOrder` records currently in `ISSUED` state. The existing `CreateFulfillmentAsync` method is unchanged.

**`OpenBoxesFulfillmentOrder`** — fields: `FulfillmentId` (OpenBoxes internal id), `OrderGuid` (correlation key written by the bridge at creation, read from the `referenceNumber` field in the OpenBoxes response), `Status`, `IssuedAtUtc`.

---

## OpenBoxes API Contract

| | |
| --- | --- |
| Endpoint | `GET /api/generic/shipment?status=ISSUED` |
| Auth | Basic auth (`OpenBoxesApiKey`) |
| Response | JSON array; each object contains `id`, `referenceNumber` (carries `OrderGuid`), `status`, `lastUpdated` |

The `referenceNumber` → `OrderGuid` mapping was confirmed by reviewing the OpenBoxes API against a local instance.

---

## Redis Cache Contract

The poller reads and writes last-known fulfillment status through nopCommerce's `IStaticCacheManager` abstraction, which routes to Redis when configured.

**Cache key pattern:** `verdemart:openboxes:status:{OrderGuid}` — one key per open order, value is the last-known OpenBoxes status string (e.g. `"PICKING"`, `"ISSUED"`).

**TTL:** 24 hours. An order not seen in OpenBoxes for 24 hours is assumed terminal; the key expires and the next tick falls back to the DB for that order.

**Cold start / cache miss:** the task queries `IOrderService` to retrieve current order status from the DB, populates the cache, then proceeds with the comparison. This ensures correctness after a Redis restart or first deployment with no warm cache.

**Write policy:** the cache is updated after the DB transaction commits successfully. If the transaction rolls back, Redis is not updated, so the next tick will re-detect the change and retry.

---

## Redis Distributed Lock

**Lock key:** `verdemart:lock:openboxes-poller` (one key shared across all nodes).

**TTL:** 60 seconds — long enough to cover the full tick duration including the OpenBoxes API call and DB transaction; auto-expires if the node crashes mid-tick so the next node can acquire it on the following interval.

**Behaviour:** at the start of each tick the task issues `SET verdemart:lock:openboxes-poller 1 NX EX 60`. If the result is `nil`, another node holds the lock — the task returns immediately with no further Redis, DB, or API access. If the result is `OK`, the node proceeds with the full tick and deletes the lock key on completion.

**Purpose:** prevents N nodes from calling OpenBoxes N times per tick. Correctness does not depend on the lock — DB and Redis writes are idempotent — but external API call multiplication is avoided.

---

## Settings

**`AllocationSettings`** (four new fields) — `OpenBoxesBaseUrl`, `OpenBoxesApiKey`, `PollerIntervalSeconds` (30), `PollerBatchSize` (50). All configurable without redeployment.

---

## What Step 6 Will Do

Step 6 sketches the updated component view and records the polling design decision.
