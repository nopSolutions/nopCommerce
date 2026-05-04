# Part 1 - Architecture Checkpoint

## 1. Scenario Choice

Selected scenario: **Scenario C - Omnichannel Commerce Core**.

VerdeMart Retail uses nopCommerce as the web storefront today. The strategic change is that nopCommerce must become the commerce core for a wider operating environment: web ordering, warehouse fulfillment, store/POS stock updates and customer-visible order state.

Business reason:

- Customers expect stock and order state to reflect what happens outside the web storefront.
- Store and warehouse operations must continue even when systems are delayed or temporarily unavailable.
- The company needs a credible path from modular monolith to wider ecosystem without rewriting nopCommerce.

The architectural problem is therefore **coordination under delay and degradation**, not feature accumulation.

## 2. Current-State Analysis

nopCommerce is a modular monolith with layered projects:

- `Nop.Web`: ASP.NET Core presentation and admin/public UI.
- `Nop.Web.Framework`: infrastructure, dependency registration, plugin support and scheduled tasks.
- `Nop.Services`: application services for orders, catalog, shipping, customers and plugins.
- `Nop.Data`: repositories, migrations and database access.
- `Nop.Core`: domain entities and events.
- `Plugins/*`: extension mechanism used by payment, shipping, tax, search and miscellaneous integrations.

Relevant strengths:

- It already has a plugin architecture, so omnichannel integration can be added without modifying large parts of core code.
- It already emits domain events in-process. `OrderProcessingService` publishes `OrderPlacedEvent` after the order is persisted and checkout data is reset.
- It already models products, warehouses, stock reservation, shipments and order status.
- It already supports scheduled tasks, useful for publishing outbox messages without making checkout wait for RabbitMQ.

Relevant limitations for Scenario C:

- Current events are in-process only. `EventPublisher` resolves local `IConsumer<T>` handlers and awaits them, so it is not a durable integration boundary.
- Checkout currently performs core order and stock work synchronously inside the monolith.
- External fulfillment, POS stock updates and delayed consistency are not explicit runtime concepts.
- There is no existing durable integration outbox/inbox for cross-system idempotency.

Key source observations:

- `Nop.Services.Events.EventPublisher` invokes in-process consumers directly: `nopCommerce/src/Libraries/Nop.Services/Events/EventPublisher.cs:20`.
- `OrderProcessingService.PlaceOrderAsync` publishes `OrderPlacedEvent` after the order has been saved: `nopCommerce/src/Libraries/Nop.Services/Orders/OrderProcessingService.cs:1617`.
- `ProductService.GetTotalStockQuantityAsync` already calculates stock across warehouses and reserved quantity: `nopCommerce/src/Libraries/Nop.Services/Catalog/ProductService.cs:1438`.
- `ProductService.AdjustInventoryAsync` already handles stock adjustment and warehouse reservation: `nopCommerce/src/Libraries/Nop.Services/Catalog/ProductService.cs:1699`.

## 3. Domain and Boundary Model

Relevant subdomains:

- **Commerce Core**: checkout, orders, customers, catalog, pricing and payment status. Owner: nopCommerce monolith.
- **Inventory Visibility**: product stock projection, warehouse-specific quantities and stale-state detection. Owner: nopCommerce plugin, fed by POS/WMS events.
- **Fulfillment Coordination**: handoff from placed order to warehouse processing. Owner: independent Omnichannel Worker.
- **Warehouse Operations**: external WMS simulator responsible for accept/reject/delay/contradict fulfillment requests.
- **Store Operations/POS**: POS simulator responsible for stock movements outside web checkout.
- **Integration Reliability**: outbox, inbox, retries, dead-letter handling and idempotency. Shared responsibility between plugin and worker, with separate storage.

Bounded contexts:

- **nopCommerce Core Context**: owns `Order`, `OrderItem`, `Product`, `ProductWarehouseInventory`, `Shipment`.
- **Omnichannel Integration Context**: owns outbox/inbox messages, fulfillment projection and stock sync state.
- **WMS Context**: owns external fulfillment request state and simulated degradation modes.
- **POS Context**: owns store-originated stock changes.

Boundary rule:

- The worker, WMS simulator and POS simulator must not read or write nopCommerce database tables directly.
- They communicate through RabbitMQ and plugin HTTP endpoints only.

## 4. Quality Attribute Scenarios

Summary table below. **Full SEI 6-part scenarios with numeric measures**: [quality-attribute-scenarios.md](quality-attribute-scenarios.md). Each scenario drives one ADD iteration in section 6.

