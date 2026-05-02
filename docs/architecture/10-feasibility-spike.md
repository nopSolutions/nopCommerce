# Feasibility Spike — Transactional Outbox + RabbitMQ

**Spike location:** `spike/OutboxSpike/`  
**ADR validated:** [ADR-004 — Transactional Outbox](07-adrs/ADR-004-transactional-outbox.md)  
**QAS validated:** QAS-1 (Reliability), QAS-3 (Availability), QAS-4 (Recoverability)

---

## Why This Spike

The riskiest assumption in the architecture is the **dual-write problem** described in ADR-004.

Iteration 1 of ADD introduced `OrderPlacedConsumer`, which called `BasicPublish` inline on the checkout thread. That approach has a fatal gap: if RabbitMQ is unreachable at the exact moment `PlaceOrderAsync` fires, the order is committed to the database but no message is sent — a silent loss with no retry.

ADR-004 closes this by adopting the transactional outbox pattern: the message intent is written to a local `OutboxMessage` table inside the **same database transaction** that commits the order. A separate `OutboxDispatcherTask` polls the table and publishes to RabbitMQ independently. The broker is entirely off the checkout thread.

This spike proves that the mechanism works under the three failure modes that matter:

| Failure mode | QAS at risk | Spike scenario |
| --- | --- | --- |
| Normal operation | QAS-1 baseline | Scenario 1 — Happy Path |
| RabbitMQ unreachable at commit time | QAS-1 full coverage | Scenario 2 — Broker Down |
| Process crash between commit and dispatch | QAS-1 / QAS-4 | Scenario 3 — Process Crash |

---

## What Was Built

A standalone .NET 9 console application (`spike/OutboxSpike/`) with no dependency on the nopCommerce codebase.

| File | Role |
| --- | --- |
| `OutboxRepository.cs` | SQLite-backed outbox: `InsertAsync`, `GetPendingAsync`, `MarkSentAsync` |
| `RabbitPublisher.cs` | Connects to RabbitMQ, declares `verdemart.orders` exchange and `verdemart.orders.openboxes` durable queue (persistent delivery, DLQ args) |
| `OutboxDispatcher.cs` | 1-second poll loop — reads Pending rows, publishes, marks Sent; catches broker errors and retries on next tick |
| `Program.cs` | Orchestrates the three scenarios in sequence, prints timestamped evidence, exits non-zero on any failure |

Infrastructure used: RabbitMQ 3 (`spike-rabbitmq` Docker container, port 5672). SQLite in-memory (Scenarios 1–2) and file-based (Scenario 3).

---

## How to Run

Prerequisites: Docker, .NET 9 SDK.

```bash
# 1 — start RabbitMQ
cd spike
docker compose up -d

# 2 — run the spike
cd OutboxSpike
dotnet run
```



---

## Recorded Output

The following output was captured during the validation run on 2026-05-02.

