# nopCommerce Architecture Analysis

**Purpose**: Current-state analysis of nopCommerce as the baseline for the architectural evolution defined in Assignment 2.

**Assignment Context**: Assignment 2 — Architectural Evolution of nopCommerce (Scenario C: Omnichannel Commerce Core). This document identifies the strengths and pressure points of the existing architecture that directly motivate the target architecture decisions.

---

## High-Level Architecture

nopCommerce follows a layered architecture with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
│  ┌──────────────┐        ┌───────────────────┐             │
│  │  Nop.Web     │        │ Nop.Web.Framework │             │
│  │ (Controllers │        │   (Infrastructure │             │
│  │   Views)     │        │    for Web)       │             │
│  └──────┬───────┘        └─────────┬─────────┘             │
└─────────┼──────────────────────────┼───────────────────────┘
          │                          │
          ▼                          ▼
┌─────────────────────────────────────────────────────────────┐
│                     SERVICE LAYER                            │
│                   Nop.Services                               │
│  ┌──────────┐  ┌──────────┐  ┌─────────┐  ┌──────────┐    │
│  │  Orders  │  │ Catalog  │  │Customers│  │ Payments │    │
│  └────┬─────┘  └────┬─────┘  └────┬────┘  └────┬─────┘    │
└───────┼─────────────┼─────────────┼────────────┼───────────┘
        │             │             │            │
        ▼             ▼             ▼            ▼
┌─────────────────────────────────────────────────────────────┐
│                      DATA LAYER                              │
│                     Nop.Data                                 │
│  ┌───────────────────────────────────────────────┐          │
│  │  Repository Pattern (IRepository<T>)          │          │
│  │  Entity Framework Core                        │          │
│  └───────────────────────────────────────────────┘          │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                       CORE LAYER                             │
│                      Nop.Core                                │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│  │ Domain   │  │  Events  │  │ Caching  │  │ Security │   │
│  │ Models   │  │(IEvent   │  │          │  │          │   │
│  │          │  │Publisher)│  │          │  │          │   │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘   │
└─────────────────────────────────────────────────────────────┘

                    ┌──────────────────┐
                    │     PLUGINS      │
                    │  (Payments, Tax, │
                    │   Shipping, etc.)│
                    └──────────────────┘
                    Cross-cuts all layers
```

---

## Layer Overview

- **Nop.Core** — domain entities (`Order`, `Product`, `Customer`, etc.), domain events (`OrderPlacedEvent`), core interfaces (`IEventPublisher`). No dependencies on other layers.
- **Nop.Data** — repository pattern (`IRepository<T>`), Entity Framework Core, migrations. Depends only on Nop.Core.
- **Nop.Services** — business logic across 40+ service areas (orders, catalog, shipping, payments, scheduling, etc.). Publishes domain events at key points. Depends on Nop.Core and Nop.Data.
- **Nop.Web / Nop.Web.Framework** — ASP.NET Core controllers, views, model factories, filters, and plugin infrastructure. Depends on Nop.Services.
- **Plugins** — independently loaded projects that implement specific interfaces and can add controllers, services, and migrations. Cross-cut all layers.

---

## Dependency Rules

```
Presentation (Nop.Web, Nop.Web.Framework)
    ↓
Service Layer (Nop.Services)
    ↓
Data Layer (Nop.Data)
    ↓
