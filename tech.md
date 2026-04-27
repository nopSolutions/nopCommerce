# VerdeMart — Omnichannel Commerce Core

## The Story of Each Component

VerdeMart is a retail business that started with a single web storefront. As it grew, the business added a physical warehouse, a store with a point-of-sale terminal, and a relationship with a shipping carrier. Each of these is a separate system, run by different people, with different concerns. The challenge is making them work as one.

---

**nopCommerce** is the commerce core — the front door of the business. Every customer-facing interaction begins and ends here. A customer browses the catalogue, adds items to a cart, and places an order. At that moment, nopCommerce becomes the system of record for that order: it knows the customer, the items, the price, the payment, and the promised delivery. From here, nopCommerce's job is to coordinate — publishing what happened so that every other system can react, and listening for updates so the customer always sees the current state of their order. It does not fulfil the order, it does not move stock physically, and it does not handle finances. It orchestrates.

---

**ERPNext** is the financial backbone. When an order is placed in nopCommerce, ERPNext receives it and creates a Sales Order. This is the financial commitment — it links the sale to the general ledger, reserves revenue, and triggers the downstream procurement and accounting processes. When the warehouse confirms dispatch, ERPNext creates a Delivery Note and books the revenue entry. It also owns the master stock ledger: when new goods arrive from a supplier, ERPNext records the receipt and signals that more inventory is available. Its audience is the finance team and the operations manager, not the customer.

---

**OpenBoxes** is the warehouse. It receives the fulfilment task — a list of items to pick, from which shelf, in which quantity — and puts it in the hands of a warehouse operative. That person picks the items, packs them into a box, and marks the shipment as dispatched. OpenBoxes owns the physical truth of the warehouse: what is on which shelf, what has been picked, what has left the building. When it marks a shipment as dispatched, that event travels back through the system and updates the order state in nopCommerce. Its audience is the warehouse team on the floor.

---

**Open Source POS** is the physical store. When a customer walks in and buys something over the counter, the POS records that sale. This matters to the rest of the system because it consumes inventory from the same shared pool. A unit sold in-store must immediately reduce the stock visible on the web storefront, otherwise another customer online could order something that no longer exists. The POS publishes every completed sale so that nopCommerce and the warehouse stay current. Its audience is the shop assistant at the till.

---

**WireMock** simulates the shipping carrier. Once OpenBoxes hands a parcel to the courier, the parcel enters a network that VerdeMart does not control. WireMock stands in for that carrier — it receives the shipment booking, and at configurable points in time it sends tracking updates back: picked up, in transit, out for delivery, delivered. This allows the demonstration to show the full end-to-end tracking flow without a real carrier account, and crucially it allows the simulation of delays and failures to test how the commerce core behaves when external information is late or missing.

---

**RabbitMQ** is the nervous system. None of the systems above talk to each other directly. Instead, every state change is published as a message to RabbitMQ, and every system subscribes to the messages it cares about. This means that if OpenBoxes is temporarily down, the order message waits in the queue and is delivered when it recovers — the customer's checkout was not affected. If ERPNext is slow, nopCommerce does not wait for it. The entire integration layer is asynchronous and decoupled, and RabbitMQ is what makes that possible.

---

**Keycloak** is the identity layer. VerdeMart has multiple systems, each with its own login screen. Without a unified identity, a warehouse operative, a store assistant, and a finance manager each need separate accounts in each system they touch. Keycloak provides a single sign-on: one account, one login, valid everywhere. Every system delegates authentication to Keycloak and trusts the token it issues. A user's role — warehouse operative, store manager, administrator — is encoded in that token and enforced locally by each system.

---

**Meilisearch** handles product search on the storefront. nopCommerce's built-in search is a simple database query. Meilisearch is a dedicated search engine that provides typo tolerance, faceted filtering, and relevance ranking. When the product catalogue changes in nopCommerce, the search index in Meilisearch is updated. Its job is narrow and its boundary is clean: it receives catalogue updates and returns ranked product lists. It does not touch orders, inventory, or fulfilment.

> **Implementation priority: last.** Meilisearch has no role in either mandatory demonstration use case. The core integration chain — order placement, fulfilment, cross-channel stock visibility — works entirely without it. Implement only after all other systems are connected and the two mandatory flows are verified end to end.

---

## Technology Choices