```
=== SCENARIO 1: Happy Path ===
[T+0.06s] INSERT order-s1-ed4d137a (Status=Pending)
[T+0.14s] Published order-s1-ed4d137a → verdemart.orders | Status=Sent
Success: QAS-1 (happy path): row published and marked Sent within 2s

=== SCENARIO 2: Broker Down → Recovery ===
[T+0.00s] Stopping spike-rabbitmq...
[T+1.30s] spike-rabbitmq stopped
[T+1.30s] INSERT order-s2-01-019345 (Status=Pending) — broker is DOWN
[T+1.30s] INSERT order-s2-02-ddf032 (Status=Pending) — broker is DOWN
[T+1.30s] INSERT order-s2-03-b70c95 (Status=Pending) — broker is DOWN
[T+1.30s] Broker unreachable — 3 row(s) remain Pending
[T+2.31s] Broker unreachable — 3 row(s) remain Pending
[T+3.31s] Broker unreachable — 3 row(s) remain Pending
[T+4.31s] Broker unreachable — 3 row(s) remain Pending
[T+5.30s] After 4s with broker DOWN: 3 row(s) still Pending (expected 3)
[T+5.30s] Starting spike-rabbitmq...
[T+5.31s] Broker unreachable — 3 row(s) remain Pending
[T+5.44s] Waiting for broker to become healthy...
[T+6.31s] Broker unreachable — 3 row(s) remain Pending
[T+7.31s] Broker unreachable — 3 row(s) remain Pending
[T+8.32s] Broker unreachable — 3 row(s) remain Pending
[T+9.32s] Broker unreachable — 3 row(s) remain Pending
[T+10.33s] Published order-s2-01-019345 → verdemart.orders | Status=Sent
[T+10.33s] Published order-s2-02-ddf032 → verdemart.orders | Status=Sent
[T+10.33s] Published order-s2-03-b70c95 → verdemart.orders | Status=Sent
[T+10.47s] spike-rabbitmq healthy
Success: QAS-1 (broker-down): all 3 rows recovered and published after broker restart

=== SCENARIO 3: Process Crash Simulation ===
[T+0.01s] INSERT order-s3-01-5829bf (Status=Pending) — dispatcher NOT started (simulates crash)
[T+0.02s] INSERT order-s3-02-114af6 (Status=Pending) — dispatcher NOT started (simulates crash)
[T+0.02s] Fresh dispatcher started (simulates process restart)
[T+0.02s] Published order-s3-01-5829bf → verdemart.orders | Status=Sent
[T+0.03s] Published order-s3-02-114af6 → verdemart.orders | Status=Sent
Success: QAS-1 (crash recovery): both rows survived and were published after restart

All scenarios complete. spike-rabbitmq left running.

```

---

## Measurements

| Metric | Value | QAS response measure | Result |
| --- | --- | --- | --- |
| Scenario 1 — publish latency (happy path) | 90ms | ≤ 1s poll interval | ✅ Pass |
| Scenario 2 — rows lost during broker outage | 0 of 3 | Zero orders lost | ✅ Pass |
| Scenario 2 — time from broker restart to all rows published | ~5s | ≤ 60s after recovery | ✅ Pass |
| Scenario 3 — rows lost after simulated process crash | 0 of 2 | Zero orders lost | ✅ Pass |
| Scenario 3 — time from fresh dispatcher start to published | 10ms | ≤ 1s poll interval | ✅ Pass |

---

## What the Spike Confirms

**ADR-004 is structurally sound.**

1. **The dual-write hole is closed.** Rows inserted into the outbox table survive both a broker outage (Scenario 2) and a process crash (Scenario 3). No message is lost between the commit and the eventual publish.

2. **Checkout latency is bounded by the database only.** The dispatcher runs on a separate loop; `BasicPublish` never appears on the order placement thread.

3. **Recovery is automatic.** Neither Scenario 2 nor Scenario 3 required any operator action. The dispatcher found and published all Pending rows on its next tick after connectivity was restored.

4. **At-least-once delivery is the natural consequence.** The spike confirms that a row stays `Pending` until the broker acknowledges the publish. Consumers must therefore be idempotent — which ADR-003 already mandates, using `OrderGuid` as the idempotency key.

---

## What the Spike Does Not Confirm

| Open item | Where it goes |
| --- | --- |
| Exact 60s recovery clause under realistic order volume and network conditions | Full QAS-1 empirical spike (Part 2 evidence pack) |
| `FOR UPDATE SKIP LOCKED` behaviour under multi-node nopCommerce deployment | Accepted as a known gap at VerdeMart's current scale; documented in `08-risk-and-validation-plan.md` |
| OpenBoxes API contract (fulfillment order creation, `referenceNumber` field shape, duplicate detection response) | Separate OpenBoxes feasibility spike — highest priority before the final demo |

---

## Relationship to the Design

The spike directly corresponds to the mechanism described in [ADR-004](07-adrs/ADR-004-transactional-outbox.md) and implemented (in design) in [ADD Iteration 2](add-iteration-2/06-add-iteration-2-step-6.md). The `OutboxRepository`, `OutboxDispatcher`, and `RabbitPublisher` classes in the spike are the direct prototypes of:

- `OutboxMessage` table + migration (Iteration 2 migration)
- `OutboxDispatcherTask` (`IScheduleTask`, 1s poll) in `Nop.Plugin.Messaging.RabbitMq`
- `RabbitMqConnectionFactory` + exchange/queue topology in the same plugin
