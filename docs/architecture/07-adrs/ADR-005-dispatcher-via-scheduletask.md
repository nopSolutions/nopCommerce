# ADR-005 — Dispatcher Hosted in IScheduleTask

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-27 |
| Produced by | ADD Iteration 2 — Step 6 (driver: QAS-1; constraint: minimise deployable count) |

## Context

ADR-004 introduces an outbox dispatcher: a component that periodically polls the `Outbox` table and publishes pending rows to RabbitMQ. Where this component runs is a separate decision with operational consequences.

The relevant constraints:
- VerdeMart's deployment surface should remain small — adding processes adds operational load.
- The dispatcher must run continuously and restart automatically on process failure.
- nopCommerce already provides an in-process scheduler (`IScheduleTask`) used by other periodic jobs.

## Decision

Implement the dispatcher as an `IScheduleTask` that runs inside the existing nopCommerce process. The scheduler entry is registered at plugin install with a 1-second interval (configurable via `OutboxSettings`). Single-instance execution is enforced by nopCommerce's scheduler within one process; for future multi-node deployments, the dispatcher's polling query uses `FOR UPDATE SKIP LOCKED` so two instances cannot publish the same row.

## Rejected Alternatives

**Separate worker process (e.g. .NET hosted service in its own container).**
*Rejected:* adds a new deployable, a new process to monitor, and new code paths for DB connection management. The benefits — independent scaling, isolation from web traffic — are not justified at VerdeMart's scale. The decision is reversible: if scale or contention forces extraction later, the dispatcher logic can be lifted into its own host without changing publishing semantics or any contract.

**Multi-instance dispatcher with leader election (Redis lock or dedicated lock service).**
*Rejected:* introduces new infrastructure (lock service) and new failure modes (split-brain) for a property that is not yet needed. Single-instance + DB row-level lock is sufficient and supports horizontal scale-out later if required.

**Database push via `LISTEN/NOTIFY`.**
*Rejected:* MySQL has no native `LISTEN/NOTIFY`. Switching the database or adding a pub/sub layer would buy sub-second push latency, but QAS-1's 60-second recovery clause does not require it. A 1-second poll has ample headroom.

## Consequences

- No new processes, containers, or hosting concerns are added to VerdeMart's deployment surface.
- The dispatcher inherits nopCommerce's scheduler reliability — automatic restart on process restart, logging into the existing admin log.
- The dispatcher is a single point of failure tied to the web process; if all web nodes are down, no publishing happens. This is recorded as a residual risk in Iteration 2 Step 7.
- The polling-based design adds at most one polling interval (1 s default) of latency between order commit and broker arrival.