| System             | Choice            |
|--------------------|-------------------|
| Commerce core      | nopCommerce       |
| Warehouse          | OpenBoxes         |
| ERP                | ERPNext           |
| POS                | Open Source POS   |
| Message broker     | RabbitMQ          |
| Shipping simulator | WireMock          |
| Identity           | Keycloak          |
| Search             | Meilisearch       |

---

## Scenario C — Omnichannel Commerce Core: Architectural Analysis

## How nopCommerce Currently Supports the Scenario

### What already works in your favour

**Multi-warehouse inventory model**
`ProductWarehouseInventory` tracks `StockQuantity` and `ReservedQuantity` per warehouse per product. `AdjustInventoryAsync` and `BookReservedInventoryAsync` in `ProductService` handle the reservation → sold lifecycle. A `StockQuantityHistory` audit table logs every adjustment with a message tag (e.g. "PlaceOrder", "Ship"), giving you an audit foundation.

**Rich order state model**
Orders carry three independent status axes: `OrderStatus`, `PaymentStatus`, and `ShippingStatus`. `OrderProcessingService` exposes explicit transition methods — `MarkOrderAsPaidAsync`, `ShipAsync`, `ReadyForPickupAsync`, `DeliverAsync`, `CancelOrderAsync` — each one publishing a typed domain event.

**Internal pub/sub event system**
`IEventPublisher` / `IConsumer<T>` provides a type-safe in-process event bus. Domain events exist for every major state change: `OrderPlacedEvent`, `OrderPaidEvent`, `ShipmentSentEvent`, `ShipmentDeliveredEvent`, `ShipmentReadyForPickupEvent`, and more. Any plugin can implement `IConsumer<T>` and react to these — this is the natural integration hook.

**Plugin extension points**
`IPaymentMethod`, `IShippingRateComputationMethod`, and `IShipmentTracker` are designed for third-party integration. Existing plugins (PayPalCommerce, Avalara) demonstrate inbound webhook controllers. The `IScheduleTask` framework supports polling loops.

**GenericAttribute as escape hatch**
Any entity can carry arbitrary key-value data via `GenericAttribute`. `Order.OrderGuid` is a stable UUID for cross-system correlation. `Shipment.TrackingNumber` accepts third-party identifiers.

---

## Architectural Seams and Pressure Points

### 1. The event system is synchronous and in-process only

**Seam:** `EventPublisher.cs`

All `IConsumer<T>` handlers execute inline, in the same request thread, with no persistence. If an ERP listener fails, the exception is swallowed (logged only). There is no retry, no dead-letter queue, no replay. An order is placed, `OrderPlacedEvent` fires, and if the fulfillment system's consumer crashes or times out, the event is simply lost.

**Conflict with scenario:** The scenario requires reliable delivery of order state to external systems (ERP, warehouse, shipping). In-process synchronous events cannot provide that guarantee.

---

### 2. Inventory is adjusted immediately, not truly reserved

**Seam:** `OrderProcessingService` → `AdjustInventoryAsync` called at order placement

When an order is placed, `StockQuantity` is decremented immediately. `ReservedQuantity` is updated, but there is no atomic hold mechanism and no enforcement at checkout: two concurrent orders for the last unit can both succeed, driving stock negative.

**Conflict with scenario:** Cross-channel stock visibility requires a true allocation model — the system showing a customer "1 in stock" must honour that across web, POS, and warehouse simultaneously. nopCommerce has no such gate.

---

### 3. The data model has no external identity or sync-state fields

**Seam:** `Order`, `Shipment`, `Warehouse` entities

None of the core entities carry:

- `ExternalOrderId` / `ExternalShipmentId` / `ExternalWarehouseCode`
- `SyncStatus` (pending / synced / error)
- `SyncDateUtc` / `LastSyncError`

The only workarounds are `GenericAttribute` (extra join per lookup) or `CustomValuesXml` (string parsing). Every integration must bolt on its own mapping layer and sync-failure tracking sits entirely outside the core model.

**Conflict with scenario:** When the ERP reports an order as fulfilled, nopCommerce has nowhere native to record "this order was acknowledged by ERP at T." The cross-channel order-state visibility flow has no first-class home.

---

### 4. The shipment state model is too thin for fulfillment workflows

**Seam:** `Shipment.cs`

