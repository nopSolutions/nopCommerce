# ADD Iteration 5 — Step 3: Choose One or More Design Concepts

## Candidate Concepts Evaluated

### Concept 1 — Polling `IScheduleTask` (selected)

A new `IScheduleTask` registered inside `Nop.Plugin.Inventory.AllocationGate` (or a new lightweight plugin) polls `GET /api/generic/shipment` on OpenBoxes on a configurable interval. For each fulfillment order that has reached `ISSUED`, it correlates back to the nopCommerce order via `OrderGuid` and updates the order status.

**Why selected:**
- **Reliability over latency.** Polling is self-healing: nopCommerce reads current state on every tick regardless of what happened between ticks. A webhook missed because nopCommerce was temporarily unavailable is a silent data loss unless OpenBoxes retries indefinitely — which it does not guarantee. Polling has no equivalent failure mode; the next tick always recovers.
- **Unidirectional dependency preserved.** OpenBoxes supports outbound webhooks (confirmed by `openboxes.com/features`), but using them would require configuring OpenBoxes with nopCommerce's address and credentials, coupling the warehouse system to the commerce core in the reverse direction. Polling keeps the boundary clean: nopCommerce reaches out to OpenBoxes; OpenBoxes remains unaware of nopCommerce.
- `IScheduleTask` is the established nopCommerce pattern for periodic background work; already used by `OutboxDispatcherTask` and `ReleaseExpiredReservationsTask`
- Stateless by design — each tick is independent; no inter-tick coordination needed
- Configurable interval via `ISettings` without redeployment
- Idempotent: detecting `ISSUED` twice for the same order is a no-op after the first status update

---

### Concept 2 — Outbound webhook from OpenBoxes (rejected)

OpenBoxes supports configurable outbound webhooks per event type, including shipment events. nopCommerce would expose a new inbound webhook endpoint (following the pattern established in Iteration 4's `CarrierWebhook` plugin), and OpenBoxes would be configured to call it when a fulfillment order reaches `ISSUED`.

*Rejected* for two reasons. First, reliability: webhooks are fire-and-forget from OpenBoxes' side. If nopCommerce is temporarily unavailable and OpenBoxes exhausts its retry window, the `ISSUED` event is lost — nopCommerce never learns the order shipped without manual reconciliation. Polling has no equivalent failure mode. Second, dependency direction: this would require configuring OpenBoxes with nopCommerce's address and credentials, making the warehouse system aware of the commerce core. The architecture has consistently kept that boundary unidirectional throughout all five iterations.

---

### Concept 3 — Manual admin trigger (rejected)

An admin observes OpenBoxes and manually marks the order as fulfilled in nopCommerce.

*Rejected:* this is not an automated architecture — it is a human process masquerading as one. It violates QAS-5's "no operator action required" clause and introduces an unbounded latency gap dependent on staff availability. The polling task costs one HTTP call per tick; the manual step costs operator attention on every order.

---

### Concept 4 — Continuous HTTP long-polling or SSE (rejected)

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
