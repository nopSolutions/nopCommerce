# ADD Iteration 5 — Step 3: Choose One or More Design Concepts

## Candidate Concepts Evaluated

### Concept 1 — Polling `IScheduleTask` (selected)

A new `IScheduleTask` registered inside `Nop.Plugin.Inventory.AllocationGate` (or a new lightweight plugin) polls `GET /api/generic/shipment` on OpenBoxes on a configurable interval. For each fulfillment order that has reached `ISSUED`, it correlates back to the nopCommerce order via `OrderGuid` and updates the order status.

**Why selected:**
- OpenBoxes has no outbound webhook capability — polling is the only automated path (confirmed by feasibility spike)
- `IScheduleTask` is the established nopCommerce pattern for periodic background work; already used by `OutboxDispatcherTask` and `ReleaseExpiredReservationsTask`
- Stateless by design — each tick is independent; no inter-tick coordination needed
- Configurable interval via `ISettings` without redeployment
- Idempotent: detecting `ISSUED` twice for the same order is a no-op after the first status update

---

### Concept 2 — Manual admin trigger (rejected)

An admin observes OpenBoxes and manually marks the order as fulfilled in nopCommerce.

*Rejected:* this is not an automated architecture — it is a human process masquerading as one. It violates QAS-5's "no operator action required" clause and introduces an unbounded latency gap dependent on staff availability. The polling task costs one HTTP call per tick; the manual step costs operator attention on every order.

---

### Concept 3 — Continuous HTTP long-polling or SSE (rejected)

nopCommerce maintains a persistent connection to OpenBoxes and waits for state changes.

*Rejected:* OpenBoxes does not support SSE or long-polling. Even if it did, a persistent connection from nopCommerce to an external system would require connection lifecycle management that is out of scope for a plugin-based `IScheduleTask`. The polling cadence of 30 seconds is sufficient for the QAS-5 response measure.

---

## State Mapping Policy (addresses CON-28)

Only `ISSUED` is mapped in this iteration. Other OpenBoxes states are observed but not acted upon.

| OpenBoxes state | nopCommerce action |
| --- | --- |
| `CREATED`, `EDITING`, `VERIFYING`, `PICKING` | No action — fulfillment in progress |
| `PICKED` | No action in this iteration — recorded as a future signal for proactive customer notification |
| `ISSUED` | Update nopCommerce order status; trigger carrier booking via outbox |
| `CANCELED` | Log warning — requires operator review; out of automated scope |

The mapping is implemented as an `IOpenBoxesStatusMapper` interface so future iterations can extend it without modifying the task.

---

## Selected Concept Summary

One `IScheduleTask` polling OpenBoxes on a 30-second default interval, correlating by `OrderGuid`, updating order status on `ISSUED`, and triggering the carrier booking chain. Hosted inside the existing `Nop.Plugin.Inventory.AllocationGate` plugin to avoid introducing a new plugin for a single task.

Step 4 names the components and allocates responsibilities.