Shipment status is encoded as three nullable date fields: `ShippedDateUtc`, `ReadyForPickupDateUtc`, `DeliveryDateUtc`. There are no intermediate states — picked, packed, dispatched, in-transit, exception, returned. There is no `ExternalShipmentId` for warehouse or 3PL reference.

**Conflict with scenario:** The buy-online / fulfill-through-another-channel flow requires tracking a shipment as it moves through warehouse pick → pack → carrier handoff → delivery. None of these steps are representable in the current schema without schema changes.

---

### 5. Scheduled tasks are single-instance with no distributed coordination

**Seam:** `IScheduleTask`, `ScheduleTaskRunner`

The task framework runs within a single app process with no distributed locking and no per-item progress tracking. If a sync task processes 50 orders and crashes on the 51st, it restarts from scratch on the next execution. There is no outbox pattern, no at-least-once delivery guarantee.

**Conflict with scenario:** Any polling task that syncs orders to ERP or pulls inventory from warehouse systems cannot be made reliably idempotent without adding that logic entirely from scratch.

---

### 6. The order-placement mutex is process-local

**Seam:** `OrderProcessingService.PlaceOrderAsync`

The duplicate-order guard uses a `Mutex` — which is per-process. In a horizontally-scaled deployment (load-balanced web nodes), two nodes can each accept an order for the same customer simultaneously and both succeed.

**Conflict with scenario:** An omnichannel core accepting orders from web and POS simultaneously must use a distributed lock (e.g. Redis) or an idempotency key at the database level. The current implementation provides false safety in multi-node deployments.

---

### 7. No webhook ingestion or outbound integration pattern in the framework

**Seam:** `BasePlugin`, `IShippingRateComputationMethod`

The plugin base provides no scaffold for outbound HTTP with retry, inbound webhook verification, message queue consumers, or polling loops. Each integration plugin must implement its own `HttpClient`, retry policy, and webhook controller from scratch (as PayPalCommerce does). There is no shared resilience layer.

**Conflict with scenario:** Every external system integration (ERP, warehouse, POS, carrier) becomes a bespoke HTTP client with no shared circuit-breaker, backoff, or dead-letter handling. The failure surface grows with each new channel added.

---

## Summary

| Scenario Requirement                                    | nopCommerce Support                                   | Gap Severity |
|---------------------------------------------------------|-------------------------------------------------------|--------------|
| Order placed → reflected in ERP/warehouse               | `OrderPlacedEvent` exists                             | High — event is in-process, no guaranteed delivery |
| Cross-channel stock visibility                          | Per-warehouse `StockQuantity` exists                  | High — no atomic allocation, no external sync state |
| System useful when external systems lag or degrade      | Plugin can implement fallback logic                   | High — no framework support for circuit breaker or stale-data mode |
| Fulfillment state back-propagation to commerce          | `ShipmentSentEvent`, `DeliveryDateUtc` exist          | Medium — shipment states too coarse, no external ID field |
| Distributed multi-node safety                           | Single `Mutex`                                        | High — process-local only |
| Sync recovery after external system outage              | `ScheduleTask` framework exists                       | Medium — no per-item progress, no outbox |

nopCommerce provides the domain model and the event vocabulary, but the delivery guarantees, external identity fields, and resilience patterns required for a true omnichannel core must all be built on top of it. The `IConsumer<T>` + `IScheduleTask` pair is where the integration layer will live.

---

## Domain and Boundary Model

### Relevant Subdomains

**Core subdomains** — these are VerdeMart's competitive differentiators; mistakes here directly hurt the customer.

| Subdomain            | Why it is core                                                              |
|----------------------|-----------------------------------------------------------------------------|
| Order Management     | The central transaction of the business; all channels converge here         |
| Inventory Allocation | Cross-channel overselling is the most visible failure mode                  |
| Fulfillment Execution| The physical act of getting goods to the customer is where promises are kept|

**Supporting subdomains** — necessary, but not differentiating; can use off-the-shelf solutions.

| Subdomain            | Why it is supporting                                      |
|----------------------|-----------------------------------------------------------|
| Product Catalog      | Standard CRUD; value is in the data, not the process      |
| Payment Processing   | Delegated entirely to payment providers                   |
| Shipping / Carrier   | Rate-and-track; carrier owns the logic                    |
| Customer Management  | Profile and loyalty; important but not the integration risk|

