# Part 1 Diagrams

Compact diagrams for the checkpoint. Mermaid format so any Markdown viewer renders them.

## DDD Context Map (bounded contexts + relationships)

```mermaid
flowchart LR
    subgraph Core["nopCommerce Core Context"]
        CC[Order, Product, Stock, Shipment]
    end

    subgraph Omni["Omnichannel Integration Context"]
        OI[Outbox, Inbox, Projection]
    end

    subgraph WMSCtx["WMS Context (external)"]
        WC[Fulfillment requests]
    end

    subgraph POSCtx["POS Context (external)"]
        PC[Stock changes]
    end

    Core -->|upstream: domain event| Omni
    Omni -->|downstream: callback API| Core
    Omni -->|downstream: ACL via worker| WMSCtx
    POSCtx -->|upstream: ACL via worker| Omni
```

- **Upstream** (provider) → **downstream** (consumer) is read in the direction of the arrow.
- The Worker acts as an **anti-corruption layer (ACL)** on the WMS and POS edges: external schemas and quirks are translated into the omnichannel envelope before the plugin sees them.
- nopCommerce Core is upstream of Omnichannel for the order event, but downstream of Omnichannel for fulfillment/stock callbacks — the two contexts have a **customer/supplier** relationship via versioned contracts, not a shared model.

## C4 Level 1 - System Context

```mermaid
flowchart LR
    Customer[Customer]
    Staff[Store/Warehouse Staff]
    Nop[nopCommerce Commerce Core]
    POS[POS Simulator]
    WMS[WMS Simulator]
    MQ[(RabbitMQ)]
    Worker[Omnichannel Worker]

    Customer -->|browse, checkout, order status| Nop
    Staff -->|store sale / stock movement| POS
    POS -->|stock events| MQ
    Nop -->|order placed events| MQ
    MQ --> Worker
    Worker -->|fulfillment request| WMS
    Worker -->|fulfillment and stock updates| Nop
```

## C4 Level 2 - Containers

Owner labels in `[brackets]` show team responsibility (see ADR-0005 for boundary rules).

```mermaid
flowchart TB
    subgraph Core["nopCommerce container [nopCommerce team]"]
        Web["Nop.Web storefront/admin<br>[nopCommerce]"]
        Services["Nop.Services orders/catalog/shipping<br>[nopCommerce]"]
        Plugin["Omnichannel Core plugin<br>[Omnichannel]"]
        NopDb[("nopCommerce DB<br>[nopCommerce]")]
        Web --> Services
        Services --> NopDb
        Services -->|OrderPlacedEvent| Plugin
        Plugin --> NopDb
    end

    MQ[("RabbitMQ<br>[Omnichannel]")]
    Worker["Omnichannel Worker<br>[Omnichannel]"]
    WMS["WMS Simulator<br>[external]"]
    POS["POS Simulator<br>[external]"]

    Plugin -->|publish outbox| MQ
    POS -->|stock changed| MQ
    MQ --> Worker
    Worker -->|HTTP fulfillment request| WMS
    Worker -->|HTTP internal callback| Plugin
```

## C4 Level 3 - Plugin Components

```mermaid
flowchart LR
    OrderEvent[OrderPlacedEvent Consumer]
    Outbox[(OmniOutboxMessage)]
    PublishTask[Outbox Publish Scheduled Task]
    Api[Internal Update API]
    Inbox[(OmniInboxMessage)]
    Fulfillment[(OmniOrderFulfillment)]
    Stock[(OmniStockSyncState)]

    OrderEvent -->|create pending message| Outbox
    PublishTask -->|read pending| Outbox
    PublishTask -->|publish confirmed| Rabbit[(RabbitMQ)]
    Api -->|idempotency check| Inbox
    Api --> Fulfillment
    Api --> Stock
```

## Runtime - Normal Fulfillment Flow

```mermaid
sequenceDiagram
    participant C as Customer
    participant N as nopCommerce
    participant P as Omnichannel Plugin
    participant R as RabbitMQ
    participant W as Worker
    participant S as WMS Simulator

    C->>N: Place order
    N->>N: Persist order and order items
    N->>P: OrderPlacedEvent
    P->>P: Store OmniOutboxMessage
    N-->>C: Order confirmation
    P->>R: Publish commerce.order.placed.v1
    R->>W: Deliver order event
    W->>S: Create fulfillment request
    S-->>W: Accepted + externalRequestId
    W->>P: fulfillment.status.changed.v1
    P->>P: Upsert fulfillment projection
```

## Runtime - WMS Unavailable

```mermaid
sequenceDiagram
    participant C as Customer
    participant N as nopCommerce
    participant P as Omnichannel Plugin
    participant R as RabbitMQ
    participant W as Worker
    participant S as WMS Simulator

    C->>N: Place order
    N->>P: OrderPlacedEvent
    P->>P: Store pending outbox message
    N-->>C: Order confirmation still succeeds
    P->>R: Publish order event
    R->>W: Deliver order event
    W->>S: Create fulfillment request
    S--xW: Unavailable
    W->>W: Retry with backoff / circuit breaker
    W->>P: Mark fulfillment pending/degraded
    S-->>W: Recovers later
    W->>S: Retry fulfillment request
    S-->>W: Accepted
    W->>P: Mark fulfillment accepted
```

## Runtime - POS Stock Update

```mermaid
sequenceDiagram
    participant POS as POS Simulator
    participant R as RabbitMQ
    participant W as Worker
    participant P as Omnichannel Plugin

    POS->>R: pos.stock.changed.v1
    R->>W: Deliver stock event
    W->>P: Submit stock sync update
    P->>P: Check messageId and sourceVersion
    P->>P: Upsert OmniStockSyncState projection
    P-->>W: Accepted or ignored as duplicate/stale
```

Per ADR-0007, this flow is projection-first. POS updates do not directly write through to nopCommerce core `ProductWarehouseInventory` during the demo; any drift is made visible instead of being silently merged.
