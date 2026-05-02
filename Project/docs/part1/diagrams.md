# Part 1 Diagrams

These diagrams are intentionally compact for the 7-minute checkpoint. They are written in Mermaid so they can be rendered by common Markdown viewers.

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

```mermaid
flowchart TB
    subgraph Core["nopCommerce container"]
        Web[Nop.Web storefront/admin]
        Services[Nop.Services orders/catalog/shipping]
        Plugin[Omnichannel Core plugin]
        NopDb[(nopCommerce DB)]
        Web --> Services
        Services --> NopDb
        Services -->|OrderPlacedEvent| Plugin
        Plugin --> NopDb
    end

    MQ[(RabbitMQ)]
    Worker[Omnichannel Worker]
    WMS[WMS Simulator]
    POS[POS Simulator]

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
    participant N as nopCommerce Catalog

    POS->>R: pos.stock.changed.v1
    R->>W: Deliver stock event
    W->>P: Submit stock sync update
    P->>P: Check messageId and sourceVersion
    P->>N: Adjust stock/projection
    P-->>W: Accepted or ignored as stale
```

