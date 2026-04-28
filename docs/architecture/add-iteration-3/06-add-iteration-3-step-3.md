# ADD Iteration 3 — Step 3: Identify Design Concepts

## What This Step Does

Step 3 selects the architectural tactics and patterns that the cross-channel order intake path applies. Each concept is paired with the alternative considered and the reason for rejection — the traceability that justifies each decision in the ADRs that close the iteration.

Seven concepts are selected. The first three address the **synchronous allocation gate**; the last four address the **asynchronous bridge consumer** and its operational hygiene.

---

## Synchronous Phase — The Allocation Gate

### 1. Pessimistic Allocation Gate Inside nopCommerce, Reached by Both Web and POS

**Driver:** QAS-2 — exactly one of two simultaneous orders for the last unit must succeed; the loser must be rejected within the same request cycle.

**Concept:** A single allocation gate lives inside nopCommerce, operating on the existing `ProductWarehouseInventory` row. The gate uses a pessimistic database row-level lock (`SELECT ... FOR UPDATE`), evaluates the available quantity, decrements, and commits — or returns a stock-unavailable response synchronously, in the same request cycle.

This positions nopCommerce as the **operational allocation authority** across channels. OpenBoxes remains authoritative for **physical** stock truth, as `03-bounded-contexts.md` already states; the bounded-contexts model is preserved by reframing the two systems as authoritative for different things — operational arbitration in nopCommerce, physical truth in OpenBoxes — reconciled via the existing `inventory.adjusted` event flow.

**Rejected alternatives:**

- *Optimistic concurrency with row version + retry* — listed as a candidate in Iter 2 Step 7. Rejected because under contention on a popular last unit, multiple retries inside the same request consume the QAS-3 latency budget; pessimistic locking gives a directly bounded rejection path.

- *OpenBoxes as synchronous allocation authority called by both web and POS* — Rejected on three grounds: (a) the OpenBoxes API capability for synchronous allocation reservation is uncertain (Step 1 — confirmed only by spike), (b) every checkout becomes a network hop into an external system, hostile to QAS-3, (c) it places OpenBoxes uptime on the critical path of every sale, which is the coupling Iterations 1 and 2 worked to remove elsewhere.

- *Redis distributed lock service* — Rejected because it introduces new infrastructure for a property already achievable inside the nopCommerce database. The brief explicitly warns against "too many technologies with shallow purpose".

---

### 2. Reservation TTL via a Dedicated Reservation Record

**Driver:** CON-16 — abandoned checkouts and crashed POS sessions must not hold stock indefinitely.

**Concept:** A successful gate decision produces two committed effects: the stock decrement on `ProductWarehouseInventory`, and a reservation record in a new table (e.g. `ProductReservation`) carrying the `OrderGuid`/POS sale identifier, the quantity held, and a `ReservedUntilUtc` timestamp. The web flow's reservation lives only as long as its own request transaction — released by transaction lifecycle. The POS flow's reservation lives between `reserve` and `confirm` calls, capped by a configurable TTL (default 5 minutes). An `IScheduleTask` reverses unconfirmed reservations after expiry, restoring `StockQuantity`.

The existing `ReservedQuantity` field on `ProductWarehouseInventory` is left untouched; using a separate reservation record keeps the new lifecycle explicit and reversible without entangling legacy nopCommerce reservation semantics.

**Rejected alternative:**

- *Reuse `ReservedQuantity` directly as the new reservation primitive* — Rejected because the legacy field's lifecycle is tied to existing nopCommerce code paths in unfamiliar ways. Repurposing it risks subtle interactions with the existing services (e.g. `BookReservedInventoryAsync`) and is not reversible without auditing every caller.

---

### 3. POS Reaches the Gate via Synchronous HTTP; Existing `pos.sale.completed` Becomes Confirmation

**Driver:** CON-15 — web and POS must reach the **same** allocation gate; QAS-2 — synchronous rejection in the same request cycle.

**Concept:** POS calls a new HTTP endpoint exposed by nopCommerce before finalising any in-store sale. The endpoint runs the same gate as the web flow on the same `ProductWarehouseInventory` row. On 200, POS proceeds and commits the sale locally; on 409, the sale is refused at the till. After local commit, POS publishes `pos.sale.completed` over the existing RabbitMQ contract from `03-bounded-contexts.md`, which becomes an **audit confirmation** rather than the allocation step.

Allocation (synchronous) and fulfillment notification (asynchronous) are separated. The async event is no longer load-bearing for QAS-2.

**Rejected alternative:**

- *Use `pos.sale.completed` directly as the allocation step (no synchronous call)* — Rejected because the message takes finite time to reach nopCommerce; in the window between POS commit and message arrival, the web storefront still shows the unit as available and oversell occurs. QAS-2 explicitly forbids this.

---

## Asynchronous Phase — The Bridge Consumer

### 4. Bridge Hosted as a Separate Deployable Service

**Driver:** CON-9 — bridge hosting model open; Group Assignment 02 — at least one independently deployable subsystem required.

**Concept:** The OpenBoxes bridge is a small independent service (single Docker container) that connects to RabbitMQ, consumes `verdemart.orders.openboxes`, and calls the OpenBoxes API. It runs in its own process with its own lifecycle, separate from the nopCommerce web process. Configuration is environment-driven; the service has no compile-time dependency on nopCommerce assemblies.

This satisfies the brief's independently-deployable-subsystem requirement and keeps nopCommerce free of OpenBoxes-specific code. The bridge's implementation language can be chosen for fit (e.g. .NET worker, Python, Node) without disturbing nopCommerce.

