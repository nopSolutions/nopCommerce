# ADD Iteration 3 — Step 1: Review Inputs

## Iteration Goal

Establish OpenBoxes as the **operational authority for cross-channel stock allocation**, by doing two things at once:

1. Build the **OpenBoxes bridge consumer** that drains `verdemart.orders.openboxes` and creates fulfillment orders in OpenBoxes — closing the publish chain proven in Iterations 1 and 2.
2. Introduce an **allocation gate** that prevents two concurrent orders (web checkout and POS sale) from succeeding against the same last unit — satisfying QAS-2.

The two strands are inseparable. Without the bridge, OpenBoxes does not know about web orders, so it cannot be claimed authoritative for cross-channel stock. Without the allocation gate, the bridge alone processes fulfillment after the fact and oversell still occurs in the request window. Iteration 3 produces both as one structural change.

This iteration also closes ADR-003's idempotent-consumption mandate — a policy until now without an implementation — and resolves the cross-cutting message-versioning concern that Iteration 2 deferred.

---

## Inputs

### Primary Driver: QAS-2 (Consistency)

Inherited verbatim from the Step 7 verdict of Iteration 2: *"Iteration 3 begins with QAS-2 and the OpenBoxes bridge consumer."*

| Field | Value |
|---|---|
| Quality attribute | Consistency |
| Stimulus | Two orders for the last unit of a product are placed simultaneously — one from the web storefront, one from the POS |
| Source | nopCommerce web checkout and POS sale event |
| Environment | Normal operation; both systems active |
| Artifact | `StockQuantity` in nopCommerce and the fulfillment queue in OpenBoxes |
| Response | Exactly one order succeeds; the other receives a stock-unavailable response; inventory never goes below zero |
| Response measure | Zero confirmed oversell events under concurrent load; the losing order is rejected within the same request cycle |

---

### Secondary Driver: QAS-1 + QAS-4 — End-to-End Empirical Coverage

Iterations 1 and 2 satisfied QAS-1 on the publisher side under all three failure modes (consumer down, broker down, app crash). The 60-second recovery clause in QAS-1 and the self-heal clause in QAS-4 require a **real consumer** to be observable end to end. Until Iteration 3 lands, both are mechanism-only — claimed by design, never measured.

The bridge consumer built in this iteration produces the artifact those measurements need.

---

### Inherited from Iteration 2 Step 7

| Inherited input | Source |
|---|---|
| Primary driver: QAS-2 | Iter 2 Step 7 — next-iteration inputs |
| Build the OpenBoxes bridge consumer | Iter 2 Step 7 |
| Settle message schema versioning policy | Iter 2 Step 7 — cross-cutting concern blocking Iteration 3 |
| Implement consumer-side idempotency | ADR-003 — policy mandated, never realised |
| Element to refine: the order placement path (concurrency control on `ProductWarehouseInventory`) | Iter 2 Step 7 |
| Candidate concept to evaluate: pessimistic DB row lock with serializable transaction | Iter 2 Step 7 |
| Candidate alternative to compare: optimistic concurrency with row version + retry | Iter 2 Step 7 |

---

### Constraints

| Constraint | Source |
|---|---|
| nopCommerce remains the fixed commerce core | Carried from Iterations 1–2 |
| Publisher-side integration lives in plugins | ADR-002 |
| RabbitMQ topology is fixed (`verdemart.orders` exchange, `verdemart.orders.openboxes` queue) | ADR-001, ADR-003 |
| `OrderPlacedMessage` wire contract is fixed | Iter 1 — preserved through Iter 2 |
| At-least-once delivery from publisher | Consequence of ADR-003 + ADR-004 |
| `OrderGuid` is the idempotency key | ADR-003 |
| OpenBoxes is the authoritative source of physical stock | `03-bounded-contexts.md` — Key Ownership Tensions |
| The brief requires at least one independently deployable subsystem | Group Assignment 02 — Required Technical Constraints |
| Web checkout response time must remain bounded under contention | QAS-2 (same request cycle) and QAS-3 (under 3 s) |

