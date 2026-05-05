# ADD Iteration 5 — Step 1: Review Inputs

## Iteration Goal

Close the warehouse visibility half of QAS-5 by making OpenBoxes fulfillment state observable in nopCommerce without operator intervention.

Iteration 4 satisfied the carrier tracking clause of QAS-5 (inbound webhook → customer-visible status in ≤30 s). The second clause — warehouse fulfillment state (OpenBoxes `ISSUED`) becoming visible in nopCommerce — was deferred. A polling `IScheduleTask` is the selected automation mechanism — a deliberate choice over the webhook capability that OpenBoxes does provide.

This iteration is deliberately narrow: one new component, one new ADR, one constrained design space.

---

## Inputs

### Primary Driver: QAS-5 Warehouse Half (Visibility)

Inherited from Iteration 4 Step 7.

| Field | Value |
| --- | --- |
| Quality attribute | Visibility (cross-channel state propagation) |
| Stimulus | OpenBoxes marks a fulfillment order as `ISSUED` |
| Source | OpenBoxes warehouse system |
| Environment | Normal operation; order has been placed and bridge consumer has created the fulfillment order in OpenBoxes |
| Artifact | The order status in nopCommerce and the customer-facing order detail page |
| Response | nopCommerce detects the state change and updates the internal order status accordingly |
| Response measure | OpenBoxes fulfillment state visible in nopCommerce within the polling interval (default 30 seconds); no operator action required |

---

### Inherited from Iteration 4 Step 7

| Inherited input | Source |
| --- | --- |
| Primary driver: QAS-5 warehouse visibility half | Iter 4 Step 7 — inputs carried forward |
| Design choice: polling over OpenBoxes webhooks | Step 3 rationale — reliability and unidirectional dependency |
| Existing pattern: `IScheduleTask` for periodic background work | `OutboxDispatcherTask` (Iter 2), `ReleaseExpiredReservationsTask` (Iter 3) |
| Existing pattern: `IOpenBoxesClient` interface in the bridge | ADR-007 — the bridge already calls OpenBoxes; the polling task reuses the same API |

---

### Constraints

| Constraint | Source |
| --- | --- |
| nopCommerce remains the fixed commerce core | Carried from Iterations 1–4 |
| Integration code lives inside plugins | ADR-002 |
| Polling chosen over OpenBoxes webhooks | Deliberate — reliability and unidirectional dependency direction |
| No new independently deployable subsystem — the brief's requirement is already met by ADR-007 | Assignment brief |
| Polling interval must be configurable without redeployment | `ISettings` convention |
| Redis is available as the nopCommerce distributed cache provider | Technology stack — optional Redis support is built into nopCommerce core |

---

### Architectural Concerns

| Concern | Description |
| --- | --- |
| CON-25 | Correlation: the polling task must match an OpenBoxes fulfillment order back to a nopCommerce order. The `OrderGuid` was sent to OpenBoxes by the bridge (ADR-007); the polling task needs to read it back from OpenBoxes' response |
| CON-26 | Idempotency: the task may run multiple times while a fulfillment order is in `ISSUED` state. Status updates must be idempotent — applying the same transition twice must not corrupt state |
| CON-27 | Polling frequency vs load: polling too frequently adds unnecessary HTTP calls to OpenBoxes; too infrequently increases the visibility lag beyond the QAS-5 response measure |
| CON-28 | What to do with OpenBoxes states other than `ISSUED` — `PICKED`, `CANCELED`, etc. The task must define a clear mapping policy |
| CON-29 | DB read load scales with open orders: on every tick both poller tasks must know the last-known status of each open order to detect changes. Reading this from the DB on every tick across all nodes grows linearly with the number of open orders. Redis is introduced as a read-through cache to absorb these reads — the DB is consulted only when a status change is detected |

---

## What Step 1 Establishes

- The iteration has a single focus: close the warehouse visibility gap with a polling task
- Step 2 selects the element to decompose (the gap between OpenBoxes state and nopCommerce order status)
- Step 3 evaluates the polling task design against other alternatives, specially webhooks
