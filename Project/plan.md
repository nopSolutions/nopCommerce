# Plan: Omnichannel Evolution of nopCommerce

## Part 1 delivered
- Main deliverable: [docs/part1/architecture-checkpoint.md](docs/part1/architecture-checkpoint.md)
- Diagrams: [docs/part1/diagrams.md](docs/part1/diagrams.md)
- 7-minute presentation script: [docs/part1/presentation-script.md](docs/part1/presentation-script.md)
- ADRs: [docs/adr/](docs/adr/)
- Feasibility spike: [docs/evidence/feasibility-spike.md](docs/evidence/feasibility-spike.md)

## Summary and Commitment
- Chosen scenario: **C — Omnichannel Commerce Core**, based on the assignment brief ([Group Assignment - Final Assignment.pdf](Group%20Assignment%20-%20Final%20Assignment.pdf)) and slides ([Assignment 2 — Architectural Evolution of nopCommerce.pdf](Assignment%202%20%E2%80%94%20Architectural%20Evolution%20of%20nopCommerce.pdf)).
- Commitment: demonstrate nopCommerce as a commerce core that accepts orders, integrates with external operations, remains useful when an external system fails, and recovers with traceability.
- Part 1, on **05/06 May 2026**: 7-minute presentation covering scenario, current-state analysis, bounded contexts, QA scenarios, ADD, target architecture, ADRs, risks, and spike.
- Final, on **02/03 June 2026**: runnable repository, short architecture report, ADRs, evidence pack, and live demo with degradation and recovery.

## Requirements and Scope
- Mandatory use cases:
  - Online purchase in nopCommerce with fulfillment through another channel, via WMS/warehouse simulator.
  - Cross-channel stock or order-state visibility, via POS/store simulator.
  - Degradation: WMS slow/unavailable/contradictory without blocking checkout or collapsing the main experience.
- Quality attribute scenarios:
  - Resilience: checkout continues when WMS is unavailable; order stays in "fulfillment pending".
  - Eventual consistency: duplicate/stale updates are ignored or marked for reconciliation.
  - Traceability: each order links OrderGuid, outbox message, worker log, WMS call, and note/status in nopCommerce.
  - Operability: demo shows queue depth, retries, DLQ, and degraded mode.
  - Performance: external integration is not synchronous in checkout.
- In scope:
  - nopCommerce plugin for omnichannel integration.
  - Asynchronous workflow with RabbitMQ.
  - Outbox/inbox, idempotency, retry, circuit breaker, and DLQ.
  - One independently deployable `.NET Worker` service.
  - Two simulators: WMS and POS.
  - C4 diagrams, ADRs, architecture report, setup instructions, and evidence pack.
- Out of scope:
  - Rewriting nopCommerce or extracting Order/Catalog as microservices.
  - Full real ERP/POS/WMS systems.
  - Keycloak, SSO, real payments, real shipping carriers.
  - Distributed exactly-once guarantees; at-least-once with idempotency is sufficient.

## Frameworks and Architecture
- Mandatory architectural framework: **ADD**, because the work is driven by quality attributes and architectural decisions. Reference: [SEI ADD](https://www.sei.cmu.edu/library/attribute-driven-design-method-collection/).
- Visual documentation: **C4 Model** for context/container/component/dynamic/deployment views. Reference: [C4 official site](https://c4model.com/).
- Decision documentation: short ADRs with one rejected alternative per decision.
- Technology stack:
  - Existing nopCommerce: ASP.NET Core / .NET 10, modular monolith, plugins, in-process events, scheduled tasks.
  - Plugin `Nop.Plugin.Misc.OmnichannelCore` with its own tables via migrations.
  - RabbitMQ + `RabbitMQ.Client`, publisher confirms, manual acknowledgements, and DLQ.
  - `.NET Worker Service` for external integration.
  - Polly for retry/circuit breaker on HTTP calls to the WMS simulator.
  - Docker Compose for nopCommerce, SQL Server, RabbitMQ, worker, WMS simulator, and POS simulator.
- Technical reference documentation:
  - [nopCommerce plugins](https://docs.nopcommerce.com/en/developer/plugins/index.html)
  - [nopCommerce plugin with data access](https://docs.nopcommerce.com/en/developer/plugins/plugin-with-data-access.html)
  - [nopCommerce scheduled tasks](https://docs.nopcommerce.com/en/developer/tutorials/scheduled-tasks.html)
  - [RabbitMQ reliability](https://www.rabbitmq.com/docs/reliability)
  - [Polly retry](https://www.pollydocs.org/strategies/retry)
  - [.NET Worker Services](https://learn.microsoft.com/en-us/dotnet/core/extensions/workers)

## Planned Implementation
- Inside nopCommerce:
  - Plugin listens to `OrderPlacedEvent` and writes `OmniOutboxMessage`.
  - Scheduled task publishes pending events to RabbitMQ with publisher confirms.
  - Internal endpoints authenticated by `X-Demo-Token` receive fulfillment/stock updates from the worker.
  - Tables: `OmniOutboxMessage`, `OmniInboxMessage`, `OmniOrderFulfillment`, `OmniStockSyncState`.
  - Do not modify core order status enums; use plugin projection and order notes for omnichannel states.
- Public events v1:
  - `commerce.order.placed.v1`: `messageId`, `orderGuid`, `orderId`, `items`, `storeId`, `createdOnUtc`.
  - `pos.stock.changed.v1`: `messageId`, `sku/productId`, `warehouseId`, `quantityOnHand`, `sourceVersion`.
  - `fulfillment.status.changed.v1`: `messageId`, `orderGuid`, `externalRequestId`, `status`, `trackingNumber`, `occurredOnUtc`.
- Independent worker service:
  - Consumes `commerce.order.placed.v1`, calls WMS simulator, and publishes/routes fulfillment states.
  - Consumes `pos.stock.changed.v1` and calls the plugin endpoint to update stock/projection.
  - Applies idempotency by `messageId`, exponential retry, circuit breaker, and DLQ.
- Simulators:
  - WMS: modes `normal`, `slow`, `unavailable`, `contradictory`.
  - POS: endpoint/script to emit a store sale or stock replenishment.
- Minimum ADRs:
  - Choose Scenario C and omnichannel focus.
  - Keep checkout/order/catalog inside the monolith.
  - Use outbox + RabbitMQ instead of synchronous WMS call.
  - Use simulators instead of real ERP/WMS/POS systems.
  - Prohibit shared database between nopCommerce and the worker.

## Validation and Deliverables
- Tests:
  - Unit tests for event serialisation, inbox/outbox idempotency, and fulfillment state transitions.
  - Integration tests with RabbitMQ and simulators.
  - Demo scripts: normal flow, WMS down, WMS recovery, POS stock update, duplicate/stale event.
- Evidence:
  - Screenshots/logs of order created, pending state, retries, DLQ, recovery, and updated stock.
  - Simple measurement: checkout not blocked by degraded WMS; compare normal flow vs. WMS slow/down.
  - Documented limitations: no exactly-once, demo-token authentication, simulators do not replace real systems.
- Artefacts:
  - `docs/architecture-report.md`: scenario, drivers, ADD, target architecture, evolution path, and limits.
  - `docs/adr/`: ADRs with rejected alternatives.
  - `docs/evidence/`: results, screenshots, logs, and reproduction instructions.
- Assumptions:
  - `dotnet` is not available in the local PATH; final validation must use Docker or install .NET 10 SDK.