**Generic subdomains** — commodity; buy or configure, do not build.

| Subdomain            | System      |
|----------------------|-------------|
| Identity & Access    | Keycloak    |
| Product Search       | Meilisearch |
| Notifications        | nopCommerce built-in (queued email) |

---

### Bounded Contexts

#### 1. Commerce Context — nopCommerce
The system of record for the customer-facing order lifecycle.

**Owns:** cart, checkout, order state machine (`Pending → Processing → Complete / Cancelled`), customer-visible pricing, shipment tracking display, product catalog (commerce view).

**Publishes to RabbitMQ:**
- `order.placed` — after `PlaceOrderAsync` succeeds
- `order.paid` — after `MarkOrderAsPaidAsync`
- `order.cancelled` — after `CancelOrderAsync`
- `shipment.requested` — when admin creates a shipment record

**Consumes from RabbitMQ:**
- `fulfillment.confirmed` — updates `ShippingStatus` to Shipped
- `shipment.status.updated` — updates tracking display
- `inventory.adjusted` — reflects external stock changes back to catalog

---

#### 2. Warehouse Context — OpenBoxes
The system of record for physical inventory movement and fulfillment execution.

**Owns:** warehouse locations, physical stock quantities, pick/pack/ship workflow, staff task queues.

**Publishes to RabbitMQ:**
- `shipment.picked`
- `shipment.packed`
- `shipment.dispatched` — triggers `fulfillment.confirmed` chain back to Commerce
- `inventory.adjusted` — when a stock count correction happens in the warehouse

**Consumes from RabbitMQ:**
- `order.placed` — creates a fulfillment order in OpenBoxes

---

#### 3. ERP Context — ERPNext
The financial and procurement system of record.

**Owns:** sales order ledger, purchase orders, supplier management, accounting entries, authoritative financial stock valuation.

**Publishes to RabbitMQ:**
- `erp.sales_order.confirmed` — acknowledgement back to Commerce
- `erp.stock.updated` — when a purchase order receipt updates the stock ledger

**Consumes from RabbitMQ:**
- `order.placed` — creates a Sales Order in ERPNext
- `shipment.dispatched` — creates a Delivery Note and revenue entry

---

#### 4. POS Context — Open Source POS
The in-store sales channel.

**Owns:** in-store transactions, cash register sessions, in-store customer interactions.

**Publishes to RabbitMQ:**
- `pos.sale.completed` — triggers inventory adjustment visible to Commerce and Warehouse

**Consumes from RabbitMQ:**
- `inventory.adjusted` — keeps in-store stock display current
- `product.catalog.updated` — keeps local product list in sync

---

#### 5. Shipping Context — WireMock (carrier simulator)
Simulates an external carrier API for the demonstration.

**Publishes (HTTP webhook to nopCommerce):**
- `shipment.status.updated` — with status, location, and timestamp

**Consumes (HTTP POST from nopCommerce plugin):**
- Shipment booking request after `shipment.dispatched`

---

#### 6. Identity Context — Keycloak
Federated identity provider for all systems.

**Owns:** user identity, roles, tokens, SSO sessions.
**Serves:** nopCommerce (OIDC plugin), ERPNext (built-in OAuth2 config), POS (OIDC if web-based).
**Does not publish domain events** — stateless token issuer only.

---

### Ownership of Major Responsibilities

| Responsibility                          | Owner          | Notes                                                                 |
|-----------------------------------------|----------------|-----------------------------------------------------------------------|
| Order state machine                     | nopCommerce    | Single source of truth for order status visible to the customer       |
| Physical stock quantity                 | OpenBoxes      | Authoritative for what is physically available in the warehouse       |
| Financial stock valuation               | ERPNext        | Authoritative for accounting; may lag physical by minutes             |
| Commerce-visible stock level            | nopCommerce    | Derived from OpenBoxes via `inventory.adjusted` events                |
| Shipment tracking events                | Carrier (WireMock) | nopCommerce displays; carrier owns                                |
| In-store sales transactions             | POS            | Feeds back into inventory via RabbitMQ                                |
| User identity and roles                 | Keycloak       | All other systems are relying parties                                 |
| Product search index                    | Meilisearch    | Fed by nopCommerce product catalog changes                            |

---

