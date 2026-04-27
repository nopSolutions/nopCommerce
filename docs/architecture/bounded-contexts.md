# Bounded Contexts and Domain Model

**Owner: Duarte**  
**Scenario C — Omnichannel Commerce Core (VerdeMart Retail)**

---

## Identified Bounded Contexts

_TODO (Duarte): Flesh out each context with subdomain classification (core/supporting/generic), key entities, and data ownership._

### 1. Order Management (nopCommerce — Core Domain)
- **Owns**: Orders, order items, order status, payment status, shipping status
- **Key entities**: `Order`, `OrderItem`, `OrderNote`
- **Upstream from**: Inventory/Stock (reads stock before placing), Fulfillment (pushes order events)
- **Data store**: nopCommerce PostgreSQL

### 2. Catalog & Pricing (nopCommerce — Core Domain)
- **Owns**: Products, categories, prices, discounts, attributes
- **Key entities**: `Product`, `Category`, `ProductWarehouseInventory`, `StockQuantityHistory`
- **Stock quantities live here** but are updated by events from the WMS (cross-channel stock corrections)
- **Data store**: nopCommerce PostgreSQL

### 3. Fulfillment Coordination (Order Integration Service — Supporting)
- **Owns**: The coordination protocol between a placed order and external operational systems
- **Stateless** — does not persist orders; reads from RabbitMQ, forwards to ERP/WMS
- **Key responsibility**: reliability (retry, circuit breaker, dead-letter, reconciliation)
- **Data store**: None (RabbitMQ dead-letter queue acts as transient state)

### 4. ERP / Back-Office (ERP Stub — External / Generic)
- **Owns**: Confirmed order records for accounting and invoicing
- **Upstream from**: Fulfillment Coordination (receives order events)
- **Data store**: ERP stub in-memory (not shared with nopCommerce)

### 5. Warehouse / Inventory (WMS Stub — External / Supporting)
- **Owns**: Physical stock quantities, warehouse reservations
- **Publishes** `stock.updated` events that drive corrections in nopCommerce Catalog context
- **Data store**: WMS stub in-memory (not shared with nopCommerce)

---

## Context Map

_TODO (Duarte): Draw or describe the relationships between contexts._

```
[Order Management] ──(OrderPlacedEvent via outbox)──▶ [Fulfillment Coordination]
                                                              │
                                        ┌─────────────────────┤
                                        ▼                     ▼
                               [ERP/Back-Office]    [Warehouse/Inventory]
                                                             │
                               [Catalog & Pricing] ◀─(stock.updated)─┘
```

Relationships:
- Order Management → Fulfillment Coordination: **Upstream/Downstream** (nopCommerce is upstream; Integration Service consumes its events)
- Fulfillment Coordination → ERP: **Customer/Supplier** (Integration Service calls ERP adapter)
- Fulfillment Coordination → Warehouse: **Customer/Supplier** with **Anti-Corruption Layer** (circuit breaker protects core from WMS failures)
- Warehouse → Catalog & Pricing: **Published Language** (WMS publishes `stock.updated` events with a well-defined schema)

---

## Data Ownership Rules

| Data | Authoritative Owner | How other contexts access it |
|------|---------------------|------------------------------|
| Order state | nopCommerce (Order Management) | Read-only via nopCommerce API; never written by external systems directly |
| Product stock quantity | nopCommerce (Catalog) | Written by `StockUpdateConsumerBackgroundService` on `stock.updated` event |
| Warehouse reservation | WMS Stub | Never read by nopCommerce directly |
| ERP order record | ERP Stub | Never read by nopCommerce directly |

**No shared database across extracted boundaries.** Each bounded context has its own data store. Cross-context communication is exclusively via events or explicit HTTP calls with well-defined contracts.

---

## Evolution Roadmap

_TODO (Duarte): Describe the sequence of steps from current monolith to target state._

### Phase 0 — Baseline (current)
nopCommerce operates as an isolated monolith. No external system integrations. Orders placed → DB only. Stock managed locally.

### Phase 1 — Outbox & Message Backbone
- Add `IntegrationEvent` outbox table to nopCommerce DB
- Add background publisher → RabbitMQ
- Deploy RabbitMQ alongside nopCommerce (Docker Compose)
- **What coexists**: nopCommerce still fully functional without Integration Service running

### Phase 2 — Fulfillment Coordination
- Deploy Order Integration Service
- Connect to RabbitMQ consumer
- ERP and WMS stubs deployed
- Happy path: order placed → ERP + WMS notified → stock updated back

### Phase 3 — Resilience & Pressure Point
- Add circuit breaker (WMS adapter)
- Add dead-letter queue handling
- Add reconciliation loop
- Add observability dashboard

### Transition Constraints
- During Phase 1–2, nopCommerce must remain fully operational for web customers even if Integration Service or stubs are not running
- The outbox acts as a buffer: if Integration Service is down, events accumulate and are processed when it starts