**Rejected alternative:**

- *Bridge as a nopCommerce plugin consuming RabbitMQ in-process* — Rejected because (a) it makes nopCommerce knowledgeable about an external system's API, weakening the boundary the plugin layer was meant to protect; (b) the project as a whole would not satisfy the brief's "independently deployable subsystem" requirement; (c) ADR-002's spirit was decoupling the **commerce core** from downstream specifics — pulling OpenBoxes back into the same process re-couples them at the assembly level.

---

### 5. Consumer Idempotency via Bridge-Local Dedup on `OrderGuid`

**Driver:** CON-10 — at-least-once delivery from the publisher (ADR-003 + ADR-004) requires the consumer to be safe under redelivery; ADR-003 mandates `OrderGuid` as the idempotency key.

**Concept:** The bridge maintains a small local dedup table (`processed_orders` with `OrderGuid PK`, `ProcessedAtUtc`, `OpenBoxesFulfillmentId`). On each message: check the table; if `OrderGuid` exists, ack and skip; otherwise, call OpenBoxes' fulfillment API, on success insert the row, then ack. If OpenBoxes returns a duplicate-detection error, treat it as success and insert the row with whatever identifier OpenBoxes returns.

This is **defence in depth**: the bridge does not assume OpenBoxes deduplicates, but co-operates with native dedup if present.

**Rejected alternatives:**

- *Trust OpenBoxes' natural idempotency only* — Rejected because Step 1 flagged OpenBoxes' API behaviour as uncertain. Building correctness on an unverified property risks silent duplicates in the demo.

- *No dedup; rely on OpenBoxes erroring on duplicate creation, retry indefinitely* — Rejected because some duplicate-creation paths return ambiguous errors that the bridge cannot distinguish from genuine failures, leading to message loops.

---

### 6. Dead-Letter Routing for Poison Messages

**Driver:** CON-12 — poison messages (malformed payload, persistent OpenBoxes rejection) must not block the queue.

**Concept:** The `verdemart.orders.openboxes` queue is configured with `x-dead-letter-exchange: verdemart.orders.dlx` and bound to a `verdemart.orders.openboxes.dlq` queue on that exchange. The bridge tracks per-message redelivery; once a configurable retry limit is exceeded (default 5), the bridge NACKs without requeue and RabbitMQ routes the message to the DLQ. The DLQ is a regular durable queue that operators can inspect, replay, or discard manually.

**Rejected alternatives:**

- *Quarantine table inside the bridge instead of a DLQ* — Rejected because RabbitMQ's native dead-letter mechanism is tested, idiomatic, and visible in standard RabbitMQ tooling. A custom table reinvents the wheel and hides poison messages from operators.

- *Infinite retry without DLQ* — Rejected because a single poison message stalls the queue indefinitely, blocking every order behind it.

---

### 7. Versioned Wire Contract with Tolerant Readers

**Driver:** CON-13 — `OrderPlacedMessage` schema evolution must not silently break the bridge.

**Concept:** The wire contract gains an explicit `version` field (initially `1`). Consumers read messages tolerantly: unknown fields are ignored, missing optional fields fall back to defaults, additive changes do not bump the version. Breaking changes require a new routing key (e.g. `order.placed.v2`) and a parallel queue binding for the consumers that can handle it. The publisher in Iteration 1's plugin gains the version field at publish time; existing consumers simply ignore it.

This is a policy decision more than a code change; it must be stated now to prevent silent drift as more consumers (ERPNext, search, CRM) bind to the same exchange in later iterations.

**Rejected alternative:**

- *No version field; consumers verify exact shape and reject unknown fields* — Rejected because every additive change becomes a breaking change for at least one consumer, blocking schema evolution.

---

## Summary: Concepts → Drivers → Rejected Alternatives

| # | Concept | Driver(s) satisfied | Rejected alternative |
|---|---|---|---|
| 1 | Pessimistic allocation gate inside nopCommerce | QAS-2 | Optimistic concurrency; OpenBoxes synchronous; Redis distributed lock |
| 2 | Reservation TTL via dedicated reservation record | CON-16 | Reuse legacy `ReservedQuantity` |
| 3 | POS calls sync HTTP gate; `pos.sale.completed` as confirmation | CON-15 + QAS-2 | `pos.sale.completed` as the allocation step |
| 4 | Bridge as separate deployable service | CON-9 + brief | nopCommerce plugin |
| 5 | Bridge-local dedup table on `OrderGuid` | CON-10 + ADR-003 | OpenBoxes natural dedup only; no dedup |
| 6 | RabbitMQ dead-letter exchange for poison messages | CON-12 | Quarantine table; infinite retry |
| 7 | Versioned wire contract with tolerant readers | CON-13 | No version field |

CON-11 (OpenBoxes API unavailable or slow at consume time) is not addressed by a new concept — it is satisfied by inheritance: ADR-003's manual-acknowledgement policy means the bridge does not ack until OpenBoxes confirms creation. While OpenBoxes is unreachable, messages remain on the queue.

CON-14 (allocation-gate latency on the checkout budget) is satisfied by Concept 1's location decision: the gate runs against a local database row, not over a network hop, keeping the synchronous cost inside the existing checkout transaction.

---

## What Step 4 Will Do

Step 4 turns these concepts into named components: classes, files, schemas, and registration points. The shape of the new HTTP allocation endpoint, the `ProductReservation` table, the bridge service skeleton, the DLX topology, and the new wire-contract field are all defined there with enough precision for Step 5 to specify interfaces ready for implementation.