Core Layer (Nop.Core)  ← no outward dependencies
```

Plugins may depend on any layer. External services (worker, simulators) must not depend on any layer directly — they communicate through RabbitMQ and plugin HTTP endpoints only.

---

## Key Architectural Patterns

- **Repository pattern** — all database access goes through `IRepository<T>`; no direct SQL in services.
- **Domain events** — services publish typed events via `IEventPublisher` at key business moments.
- **Plugin model** — plugins register services, consumers, controllers, and migrations without modifying core code.
- **Scheduled tasks** — background work runs through `IScheduleTask`, used for cache warm-up, email, and similar periodic operations.
- **Constructor injection** — all dependencies are resolved via DI; services commonly have 15–25 injected dependencies.

---

## Architectural Pressure Points for Scenario C

These are the specific gaps between the current nopCommerce architecture and what Scenario C (Omnichannel Commerce Core) requires. They directly motivate the target architecture decisions in `docs/part1/architecture-checkpoint.md` §6.

In ADD terms, this list is the **input set for Iteration 1** (resilience driver). Each pressure point becomes either an iteration goal, a constraint, or an explicit out-of-scope decision.

### 1. In-Process Events Are Not a Durable Integration Boundary

`IEventPublisher.PublishAsync` resolves `IConsumer<TEvent>` handlers from the local DI container and awaits them within the same process.

- **Source**: `src/Libraries/Nop.Services/Events/EventPublisher.cs:20`
- **Problem**: `OrderPlacedEvent` is consumed in-process only. There is no persistence, no guaranteed delivery to external systems, and no replay on failure.
- **Consequence**: Without an outbox, any omnichannel integration reacting to a placed order is either on the checkout critical path (a reliability risk) or missing entirely.

### 2. Checkout Is Fully Synchronous with No External Coordination

`OrderProcessingService.PlaceOrderAsync` performs all order and stock work inside a single synchronous flow.

- **Source**: `src/Libraries/Nop.Services/Orders/OrderProcessingService.cs:1617`
- **Problem**: No mechanism exists to hand off to warehouse or POS systems after placement without either blocking checkout or losing the handoff on failure.
- **Consequence**: Fulfillment coordination must be decoupled from the checkout path without modifying `OrderProcessingService`.

### 3. No Outbox, Inbox, or Idempotency Model

There is no existing concept of a durable outbox message, an inbox for deduplication, or an idempotency key for external integration events.

- **Consequence**: At-least-once delivery with safe duplicate handling must be added. This is the primary responsibility of the `Nop.Plugin.Misc.OmnichannelCore` plugin.

### 4. Stock and Warehouse Models Exist but Are Internal Only

`ProductService.GetTotalStockQuantityAsync` and `ProductService.AdjustInventoryAsync` model warehouse-level stock correctly.

- **Sources**: `src/Libraries/Nop.Services/Catalog/ProductService.cs:1438` and `:1699`
- **Problem**: These are internal operations. There is no model for stock changes arriving from external systems (POS, WMS) or for detecting stale updates from those systems.
- **Consequence**: The omnichannel layer needs its own stock-sync model. ADR-0007 chooses a projection-first approach: POS-originated stock is recorded in `OmniStockSyncState`, while core `ProductWarehouseInventory` remains owned by nopCommerce flows during the demo.

### 5. Plugin Architecture Is the Correct Extension Seam

Existing plugins demonstrate that nopCommerce supports independent plugin folders, controllers, services, migrations, and event consumers without modifying any core library.

- **Consequence**: The entire omnichannel integration layer can be added as `Nop.Plugin.Misc.OmnichannelCore` without touching `Nop.Services` or `Nop.Core`. This is a strength that makes the evolution selective and defensible.

---

## Assignment 2 Artefacts

- [Architecture checkpoint and target architecture](part1/architecture-checkpoint.md) — 3 ADD iterations, framework justification, migration roadmap.
- [Quality attribute scenarios](part1/quality-attribute-scenarios.md) — SEI 6-part with numeric measures.
- [Context map and C4 diagrams](part1/diagrams.md)
- [ADR set](adr/) — 10 ADRs, all Accepted.
- [Feasibility spike (experiment charter)](evidence/feasibility-spike.md)

**Key takeaway**: nopCommerce's plugin model and existing domain concepts make it a viable base for Scenario C. The missing pieces — durable events, outbox, inbox, and external system coordination — are the exact architectural problem the evolution is designed to solve.
