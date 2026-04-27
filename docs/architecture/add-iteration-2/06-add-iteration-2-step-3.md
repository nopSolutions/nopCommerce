# ADD Iteration 2 — Step 3: Identify Design Concepts

## What This Step Does

Step 3 selects the architectural tactics and patterns the new publish path applies. Each concept is paired with the alternative considered and the reason for rejection — that traceability is the heart of the design defence.

---

## Design Concepts Selected

### 1. Transactional Outbox

**Driver:** QAS-1 — order commit and publish intent must be atomic so no order is committed without a guaranteed publish.

**Concept:** Inside the same database transaction that commits the `Order` and decrements `StockQuantity`, insert a row into an `Outbox` table. The row carries the serialised message and a `Pending` status. Either both writes commit or neither does.

**Rejected alternative:** *Publish-with-retry off-thread.* Move the inline `BasicPublish` to a background queue (`Channel<T>` or `Task.Run`) and retry on failure.
**Why rejected:** Off-thread retry is not atomic with the order commit. If the application process dies between commit and successful publish, the in-memory queue dies with it and the message is lost. The whole point of this iteration is to remove that window.

---

### 2. Polling the Outbox Table

**Driver:** The dispatcher needs to discover new outbox rows quickly enough to satisfy the 60-second QAS-1 recovery clause.

**Concept:** The dispatcher runs a poll loop. Every poll interval (1–2 seconds), it selects a batch of `Pending` rows ordered by `CreatedAtUtc`, publishes each, and updates the row status.

**Rejected alternative:** *Database push via `LISTEN/NOTIFY`.*
**Why rejected:** MySQL has no native `LISTEN/NOTIFY` equivalent. PostgreSQL or a pub/sub layer would be required, and that pulls in infrastructure changes for a benefit (sub-second push) the QAS does not require. A 1–2 second poll interval gives plenty of headroom against the 60 s target.

---

### 3. Dispatcher Hosted Inside `IScheduleTask`

**Driver:** Constraints: no operator action; minimise the number of independent processes to deploy and observe.

**Concept:** The dispatcher is registered as an `IScheduleTask`. nopCommerce's existing scheduler runs it on the configured interval, restarts it after process restart, and surfaces failures in the admin log.


---

### 4. Single-Instance Dispatcher

**Driver:** Simplicity; correctness without external coordination.

**Concept:** Only one dispatcher instance runs at a time. nopCommerce's scheduler already enforces single-instance execution per task within one process; for multi-node deployments, a database-level row-level lock (`SELECT … FOR UPDATE SKIP LOCKED`) on the polling query keeps two nodes from publishing the same row twice.

---

### 5. At-Least-Once Delivery Semantics

**Driver:** QAS-4 (recoverability); the universal property of distributed publish under failure.

**Concept:** The dispatcher publishes a message before marking the row as `Sent`. If it crashes between the publish and the status update, the row remains `Pending` and is republished on the next loop. Consumers must therefore be idempotent, which is already mandated by ADR-003, with `OrderGuid` as the idempotency key.

---

---

## Summary: Concepts → Drivers

| Concept | Primary driver satisfied | Rejected alternative |
|---|---|---|
| Transactional Outbox | QAS-1 (atomicity) | Publish-with-retry; CDC |
| Polling | QAS-1 (recovery time) | LISTEN/NOTIFY |
| Dispatcher in `IScheduleTask` | Constraint (no new process) | Separate worker |
| Single-instance + row lock | Simplicity | Multi-instance with leader election |
| At-least-once + idempotent consumers | QAS-4 + reality | Exactly-once / 2PC |

---

## What Step 4 Will Do

Step 4 turns these concepts into named components, file paths, schema definitions, and DI registrations.
