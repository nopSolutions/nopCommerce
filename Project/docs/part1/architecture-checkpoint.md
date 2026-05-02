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

| Attribute | Scenario | Response | Measure |
| --- | --- | --- | --- |
| Resilience | WMS is unavailable when an order is placed | Checkout succeeds, outbox records event, fulfillment remains pending and worker retries | User receives order confirmation; no checkout failure caused by WMS |
| Consistency | POS sends duplicate or stale stock event | Plugin inbox detects duplicate/stale `messageId` or `sourceVersion` | Duplicate ignored; stale update marked as ignored/reconciled |
| Traceability | Reviewer asks why an order is pending | OrderGuid links outbox message, worker attempt and fulfillment projection | Evidence pack includes order, message and retry log |
| Operability | WMS outage persists | RabbitMQ queue depth/retry/DLQ becomes visible in demo logs | Demo shows pending/retry/DLQ state |
| Performance | WMS is slow for 10 seconds | Checkout does not wait for WMS response | Checkout duration remains close to normal-path baseline |

## 5. Chosen Design Framework

Chosen method: **ADD - Attribute-Driven Design**.

Why ADD fits:

- The assignment is explicitly driven by quality attributes such as resilience, consistency, operability and traceability.
- The target architecture is a selective evolution, not a full enterprise architecture program.
- ADD lets the team start from drivers, choose tactics and then justify concrete design decisions.

ADD application:

1. Identify drivers: omnichannel visibility, resilience under degradation, traceable recovery.
2. Select architectural tactics: asynchronous messaging, outbox, idempotent consumers, retry/DLQ, circuit breaker, projection state.
3. Allocate responsibilities: nopCommerce owns commerce core; worker owns external coordination; simulators create pressure.
4. Validate with scenarios: normal order, WMS down, WMS recovery, POS stock update, duplicate/stale event.

Visual notation: **C4 Model**, used for context, container, component and runtime diagrams.

## 6. Target Architecture

Main components:

- **nopCommerce monolith**: existing storefront, checkout, order and catalog behavior.
- **Omnichannel Core plugin**: listens to `OrderPlacedEvent`, stores outbox messages, exposes internal update endpoints and maintains projection tables.
- **RabbitMQ**: durable asynchronous transport between nopCommerce and the worker.
- **Omnichannel Worker**: independently deployable service that consumes integration events, calls external systems and returns status updates.
- **WMS simulator**: receives fulfillment requests and can act normal, slow, unavailable or contradictory.
- **POS simulator**: emits stock changes from store operations.

Data ownership:

- nopCommerce owns orders, products, stock and shipments.
- Plugin owns integration metadata and projections: outbox, inbox, fulfillment state, stock sync state.
- Worker owns only its local delivery/idempotency state.
- WMS/POS simulators own their own simulated state.

Interaction style:

- Checkout path stays synchronous inside nopCommerce.
- Integration path is asynchronous: `OrderPlacedEvent -> Outbox -> RabbitMQ -> Worker -> WMS -> Plugin callback`.
- POS stock updates are asynchronous: `POS -> RabbitMQ -> Worker -> Plugin endpoint`.

Cross-cutting decisions:

- At-least-once delivery with idempotent consumers.
- Retry with backoff and DLQ for persistent failures.
- Circuit breaker around WMS calls.
- Correlation by `OrderGuid`, `messageId` and `externalRequestId`.
- No shared database across extracted service boundaries.

## 7. Architectural Decisions

Initial ADRs:

- [ADR-0001 - Select Scenario C](../adr/0001-select-scenario-c-omnichannel.md)
- [ADR-0002 - Keep commerce core inside nopCommerce](../adr/0002-keep-commerce-core-inside-nopcommerce.md)
- [ADR-0003 - Use outbox and RabbitMQ for fulfillment integration](../adr/0003-use-outbox-rabbitmq-for-fulfillment.md)
- [ADR-0004 - Use WMS and POS simulators](../adr/0004-use-wms-pos-simulators.md)
- [ADR-0005 - Do not share databases across service boundaries](../adr/0005-no-shared-database-boundaries.md)

## 8. Risk and Validation Plan

| Risk | Why it matters | Validation |
| --- | --- | --- |
| Checkout accidentally depends on WMS availability | Would fail mandatory pressure point | Spike proves event can be captured after order placement and published later |
| Duplicate messages create duplicate fulfillment | At-least-once messaging requires idempotency | Inbox test and duplicate demo event |
| Stock projection becomes misleading | Cross-channel visibility depends on freshness | POS stale-version scenario |
| Too much scope for final delivery | Assignment rewards selective evolution | Keep WMS/POS as simulators and avoid real ERP/POS setup |
| nopCommerce plugin integration is harder than expected | Plugin is the main extension seam | Part 1 spike maps extension points and final plugin files |

## 9. Evolution Roadmap

Phase 1 - Checkpoint:

- Decide scenario C and quality attributes.
- Document current architecture pressure points.
- Define target architecture and ADRs.
- Validate extension feasibility through spike.

Phase 2 - Minimal runtime skeleton:

- Create `Nop.Plugin.Misc.OmnichannelCore`.
- Add plugin tables and admin/internal views for outbox/projections.
- Add worker, RabbitMQ, WMS simulator and POS simulator to Docker Compose.

Phase 3 - Normal flow:

- Capture `OrderPlacedEvent`.
- Publish `commerce.order.placed.v1`.
- Worker calls WMS simulator.
- Plugin records fulfillment status.

Phase 4 - Pressure and recovery:

- Add WMS `slow`, `unavailable` and `contradictory` modes.
- Add retry, circuit breaker and DLQ.
- Add POS stock update flow.
- Record evidence pack.

## 10. Feasibility Spike

Spike result: **feasible**.

The riskiest design point is whether a placed order can be turned into a durable external integration message without rewriting checkout.

Findings:

- nopCommerce already publishes `OrderPlacedEvent` after order persistence.
- Plugins can register consumers and services through the existing plugin infrastructure.
- A plugin can create its own tables using nopCommerce migration patterns.
- A scheduled task can later publish pending outbox rows, keeping RabbitMQ off the checkout critical path.

See [feasibility spike](../evidence/feasibility-spike.md) for concrete event shape, insertion points and Part 2 implementation notes.

