# ADR-010 — Redis as Status Cache and Distributed Lock for Polling Tasks

| | |
|---|---|
| Status | Superseded — not implemented |
| Date | 2026-05-05 |
| Superseded by | Implementation — pollers adopted different idempotency mechanisms; Redis design assumptions did not hold |
| Produced by | ADD Iteration 5 — Step 6 (drivers: CON-29; multi-node external API multiplication) |

## Context

Iteration 5 introduced `OpenBoxesStatusPollerTask` and Iteration 4 introduced `CarrierStatusPollerTask`. Both tasks run on a 30-second interval and must detect state changes in external systems by comparing the API response against the last-known status of each open order or shipment.

Two problems arise when nopCommerce is scaled out horizontally:

**DB read multiplication (CON-29).** The straightforward implementation reads last-known status from the DB on every tick. Read load grows linearly with the number of open orders and compounds with node count — each additional node issues the same DB reads on every tick.

**External API multiplication.** `IScheduleTask` runs on every node independently with no built-in leader election. With N nodes, OpenBoxes and WireMock receive N calls per tick instead of one. These are external systems not owned by VerdeMart; call volume scales with the nopCommerce deployment size.

nopCommerce includes built-in support for Redis as a distributed cache, exposed through the `IStaticCacheManager` abstraction. No new infrastructure decision is required to use it.

## Decision

Introduce Redis for two distinct purposes in both polling tasks: a distributed lock to control which node runs each tick, and a read-through cache to avoid DB reads in the common case.

### Distributed Lock

**Lock key:** `verdemart:lock:openboxes-poller` and `verdemart:lock:carrier-poller` — one key per task, shared across all nodes.

**TTL:** 60 seconds — covers the full tick duration including the external API call and DB transaction; auto-expires if the node crashes mid-tick.

**Tick flow (lock phase):**

1. `SET verdemart:lock:openboxes-poller 1 NX EX 60`
2. Result `nil` → another node holds the lock; return immediately — no cache read, no API call, no DB access
3. Result `OK` → this node owns the tick; proceed

**Purpose:** ensures exactly one node calls the external API per tick regardless of how many nodes are running. Correctness does not depend on the lock — all writes are idempotent — but external API call multiplication is eliminated.

### Status Cache

**Cache key pattern:** `verdemart:openboxes:status:{OrderGuid}` for the OpenBoxes poller and `verdemart:carrier:status:{ExternalShipmentId}` for the carrier poller. One key per open order or shipment; value is the last-known status string.

**TTL:** 24 hours. An entry not refreshed within 24 hours is assumed to belong to a terminal order; expiry triggers a DB fallback on the next tick.

**Tick flow (cache phase, winning node only):**

1. Read last-known status from Redis (cache hit — no DB query)
2. Cache miss → read from DB, populate Redis
3. Call external API
4. Compare API response against cached status
5. On change: write updated status to DB, update Redis cache, release lock
6. On no change: release lock; nothing else written

**Write policy:** the Redis cache is updated after the DB transaction commits successfully. If the transaction rolls back, Redis is not updated, so the next tick re-detects the change and retries.

## Rejected Alternatives

**DB reads on every tick (original implicit design).**
Simple and correct, but read load grows linearly with open orders and scales with node count. No mechanism to absorb the common case (no change detected). *Rejected in favour of the cache design.*

**Redis as system of record (skip DB writes).**
The DB holds order and shipment state read by the storefront, admin UI, and all other nopCommerce services. Removing DB writes from the polling path would couple all consumers to a cache rather than the authoritative store. *Rejected* — the DB remains the system of record; Redis is a cache layer only.

**Application-level in-memory cache (per-node dictionary).**
Each node maintains its own in-memory map of last-known statuses. Simple, no Redis dependency. *Rejected:* each node holds an independent copy so two nodes polling concurrently both see a stale in-memory value and both call the external API and write to DB. The writes are idempotent but the external API multiplication is not resolved. A shared Redis cache combined with a lock eliminates both problems.

**Rely on idempotency alone (no lock, shared cache only).**
DB and Redis writes are idempotent, so concurrent nodes writing the same change is harmless. *Rejected for the lock specifically:* idempotency protects correctness but does not prevent N nodes from calling OpenBoxes and WireMock N times per tick. The lock costs one Redis call per node per tick and is negligible given Redis is already in the stack.

## Why This Decision Was Superseded

**No QA drives Redis.** Latency is governed by the polling interval, reliability by RabbitMQ and the outbox, and consistency by `TransactionScope`. Redis addresses none of these directly.

**Cache hit rate would be zero for the OpenBoxes poller.** Each fulfillment is processed exactly once. After processing, `ReceiveFulfillmentAsync` advances its status out of `ISSUED` in OpenBoxes so it never reappears in the poll batch. There is no repeated read of the same status to cache.

**The only valid Redis use case is a distributed lock for multi-instance deployments.** None of the QASs define a multi-instance requirement for VerdeMart. Introducing Redis infrastructure to solve a problem the QASs do not require violates the brief's explicit warning against technologies added without architectural purpose.

**The idempotency assumption changed.** The cache design assumed both pollers would compare a last-known status string against the API response. The carrier poller does this using `Shipment.ExternalShippingStatus` directly — no extra DB read, nothing to cache. The OpenBoxes poller does not compare statuses at all; it uses an `OpenBoxesReceiveConfirmed` flag on the `Shipment` entity. The cache key pattern `verdemart:openboxes:status:{OrderGuid}` maps to a mechanism that was never built.

## Consequences

- No Redis infrastructure is introduced.
- The multi-node external API multiplication concern is unresolved and documented as a known gap in ADR-008 and ADR-009. It has no practical effect at VerdeMart's current single-node deployment.
