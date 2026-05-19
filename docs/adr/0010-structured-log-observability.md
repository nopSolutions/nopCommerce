# ADR-0010 - Structured-Log Observability with Explicit Correlation IDs

## Status

Accepted.

## Context

QA-3 (Traceability) requires linking outbox row → MQ message → worker attempt → projection per order, end-to-end. QA-4 (Operability) requires that operators can see queue, retry, and DLQ state during degradation. Several observability mechanisms were considered, from full distributed-tracing infrastructure (OpenTelemetry SDK + collector + storage backend + UI) to ad-hoc log search.

The chosen mechanism must answer the SEI scenario measures (≤ 3 admin clicks for a pending order; queue/retry/DLQ visible in one dashboard view) without adding deployment surface that we then have to defend at the demo.

This decision sits on top of ADR-0008: ADR-0008 says **which** identifiers exist (`OrderGuid` + `messageId` + `externalRequestId`); ADR-0010 says **how** we observe and surface them.

## Decision

Observability is delivered by three concrete components:

1. **Structured logs** at every step (plugin, worker, scheduled tasks) carrying the 3-ID set from ADR-0008. Field names are deterministic so the logs are queryable end-to-end without joining by timestamp.
2. **The plugin admin view**, indexed by `OrderGuid`, that surfaces outbox row, MQ message ID, worker attempts, and fulfillment projection state. This is the support entry point for "why is this order pending?".
3. **RabbitMQ Management UI** for queue depth, retry rate, and DLQ size — used directly by operators during the QA-4 demo scenario.

No tracing collector, no separate trace storage backend, and no tracing UI are added.

## Consequences

- Forensic queries across services are done by filtering structured logs on `OrderGuid` or `messageId`. The IDs already propagate end-to-end (ADR-0008), so no joins by timestamp are required.
- The architecture is **trace-ready** without being trace-instrumented: the 3-ID propagation that ADR-0008 mandates is exactly what any tracing system would consume. Adding tracing later is additive, not reworking.
- Operators have a single concrete answer for QA-4: open RabbitMQ Management for transport state, open the plugin admin view for domain state.
- QA-3's measure ("≤ 3 admin clicks to find a pending order; 100% link end-to-end") is satisfied by the plugin admin view and structured logs; tracing infra would not change the user-facing answer.

## Tradeoffs

- Less rich timing data: we cannot easily say "WMS call took 87 ms at p95" from a UI; we can compute it from logs.
- SLO-style cross-service latency monitoring (if it becomes a Part 2 ask) would still need additional tooling.
- Visualising long-tail latency anomalies is harder without span flame graphs.
- Log retention becomes a real constraint — if logs roll off before a support inquiry lands, the cross-service trace is gone.

## Rejected Alternatives

- **OpenTelemetry / Jaeger / Tempo distributed-tracing stack.** Rejected because adding a collector, a storage backend, and a UI for a demo we can already answer with structured logs is exactly the *"impress by increasing the number of technologies"* failure the rubric penalises. Instrumenting nopCommerce (a third-party monolith we deliberately do not modify, ADR-0002) would either require code changes ruled out by ADR-0002 or auto-instrumentation we cannot fully validate in the demo window.
- **APM-only (e.g., Application Insights, New Relic).** Rejected because we control neither nopCommerce's instrumentation depth nor the storage tier, and the demo cannot prove anything is actually reaching the backend within the demo window.
- **Free-text logs (no structured fields).** Rejected because correlation across services requires deterministic field names; free-text loses the ability to query end-to-end on `OrderGuid` or `messageId` without brittle regex.

## Triggers to revisit

Reopen this decision if any of the following becomes true:

- An SLO or contract-driven cross-service latency requirement emerges (e.g., "p99 order → fulfillment-accepted under N seconds").
- Operators repeatedly ask cross-service timing questions that logs cannot answer cheaply.
- Production usage exposes a long-tail anomaly that requires span-level analysis to diagnose.
- A platform team takes ownership of observability and provides tracing as a shared service (zero marginal cost to us).