---

### Architectural Concerns

| Concern | Description |
|---|---|
| CON-9 | Bridge hosting model is open: separate process vs nopCommerce plugin. The bridge talks to OpenBoxes — an external system — so coupling decisions matter |
| CON-10 | Where the idempotency check lives — bridge dedup table, OpenBoxes natural dedup on `OrderGuid`, or both. Each has different failure modes |
| CON-11 | OpenBoxes' API may be unavailable or slow at consume time. The bridge must hold messages without loss (manual ack from ADR-003) |
| CON-12 | Poison messages (malformed payload, persistent OpenBoxes rejection) must not block the queue indefinitely. A DLQ becomes meaningful now that there is a consumer to be poisoned |
| CON-13 | `OrderPlacedMessage` has no version field. Schema evolution would silently break the bridge. A versioning policy must land in this iteration |
| CON-14 | Allocation-gate latency — any synchronous call from web checkout into OpenBoxes adds to the checkout budget |
| CON-15 | Web and POS must reach the **same** allocation gate; otherwise different views of stock allow oversell |
| CON-16 | Allocation reservations must time out automatically — if a checkout is abandoned mid-flight, the held unit must release without operator action |

---

### Relevant Existing Structures

| Element | Role |
|---|---|
| `verdemart.orders` exchange + `verdemart.orders.openboxes` queue | Iter 1 — fixed entry point for the bridge |
| `OrderPlacedMessage` JSON wire contract | Iter 1 — fixed payload shape |
| Outbox pattern + dispatcher | Iter 2 — guarantees that "order committed" implies "message will be delivered"; the bridge can trust the upstream invariant |
| `ProductWarehouseInventory` (entity in `Nop.Core.Domain.Catalog`) | Today the local stock state, with `StockQuantity` and `ReservedQuantity`. Its role under the new allocation gate is a Step 3 decision |
| `AdjustInventoryAsync` / `BookReservedInventoryAsync` (in `ProductService`) | Existing mutation points for stock; named in `02-current-state.md` as the seam where QAS-2's gate must be added |
| `pos.sale.completed` (publishes from POS) | Declared in `03-bounded-contexts.md` — already part of the cross-channel contract |
| OpenBoxes (external system) | Authoritative for physical stock per `03-bounded-contexts.md`. Whether its API natively supports synchronous allocation reservation, or only post-hoc fulfillment-order creation, is open — Step 3 must treat both possibilities and a feasibility spike (topic 10) confirms which is real |
| ADR-002 (plugin boundary) | States *"all integration code lives inside nopCommerce plugins"*. Whether this also covers a downstream-of-broker bridge that talks to OpenBoxes is not settled by ADR-002 alone — it is a Step 3 question |

---

## What Step 1 Establishes

- The iteration has a unified theme — **OpenBoxes as the working cross-channel stock authority** — even though it produces two visible artifacts (bridge consumer + allocation gate)
- QAS-2 is the primary driver; QAS-1 and QAS-4 gain end-to-end empirical coverage as a secondary outcome of the bridge work
- The candidate concept inherited from Iter 2 (pessimistic DB row lock) is **one option among several** for the allocation gate — its viability depends on whether the lock lives in nopCommerce, in OpenBoxes, or in a shared layer (CON-15). Step 3 evaluates the alternatives
- Two structural questions become real this iteration: **where the bridge runs** (CON-9) and **where the allocation gate lives** (CON-14, CON-15)
- The deferred cross-cutting concerns from Iter 2 — message versioning (CON-13), DLQ (CON-12) — close in this iteration
- The brief's "≥1 independently deployable subsystem" requirement is a candidate motivator for the bridge hosting decision, not a forced answer

Step 2 selects the element to decompose.