### Key Ownership Tensions

**Stock quantity has two owners.**
OpenBoxes owns physical stock; ERPNext owns financial stock. nopCommerce must pick one as its authoritative source for the "available to purchase" figure. The correct choice is **OpenBoxes** — it reflects what can actually be picked. ERPNext may lag by a purchase order receipt cycle.

**Order state exists in two systems simultaneously.**
Every `order.placed` event creates both a nopCommerce Order and an ERPNext Sales Order. These are siblings, not copies — nopCommerce owns the customer-facing lifecycle, ERPNext owns the financial lifecycle. Synchronisation is one-directional on placement (`nopCommerce → ERPNext`) and one-directional on confirmation (`ERPNext → nopCommerce`).

**Customer identity is split across systems.**
Keycloak owns authentication identity. nopCommerce, ERPNext, and POS each hold a local customer/contact record linked by the Keycloak subject ID (`sub` claim). Profile changes in one system do not automatically propagate — this is a documented limitation for the demo scope.

---

## Quality Attribute Scenarios

Each scenario is written as: **Stimulus → Source → Environment → Artifact → Response → Response Measure**

---

### QAS-1 — Reliability: Order event must survive a warehouse outage

| Field            | Value |
|------------------|-------|
| Quality attribute | Reliability |
| Stimulus         | A customer places an order on the web storefront |
| Source           | nopCommerce checkout |
| Environment      | OpenBoxes is temporarily unavailable (process down or network partition) |
| Artifact         | The `order.placed` message published to RabbitMQ |
| Response         | The message is durably queued; OpenBoxes consumes and processes it when it recovers; no manual intervention is required |
| Response measure | Zero orders lost during an OpenBoxes outage of up to 30 minutes; order appears in OpenBoxes within 60 seconds of recovery |

**Design decision forced:** The nopCommerce → OpenBoxes integration must use a durable message queue (RabbitMQ with persistent queues and acknowledgements), not a direct HTTP call. Direct HTTP fails silently or blocks the checkout thread.

---

### QAS-2 — Consistency: Last-unit oversell must not occur across channels

| Field            | Value |
|------------------|-------|
| Quality attribute | Consistency |
| Stimulus         | Two orders for the last unit of a product are placed simultaneously — one from the web storefront, one from the POS |
| Source           | nopCommerce web checkout and POS sale event |
| Environment      | Normal operation, both systems active |
| Artifact         | `StockQuantity` in nopCommerce and the fulfillment queue in OpenBoxes |
| Response         | Exactly one order succeeds; the other receives a stock-unavailable response; inventory never goes below zero |
| Response measure | Zero confirmed oversell events under concurrent load; the losing order is rejected within the same request cycle |

**Design decision forced:** Inventory decrement at checkout must be protected by a serializable database transaction or a per-product lock. The current `AdjustInventoryAsync` path has no such gate — it must be added.

---

### QAS-3 — Availability: Checkout must complete when ERPNext is slow

| Field            | Value |
|------------------|-------|
| Quality attribute | Availability |
| Stimulus         | A customer completes checkout while ERPNext is responding slowly (>5 s per request) |
| Source           | ERPNext under load or network degradation |
| Environment      | nopCommerce is processing the order; ERPNext sync is part of the post-placement flow |
| Artifact         | nopCommerce order placement and the customer-facing confirmation page |
| Response         | The order is confirmed to the customer immediately; ERPNext synchronisation happens asynchronously via RabbitMQ; the customer never waits on ERPNext |
| Response measure | Checkout response time remains under 3 seconds regardless of ERPNext latency; no checkout failures attributable to ERPNext slowness |

**Design decision forced:** ERPNext must never be in the synchronous checkout path. The `IConsumer<OrderPlacedEvent>` that notifies ERPNext must publish to RabbitMQ and return immediately. Any blocking HTTP call to ERPNext during checkout is an architectural violation.

---

### QAS-4 — Recoverability: Missed events must self-heal after outage

| Field            | Value |
|------------------|-------|
| Quality attribute | Recoverability |
| Stimulus         | OpenBoxes recovers after a 30-minute outage during which 12 orders were placed |
| Source           | OpenBoxes process restart |
| Environment      | RabbitMQ was running throughout the outage; messages were durably queued |
| Artifact         | The fulfillment order queue in RabbitMQ and the order records in OpenBoxes |
| Response         | OpenBoxes consumes the 12 queued `order.placed` messages in order; all fulfillment tasks are created; nopCommerce order state is updated accordingly |
| Response measure | All orders placed during the outage are fully processed within 5 minutes of OpenBoxes recovery; no operator action required |

