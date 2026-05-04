# ADR-0010 - Reject Distributed Tracing Infrastructure (OpenTelemetry / Jaeger / Tempo)

## Status

**Rejected.**

## Context

QA-3 (Traceability) requires linking outbox row → MQ message → worker attempt → projection per order, end-to-end. The industry-standard answer is distributed tracing: OpenTelemetry SDK in every service, a collector (OTLP), a backend store (Jaeger, Tempo, or equivalent), and a query UI. This was considered as either the primary mechanism for QA-3 or a supplement to ADR-0008 (3-ID correlation).

## Decision

We will **not** add distributed-tracing infrastructure for Part 1 or Part 2. Traceability is delivered by the explicit 3-ID correlation in ADR-0008 (`OrderGuid` + `messageId` + `externalRequestId`) propagated through structured logs, message envelopes, and DB rows, plus a plugin admin view that surfaces them.

## Reasons

- QA-3's measure (*"≤ 3 admin clicks to find a pending order; 100% link end-to-end"*) is satisfied by the plugin admin view and structured logs. Tracing infra would not change the user-facing answer.
- Tracing adds a collector, a storage backend, and a UI to the deployment surface — three new operational concerns for a demo.
- Instrumenting nopCommerce (a third-party monolith we deliberately do not modify, ADR-0002) would either require code changes we have ruled out or auto-instrumentation that we cannot fully validate in the demo window.
- Rubric penalises tooling-for-its-own-sake; the team must defend every component.

## Consequences

- Forensic queries across services are done by filtering structured logs on `OrderGuid` or `messageId`. The IDs already propagate end-to-end.
- No span-level timing UI; cross-service timing analysis is manual when needed.
- The architecture is **trace-ready** without being trace-instrumented: the 3-ID propagation that ADR-0008 mandates is exactly what any tracing system would consume. Adding tracing later is additive, not reworking.

## Tradeoffs

- Less rich timing data: we cannot easily say "WMS call took 87 ms at p95" from a UI; we can compute it from logs.
- SLO-style cross-service latency monitoring (if it becomes a Part 2 ask) would still need additional tooling.
- Visualising long-tail latency anomalies is harder without span flame graphs.

## Triggers to revisit

Reopen this decision if any of the following becomes true:

- An SLO or contract-driven cross-service latency requirement emerges (e.g., "p99 order → fulfillment-accepted under N seconds").
- Operators repeatedly ask cross-service timing questions that logs cannot answer cheaply.
- Production usage exposes a long-tail anomaly that requires span-level analysis to diagnose.
- A platform team takes ownership of observability and provides tracing as a shared service (zero marginal cost to us).
