# ADR-0008 - Correlation IDs Propagated End-to-End

## Status

Accepted.

## Context

QA-3 requires that a support agent can answer "why is order X pending?" without reading code. This needs every artefact in the integration path — order row, outbox row, MQ message, worker attempt, fulfillment projection — to be linkable from a single identifier.

## Decision

Three identifiers, present on every artefact:

- `OrderGuid` — already exists in nopCommerce; stable for the lifetime of the order.
- `messageId` — UUIDv4, stamped at the producer, travels with every MQ message and is logged at every consumer.
- `externalRequestId` — stamped by the worker when it calls WMS; returned in WMS responses and logged on the plugin callback.

All structured logs and DB rows in the integration path carry these three. The plugin admin view indexes by `OrderGuid` and reveals the full chain.

Standard message envelope:

```json
{
  "messageId": "<uuid>",
  "correlationId": "<OrderGuid or upstream messageId>",
  "eventType": "commerce.order.placed.v1",
  "occurredOnUtc": "<ISO-8601>"
}
```

## Consequences

- Support flow: enter `OrderGuid` → see outbox row, MQ message, worker attempt, projection — in ≤ 3 admin clicks (QA-3 measure).
- Logs are queryable end-to-end without joining across systems by timestamp.
- Producers and consumers must conform to the envelope; plugin enforces it on inbound messages.

## Tradeoffs

- Storing three IDs per row adds storage cost (small).
- Worker must propagate `correlationId` even on error paths; a bug here makes traceability silently fail.
- We commit to UUIDv4 for `messageId`; switching format later requires migration.

## Rejected Alternatives

- **Timestamp-based correlation only**. Rejected: clock skew across systems makes this unreliable for forensic queries.
- **Distributed tracing (OpenTelemetry) instead of explicit IDs**. Rejected: adds Part 2 scope (collector, storage, UI). Identifiers in admin view satisfy QA-3 with less infra.
- **Single ID (just `messageId`)**. Rejected: support agents start from `OrderGuid` (the customer-facing identifier); requiring them to find a `messageId` first defeats QA-3.