| ID  | Attribute     | Stimulus                                       | Response                                                | Measure (full version in QA doc)              |
|-----|---------------|------------------------------------------------|---------------------------------------------------------|-----------------------------------------------|
| QA-1| Resilience    | WMS returns 503 for 30 s                       | Checkout completes; order pending; worker retries       | Checkout P95 ≤ 1.5× baseline; 0 checkout failures |
| QA-2| Consistency   | Duplicate or stale POS event                   | Inbox rejects duplicate; stale `sourceVersion` ignored  | Duplicate detection ≤ 50 ms; 0 duplicate fulfillment |
| QA-3| Traceability  | Support investigates a pending order           | Outbox/MQ/worker/projection linked by `OrderGuid`       | 100% of orders link end-to-end; ≤ 3 admin clicks |
| QA-4| Operability   | WMS in `slow` or `unavailable` mode            | Queue/retry/DLQ visible in dashboard                    | Single dashboard view; refresh ≤ 5 s          |
| QA-5| Performance   | `OrderPlacedEvent` fired                       | Plugin writes outbox row, no sync external call         | Outbox row ≤ 100 ms; 0 sync HTTP in checkout  |

## 5. Chosen Design Framework

Chosen method: **ADD - Attribute-Driven Design**, primary. **ACDM** governance vocabulary and **ADM Phase F** migration discipline are borrowed where ADD is silent.

### Why ADD, and why not ACDM or ADM

Scenario C has multiple interacting quality drivers (resilience, consistency, traceability, operability, performance) and demands a selective evolution of one existing product. ADD fits because it is iterative (one driver per iteration), driver-first (decompose only where pressure exists), and its 7-step iteration output (goal, drivers, refined element, concepts/tactics, responsibilities, interfaces, 2–5 decisions, analysis) maps directly to this rubric.

- **ADD over ACDM**: ACDM is single-pass single-driver (slide 11). Compressing five QA drivers into one pass loses fidelity; doing three independent ACDM passes is not what the method prescribes. ADD's iterative loop is the natural fit.
- **ADD over ADM**: ADM assumes enterprise breadth — business architecture, multiple delivery teams, cross-unit governance. We evolve one product. The rubric explicitly penalises inflated scope.

### What we borrow and why

| Borrowed                         | From  | Fills which ADD gap                          |
|----------------------------------|-------|----------------------------------------------|
| Go / partial-go / no-go decision | ACDM  | ADD does not define architecture evaluation  |
| Experiment charter framing       | ACDM  | Anchors the feasibility spike artefact       |
| Phase-F migration table          | ADM   | ADD does not cover migration sequencing      |
| Phase-H change vocabulary        | ADM   | ADD does not cover post-rollout governance   |

### How ADD is applied here

Three iterations, each producing the seven ADD outputs and ending with an ACDM-style go/no-go. Iteration goals, refined elements, tactics, and decisions are detailed in §6.

| # | Iteration goal (one driver)                                          | Element refined           | Decisions                          |
|---|----------------------------------------------------------------------|---------------------------|------------------------------------|
| 1 | Stay useful when WMS is slow or unavailable                          | Order → WMS edge          | ADR-0003, ADR-0005                 |
| 2 | Don't lose state under at-least-once delivery and stale POS updates  | Inbox / projection edge   | ADR-0006, ADR-0007                 |
| 3 | Make ops and audit able to explain a delayed/recovering order        | Cross-cutting traceability| ADR-0008                           |

Frame decisions (precede the iterations): ADR-0001 (scenario), ADR-0002 (keep core in monolith), ADR-0004 (simulators).

Visual notation: **C4 Model** for context, container, component and runtime diagrams.

## 6. Target Architecture

Components, ownership, and interaction style are described iteration by iteration below. Each iteration follows ADD's 7-step loop and ends with an ACDM-style go/no-go.

### Components and ownership (cross-iteration view)

| Component                | Owner team   | Responsibility                                                                 |
|--------------------------|--------------|--------------------------------------------------------------------------------|
| nopCommerce monolith     | nopCommerce  | Storefront, checkout, order, catalog, stock, shipments                         |
| Omnichannel Core plugin  | Omnichannel  | OrderPlacedEvent consumer, outbox, inbox, projections, internal callback API   |
| RabbitMQ                 | Omnichannel  | Durable async transport                                                        |
| Omnichannel Worker       | Omnichannel  | Consumes events, calls WMS, returns status; idempotency state                  |
| WMS simulator            | external     | Fulfillment behavior with normal/slow/unavailable/contradictory modes          |
| POS simulator            | external     | Store-originated stock change events                                           |

**Boundary rule** (ADR-0005): worker, WMS, POS never read or write nopCommerce DB. All cross-boundary communication is RabbitMQ events or plugin HTTP endpoints.

### Iteration 1 — Stay useful when WMS is slow or unavailable

