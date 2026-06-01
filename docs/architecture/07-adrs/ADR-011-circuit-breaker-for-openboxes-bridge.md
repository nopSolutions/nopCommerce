# ADR-011 — Circuit Breaker for OpenBoxes Bridge Resilience

| | |
|---|---|
| Status | Accepted |
| Date | 2026-06-01 |
| Produced by | Post-iteration resilience improvement — driven by QA finding: 12 orders lost to DLQ during a 30-minute OpenBoxes outage |

## Context

ADR-007 established the OpenBoxes Bridge as a separate deployable service that consumes `verdemart.orders.openboxes` and calls the OpenBoxes HTTP API. ADR-003 established durable queues and manual ack to guarantee at-least-once delivery.

Under normal operation these decisions are sufficient. However, a QA scenario exposed a gap: if OpenBoxes is down for an extended period (e.g. 30 minutes), the bridge exhausts its retry budget (~30 seconds of exponential backoff across 5 attempts) and sends orders to the Dead Letter Queue. Recovery then requires a human operator to manually republish messages from the DLQ via the RabbitMQ Management UI.

This creates two problems:

1. **DLQ semantics are violated.** The DLQ is architecturally intended to hold messages that are genuinely unprocessable — malformed payloads, permanent business-level rejections. Transient outages are not in that category. Mixing them pollutes the DLQ signal: an operator looking at the DLQ cannot distinguish "bad data" from "OpenBoxes was down for 30 minutes".

2. **Recovery requires manual intervention.** The architecture's at-least-once guarantee depends on durable queues and manual ack, but those guarantees are voided once a message lands in the DLQ and no operator is available to replay it.

The question is: how should the bridge behave when OpenBoxes is known to be down, and how should it recover automatically when OpenBoxes comes back?

## Decision

Implement a **circuit breaker** pattern inside the bridge. The circuit breaker is a singleton (`CircuitBreaker`) that tracks OpenBoxes reachability across all message deliveries. `BridgeWorker` uses the circuit state to actively manage the RabbitMQ consumer lifecycle.

### State machine

```
CLOSED ──(N consecutive transient failures)──► OPEN
  ▲                                              │
  │                                    (OpenTimeoutSeconds elapsed)
  │                                              │
  └──(probe succeeds)── HALF-OPEN ◄─────────────┘
                              │
                         (probe fails)
                              ▼
                            OPEN (timer resets)
```

- **Closed**: normal operation. Calls to OpenBoxes go through. Consecutive transient failure count is tracked.
- **Open**: OpenBoxes is known unreachable. `BridgeWorker` calls `BasicCancelAsync` to stop accepting new messages. Messages already in the main queue remain there untouched. No calls to OpenBoxes are attempted.
- **Half-Open**: after `CircuitBreakerOpenTimeoutSeconds` (default 60s), `BridgeWorker` calls `BasicConsumeAsync` to resume consumption. The first delivered message is used as a probe call to OpenBoxes. Success closes the circuit; failure re-opens it with a reset timer.

### Consumer lifecycle

`BridgeWorker` runs a loop: start consumer → poll circuit state every 200 ms → when circuit opens, call `BasicCancelAsync` → poll every 1s until circuit reaches HalfOpen → loop back to start consumer. This is the key difference from the previous design: the consumer is no longer a passive always-on subscription. It is explicitly started and stopped based on downstream health.

### Message routing

| Event | Route |
|---|---|
| Circuit open on delivery (race window) | `BasicNackAsync(requeue: true)` — message stays in main queue |
| Transient failure, circuit still closed | `BasicNackAsync(requeue: true)` with exponential backoff |
| Transient failure opens the circuit | `BasicNackAsync(requeue: true)` — consumer cancelled immediately after |
| Permanent failure (non-transient HTTP error) | `BasicNackAsync(requeue: false)` → DLQ |
| Parse / deserialise failure | `BasicNackAsync(requeue: false)` → DLQ |
| Success | `BasicAckAsync` + dedup recorded |

The DLQ now strictly receives messages that are genuinely unprocessable. Transient failures never reach it.

### Configuration

| Setting | Default | Meaning |
|---|---|---|
| `CircuitBreakerFailureThreshold` | 3 | Consecutive transient failures before opening |
| `CircuitBreakerOpenTimeoutSeconds` | 60 | Seconds circuit stays open before half-open probe |

## Rejected Alternatives

**DLQ replay worker (polling + `BasicGetAsync`).**
A `BackgroundService` that periodically probes OpenBoxes health and, when healthy, drains the DLQ by republishing messages to the main exchange. *Rejected:* this re-processes all DLQ messages indiscriminately, including genuine poison messages (parse failures, permanent errors) that will fail again and bounce back to the DLQ. It also requires a health probe loop, adds a second worker, and introduces an extra RabbitMQ hop per message (DLQ → exchange → main queue). Most importantly, it does not fix the root cause — it merely adds a recovery mechanism on top of a design that still routes transient failures to the wrong queue.

**Delayed retry queue (TTL + dead-letter back to main exchange).**
Instead of the DLQ, transient-exhausted messages are published to a separate queue with `x-message-ttl` equal to the circuit timeout and `x-dead-letter-exchange` pointing back to the main exchange. RabbitMQ automatically returns them after the TTL. *Rejected:* this approach is correct in that it preserves DLQ semantics, but it requires messages to exhaust their retry budget (5 attempts, ~30s) before being parked — even when OpenBoxes has been known unreachable since the first attempt. The circuit breaker avoids this: once the circuit opens, subsequent messages skip retries entirely and stay in the main queue with no wasted attempts. The TTL queue also adds a topology element and introduces a fixed replay delay regardless of when OpenBoxes actually recovers, whereas the half-open probe fires exactly at the timeout boundary.

**Increase `MaxRedeliveryAttempts` indefinitely.**
Raising the retry ceiling means messages remain in the main queue longer before reaching the DLQ. *Rejected:* this is not a circuit breaker — it is a delay. With exponential backoff capped at 30s, a sufficiently long outage still exhausts any finite retry budget. It also holds the consumer thread in a delay loop for every message during the outage, blocking other messages from being processed.

## Consequences

- Transient outages no longer send any message to the DLQ. The DLQ signal is restored to its intended meaning: bad data or permanent rejection.
- No operator intervention is required for recovery. The bridge self-heals within one circuit timeout after OpenBoxes comes back.
- Messages are never lost: they remain in the durable main queue while the circuit is open, honouring the at-least-once guarantee established in ADR-003.
- The bridge is no longer a passive consumer. `BridgeWorker` actively manages the consumer lifecycle, adding a small polling overhead (200 ms poll on circuit state).
- The circuit breaker state is in-process memory (singleton). If the bridge restarts while the circuit is open, the state resets to Closed. The first message after restart will probe OpenBoxes and re-open the circuit if it is still down — at the cost of one extra failed attempt per restart.
- A new operational signal is available: the circuit state transitions are logged at Warning level, making OpenBoxes outages visible in structured logs without requiring RabbitMQ Management UI inspection.