**Design decision forced:** RabbitMQ queues must be declared as durable with manual acknowledgement. OpenBoxes consumption must be idempotent — replaying a message for an already-created fulfillment order must not create a duplicate.

---

### QAS-5 — Visibility: Carrier tracking update must reach the customer quickly

| Field            | Value |
|------------------|-------|
| Quality attribute | Visibility (cross-channel state propagation) |
| Stimulus         | WireMock (carrier) sends a `shipment.status.updated` webhook — status changes to "In Transit" with a tracking location |
| Source           | Carrier system (WireMock) |
| Environment      | Normal operation; shipment has been dispatched from OpenBoxes |
| Artifact         | The order detail page in nopCommerce and the customer notification email |
| Response         | nopCommerce receives and processes the webhook; the tracking status is updated in the order record; a notification email is queued |
| Response measure | Order tracking status visible to the customer within 10 seconds of the webhook being received; email queued within the same window |

**Design decision forced:** nopCommerce must expose a webhook endpoint that accepts carrier callbacks, verifies the payload, and maps the external shipment status to the internal `ShippingStatus`. The `Shipment` entity needs an `ExternalShipmentId` field to correlate the incoming webhook to the correct order.

---

## Chosen Framework

### Selected method: ADD (Attribute-Driven Design)

ADD was chosen over ACDM and ADM for this project.

---

### What each method offers

**ADD** is an iterative decomposition method driven entirely by quality attributes. You start with the system as a whole, identify the architectural drivers — quality attribute scenarios, constraints, and concerns — and decompose the system into components that satisfy them. Each iteration selects the most critical driver, chooses an architectural tactic to address it, and instantiates the components and connectors that realise that tactic. The output at each step is a set of architectural decisions directly traceable to a quality requirement.

**ACDM (Architecture-Centric Design Method)** is a more process-heavy method with formal phases: elicit requirements, establish an architecture baseline, evaluate, refine, and produce documentation artefacts at each gate. It is designed for larger teams with formal handoffs between roles — architect, developer, reviewer — and produces a richer paper trail. The process itself is the primary product.

**ADM (TOGAF Architecture Development Method)** is an enterprise-wide framework covering business architecture, information systems architecture, technology architecture, and migration planning across an entire organisation. It answers the question of how a whole enterprise should evolve its systems over years, not how a specific integration should be designed this semester.

---

### Why ADD fits this case

The work done in this document maps directly onto ADD's inputs and outputs:

| ADD concept | What it corresponds to in this project |
|---|---|
| Architectural drivers | The 7 pressure points identified in the nopCommerce analysis |
| Quality attribute scenarios | QAS-1 through QAS-5 |
| Decomposition elements | The 6 bounded contexts (Commerce, Warehouse, ERP, POS, Shipping, Identity) |
| Architectural tactics | Durable queues, serializable transactions, async integration, webhook ingestion, external ID fields |
| Constraints | nopCommerce as the fixed commerce core; RabbitMQ as the broker; demo scope for Keycloak and Meilisearch |

ADD is appropriate here for three specific reasons.

First, the scenario is quality-attribute-driven. The three strategic goals — reliable commerce core, cross-channel visibility, and resilience to degraded external systems — are not functional requirements. They are quality attributes. ADD is the only one of the three methods that treats quality attributes as the primary input to every design decision, not as a secondary concern after structure is established.

Second, the scope is bounded. VerdeMart is integrating a defined set of systems for a defined set of use cases. ADM is designed for enterprise-wide transformation programmes spanning years. ACDM introduces process overhead — formal gates, documentation artefacts, role separation — that adds no value when one or two people are designing and building the same system. ADD is lean enough to match the project scale.

Third, ADD is traceable by construction. Every architectural decision in ADD exists because a quality attribute scenario demanded it. This traceability — QAS-2 demands serializable inventory decrement, QAS-3 demands async ERPNext integration, QAS-1 demands durable queues — makes the reasoning behind each design choice explicit and defensible, which is exactly what a demonstration review requires.
