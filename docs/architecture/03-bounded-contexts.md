# Domain and Boundary Model

## Relevant Subdomains

### Core Subdomains - VerdeMart's competitive differentiators

| Subdomain | Why it is core |
|---|---|
| Order Management | The central transaction of the business; all channels converge here |
| Inventory Allocation | Cross-channel overselling is the most visible failure mode |
| Fulfillment Execution | The physical act of getting goods to the customer is where promises are kept |

### Supporting Subdomains - necessary, but not differentiating

| Subdomain | Why it is supporting |
|---|---|
| Product Catalog | Standard CRUD; value is in the data, not the process |
| Payment Processing | Delegated entirely to payment providers |
| Shipping / Carrier | Rate-and-track; carrier owns the logic |
| Customer Management | Profile and loyalty; important but not the integration risk |

### Generic Subdomains - commodity; buy or configure, do not build

| Subdomain | System |
|---|---|
| Identity & Access | Keycloak |
| Product Search | Meilisearch |
| Notifications | nopCommerce built-in (queued email) |

---

## Bounded Contexts

### 1. Commerce Context - nopCommerce

The system of record for the customer-facing order lifecycle.

**Owns:** cart, checkout, order state machine (`Pending → Processing → Complete / Cancelled`), customer-visible pricing, shipment tracking display, product catalog (commerce view).

**Publishes to RabbitMQ:**
- `order.placed` - after `PlaceOrderAsync` succeeds
- `order.paid` - after `MarkOrderAsPaidAsync`
- `order.cancelled` - after `CancelOrderAsync`
- `shipment.requested` - when admin creates a shipment record

**Consumes from RabbitMQ:**
- `fulfillment.confirmed` - updates `ShippingStatus` to Shipped
- `shipment.status.updated` - updates tracking display
- `inventory.adjusted` - reflects external stock changes back to catalog

---

### 2. Warehouse Context - OpenBoxes

The system of record for physical inventory movement and fulfillment execution.

**Owns:** warehouse locations, physical stock quantities, pick/pack/ship workflow, staff task queues.

**Publishes to RabbitMQ:**
- `shipment.picked`
- `shipment.packed`
- `shipment.dispatched` - triggers `fulfillment.confirmed` chain back to Commerce
- `inventory.adjusted` - when a stock count correction happens in the warehouse

**Consumes from RabbitMQ:**
- `order.placed` - creates a fulfillment order in OpenBoxes

---

### 3. ERP Context - ERPNext

The financial and procurement system of record.

**Owns:** sales order ledger, purchase orders, supplier management, accounting entries, authoritative financial stock valuation.

**Publishes to RabbitMQ:**
- `erp.sales_order.confirmed` - acknowledgement back to Commerce
- `erp.stock.updated` - when a purchase order receipt updates the stock ledger

**Consumes from RabbitMQ:**
- `order.placed` - creates a Sales Order in ERPNext
- `shipment.dispatched` - creates a Delivery Note and revenue entry

---

### 4. POS Context - Open Source POS

The in-store sales channel.

**Owns:** in-store transactions, cash register sessions, in-store customer interactions.

**Publishes to RabbitMQ:**
- `pos.sale.completed` - triggers inventory adjustment visible to Commerce and Warehouse

**Consumes from RabbitMQ:**
- `inventory.adjusted` - keeps in-store stock display current
- `product.catalog.updated` - keeps local product list in sync

---

### 5. Shipping Context - WireMock (carrier simulator)

Simulates an external carrier API for the demonstration.

**Publishes (HTTP webhook to nopCommerce):**
- `shipment.status.updated` - with status, location, and timestamp

**Consumes (HTTP POST from nopCommerce plugin):**
- Shipment booking request after `shipment.dispatched`

---

### 6. Identity Context - Keycloak

Federated identity provider for all systems.

**Owns:** user identity, roles, tokens, SSO sessions.
**Does not publish domain events** - stateless token issuer only.

---

## Ownership of Major Responsibilities

| Responsibility | Owner | Notes |
|---|---|---|
| Order state machine | nopCommerce | Single source of truth for order status visible to the customer |
| Physical stock quantity | OpenBoxes | Authoritative for what is physically available in the warehouse |
| Financial stock valuation | ERPNext | Authoritative for accounting; may lag physical by minutes |
| Commerce-visible stock level | nopCommerce | Derived from OpenBoxes via `inventory.adjusted` events |
| Shipment tracking events | Carrier (WireMock) | nopCommerce displays; carrier owns |
| In-store sales transactions | POS | Feeds back into inventory via RabbitMQ |
| User identity and roles | Keycloak | All other systems are relying parties |

---

## Key Ownership Tensions

**Stock quantity has two owners.**
OpenBoxes owns physical stock; ERPNext owns financial stock. nopCommerce must pick one as its authoritative source for the "available to purchase" figure. The correct choice is **OpenBoxes** - it reflects what can actually be picked. ERPNext may lag by a purchase order receipt cycle.

**Order state exists in two systems simultaneously.**
Every `order.placed` event creates both a nopCommerce Order and an ERPNext Sales Order. These are siblings, not copies - nopCommerce owns the customer-facing lifecycle, ERPNext owns the financial lifecycle. Synchronisation is one-directional on placement (`nopCommerce → ERPNext`) and one-directional on confirmation (`ERPNext → nopCommerce`).

**Customer identity is split across systems.**
Keycloak owns authentication identity. nopCommerce, ERPNext, and POS each hold a local customer/contact record linked by the Keycloak subject ID (`sub` claim). Profile changes in one system do not automatically propagate - this is a documented limitation for the demo scope.