- **Driver**: Resilience scenario QA-1 (see [QA scenarios](quality-attribute-scenarios.md)).
- **Element refined**: Order → WMS edge.
- **Tactics**: outbox, async messaging, retry with exponential backoff, circuit breaker, dead-letter queue.
- **Responsibilities**: plugin writes outbox row inside `OrderPlacedEvent` consumer; scheduled task publishes to RabbitMQ with publisher confirms; worker consumes, calls WMS, callbacks plugin.
- **Interfaces**: `commerce.order.placed.v1` (plugin → MQ → worker); `fulfillment.status.changed.v1` (worker → plugin HTTP).
- **Decisions**: [ADR-0003](../adr/0003-use-outbox-rabbitmq-for-fulfillment.md), [ADR-0005](../adr/0005-no-shared-database-boundaries.md).
- **Analysis**: checkout latency stays bounded under WMS 503 (validated by spike). DLQ contains poison messages without blocking the live path.
- **Go/Partial-Go/No-Go**: **Go** on outbox + RabbitMQ + worker. Risk: scheduled-task publish lag must stay under one minute under load — measured in Part 2.

### Iteration 2 — Don't lose state under at-least-once delivery and stale POS updates

- **Driver**: Consistency scenario QA-2.
- **Element refined**: Inbox / projection edge.
- **Tactics**: idempotent receiver (`messageId`), version-based stale detection (`sourceVersion`), local projection.
- **Responsibilities**: plugin records every external `messageId` in `OmniInboxMessage` before applying side effects; stock updates compare `sourceVersion`; projection table `OmniStockSyncState` shadows core stock without writing to it (initially).
- **Interfaces**: `pos.stock.changed.v1` (POS → MQ → worker → plugin HTTP).
- **Decisions**: [ADR-0006](../adr/0006-idempotency-strategy.md), [ADR-0007](../adr/0007-stock-projection-vs-writethrough.md).
- **Analysis**: duplicates rejected; older `sourceVersion` ignored; projection diverges from core stock only when external stock changes — surfaced as drift, not silently merged.
- **Go/Partial-Go/No-Go**: **Go** on idempotent inbox. **Partial-go** on projection-only stock; revisit write-through after Part 2 measures drift impact.

### Iteration 3 — Make ops and audit able to explain a delayed or recovering order

- **Driver**: Traceability scenario QA-3.
- **Element refined**: Cross-cutting correlation and observability.
- **Tactics**: correlation ID propagation, structured outbox/inbox state transitions, queue/retry/DLQ exposure.
- **Responsibilities**: every log line, message, and DB row carries `OrderGuid` + `messageId` + `externalRequestId`; admin view in plugin lists fulfillment state per order; RabbitMQ management UI exposes queue depth and DLQ.
- **Interfaces**: standard message envelope (`messageId`, `correlationId`, `eventType`, `occurredOnUtc`).
- **Decisions**: [ADR-0008](../adr/0008-correlation-and-traceability.md).
- **Analysis**: support can answer "why is this order pending?" from the plugin admin view alone, without code spelunking.
- **Go/Partial-Go/No-Go**: **Go** on correlation propagation. Risk: dashboard depth depends on Part 2 implementation budget.

### Required Technical Constraints — coverage map

| Constraint (assignment §)                              | Where satisfied                                          |
|--------------------------------------------------------|----------------------------------------------------------|
| 1. At least one asynchronous workflow                  | Iteration 1: outbox → RabbitMQ → worker                  |
| 2. At least one explicit reliability decision          | Iteration 1: ADR-0003 (retry/backoff, circuit breaker, DLQ); Iteration 2: ADR-0006 (idempotency)         |
| 3. Two surrounding systems represented                 | WMS simulator + POS simulator (ADR-0004)                 |
| 4. One independently deployable subsystem              | Omnichannel Worker                                       |
| 5. No shared database across extracted boundaries      | ADR-0005                                                 |
| 6. Justify what remains in the monolith                | ADR-0002                                                 |
| 7. Performance/resilience/operability/traceability evidence | Part 2 evidence pack (see Iteration 1–3 measures)   |

## 7. Architectural Decisions

Frame decisions:

- [ADR-0001 - Select Scenario C](../adr/0001-select-scenario-c-omnichannel.md)
- [ADR-0002 - Keep commerce core inside nopCommerce](../adr/0002-keep-commerce-core-inside-nopcommerce.md)
- [ADR-0004 - Use WMS and POS simulators](../adr/0004-use-wms-pos-simulators.md)

Iteration decisions:

- Iteration 1 (resilience): [ADR-0003 - Outbox + RabbitMQ](../adr/0003-use-outbox-rabbitmq-for-fulfillment.md), [ADR-0005 - No shared database](../adr/0005-no-shared-database-boundaries.md)
- Iteration 2 (consistency): [ADR-0006 - Idempotency strategy](../adr/0006-idempotency-strategy.md), [ADR-0007 - Projection vs write-through](../adr/0007-stock-projection-vs-writethrough.md)
- Iteration 3 (traceability): [ADR-0008 - Correlation and traceability](../adr/0008-correlation-and-traceability.md)

Each ADR includes Status, Context, Decision, Consequences, **Tradeoffs**, and Rejected Alternatives.

## 8. Risk and Validation Plan

| Risk                                                  | Why it matters                                | Validation                                           | Success signal                                                          |
|-------------------------------------------------------|-----------------------------------------------|------------------------------------------------------|-------------------------------------------------------------------------|
| Checkout accidentally depends on WMS availability     | Fails mandatory pressure point (QA-1)         | Spike + Part 2 e2e test with WMS sim returning 503   | Checkout test passes; checkout P95 ≤ 1.5× baseline                      |
| Duplicate messages create duplicate fulfillment        | At-least-once requires idempotency (QA-2)     | Inbox test with replayed `messageId`                 | 0 duplicate fulfillment rows; duplicate detected ≤ 50 ms                |
| Stock projection becomes misleading                    | Cross-channel visibility (QA-2, QA-3)         | POS stale-version scenario; admin drift view         | Older `sourceVersion` ignored; drift count visible in admin             |
| Too much scope for final delivery                      | Assignment rewards selective evolution        | Migration table (§9) caps stages to 5 moves          | All stages traceable to one ADD iteration; no out-of-scope work added   |
| nopCommerce plugin integration harder than expected    | Plugin is the main extension seam             | Part 1 spike maps extension points                   | Spike outcome documented in `feasibility-spike.md`; result = feasible   |

## 9. Evolution Roadmap

Format: ADM Phase-F migration table (stage / move / why now / what coexists / owner). Each stage is one move; nothing is replaced wholesale.

| Stage | Move                                                                  | Why now                                              | What still coexists                                      | Owner          |
|-------|-----------------------------------------------------------------------|------------------------------------------------------|----------------------------------------------------------|----------------|
| 1     | Plugin scaffolding + outbox/inbox/projection tables                   | Foundation for Iterations 1–3; lowest risk           | nopCommerce checkout, in-process events, existing plugins| Omnichannel    |
| 2     | RabbitMQ + worker + Iteration 1 normal flow (order → WMS sim)         | Validates async path end-to-end before pressure work | All Stage 1; WMS sim in `normal` mode only               | Omnichannel    |
| 3     | Iteration 1 pressure work: retry, circuit breaker, DLQ + WMS modes    | Demonstrates resilience under degradation            | Stage 2; reuses worker and plugin                        | Omnichannel    |
| 4     | Iteration 2: POS sim, idempotent inbox, stock projection              | Adds consistency dimension once resilience is stable | All Stages 1–3                                           | Omnichannel    |
| 5     | Iteration 3: correlation propagation + admin view + dashboard         | Closes traceability/operability rubric items         | All prior stages; instruments existing flows             | Omnichannel    |

### Coexistence rules

- nopCommerce DB stays single-owner. Worker, WMS, POS never read or write it (ADR-0005).
- In-process `OrderPlacedEvent` continues to exist for non-omnichannel consumers. The plugin **adds** a consumer; it does not replace the event.
- WMS/POS simulators replace real systems only for the demo. Production swap-out is out of scope; contracts (`commerce.order.placed.v1`, `pos.stock.changed.v1`, `fulfillment.status.changed.v1`) are versioned to allow it.
- No core nopCommerce service is modified. Plugin migrations own all new tables.

## 10. Feasibility Spike (Experiment Charter)

Framed as an ACDM-style experiment charter:

- **Question**: can a placed order start an asynchronous omnichannel workflow without making checkout depend on WMS/POS availability and without rewriting core order processing?
- **Success signal**: a plugin consumer of `OrderPlacedEvent` can write a durable outbox row in < 100 ms and return; a scheduled task can publish that row to RabbitMQ later; no synchronous external HTTP on the checkout thread.
- **If it fails**: the architecture must change — either extract order processing or accept synchronous WMS coupling. Both would invalidate the current target architecture.
- **Outcome**: **feasible**. `OrderPlacedEvent` is published after order persistence (`OrderProcessingService.cs:1617`); plugins can consume it and own their tables; scheduled tasks decouple publish from checkout.

See [feasibility spike](../evidence/feasibility-spike.md) for concrete event shape, source insertion points and Part 2 implementation notes.

