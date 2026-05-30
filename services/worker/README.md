# Omnichannel Worker

Independently deployable .NET Worker Service that owns the **send-out** edge of
the omnichannel integration (Pair B). It is the assignment's *independently
deployable subsystem* and carries the *async workflow* + *explicit reliability
decision* (retry + circuit breaker + DLQ).

> **Status: scaffold (Phases 2–3).** Consume + WMS call + ack/nack/DLQ are wired.
> Posting `fulfillment.status.changed.v1` back to the plugin callback is stubbed
> (logs only), and the Polly thresholds are not yet tuned to the QA-1 measures.
> Build it, then finish + measure before the demo.

## Flow

```
RabbitMQ (commerce.order.placed.v1)
        │  consume (manual ack)
        ▼
   OrderPlacedConsumer ──► WmsClient ──► WMS sim  POST /fulfillments
        │                     │  (Polly retry + circuit breaker)
        │                     ▼
        │            FulfillmentStatusChanged
        ▼
  POST fulfillment.status.changed.v1 → nopCommerce plugin callback   [TODO Phase 2]
        │
   on failure → BasicNack(requeue:false) → DLQ
```

## RabbitMQ topology

Declared idempotently on startup (`OrderPlacedConsumer.DeclareTopologyAsync`),
names centralised in `contracts/Topology.cs`:

| Object | Name | Notes |
|--------|------|-------|
| Exchange (topic) | `commerce` | order events from the plugin outbox |
| Queue | `wms.order.placed` | bound on `commerce.order.placed.v1`; dead-letters to the DLX |
| Dead-letter exchange | `commerce.dlx` | poison messages |
| Dead-letter queue | `wms.order.placed.dlq` | inspected during QA-4 (operability) |
| Exchange (topic) | `fulfillment` | reserved for fulfillment results |

Main-queue args: `x-dead-letter-exchange=commerce.dlx`,
`x-dead-letter-routing-key=commerce.order.placed.v1`.

## Reliability decision (QA-1)

`Resilience/WmsResiliencePipeline.cs` builds: **exponential backoff retry** (jitter,
`MaxRetryAttempts`) → **circuit breaker** (`FailureRatio`, `MinimumThroughput`,
`BreakDuration`). When the breaker opens, fulfillments should be marked
`pending/degraded` and the backlog should drain on close. **Record the tuned
thresholds here** once QA-1 is measured (target: checkout P95 ≤ 1.5× baseline
during a 30 s WMS 503; backlog drain ≤ 60 s; 0 orders pending > 5 min).

## Configuration

Bound from the `Worker` section of `appsettings.json` or `Worker__*` env vars
(see `WorkerOptions.cs`). Defaults target the docker-compose service names.

## Build & run

```bash
# from services/ (Dockerfile expects worker/ and contracts/ in build context)
docker build -f worker/Dockerfile -t omni-worker ..
```

Normally started via the root `docker-compose.yml` alongside RabbitMQ + wms-sim.
