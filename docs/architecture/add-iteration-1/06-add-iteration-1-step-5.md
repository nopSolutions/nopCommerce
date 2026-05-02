# ADD Iteration 1 — Step 5: Define Interfaces

## Key Interfaces

**`IRabbitMqConnectionFactory`** — exposes `CreateChannel()`; returns a new short-lived channel from the shared connection; caller is responsible for disposal; connection opens lazily on first call.

**`IConsumer<OrderPlacedEvent>`** (nopCommerce core) — `HandleEventAsync` is the single entry point fired after every successful order placement; must not block the checkout thread; must not swallow exceptions.

**`OrderPlacedMessage`** — immutable JSON record; fields: `OrderId`, `OrderGuid`, `CustomerId`, `OrderTotal`, `CreatedOnUtc`, `Items`; `OrderGuid` is the idempotency key for all consumers.

**`RabbitMqSettings`** (`ISettings`) — configurable without redeployment: `Host`, `Port`, `Username`, `Password`, `ExchangeName`, `OrderPlacedQueueName`.

---

## RabbitMQ Topology Contract

| Element | Name | Properties |
|---|---|---|
| Exchange | `verdemart.orders` | direct, durable, no auto-delete |
| Queue | `verdemart.orders.openboxes` | durable, manual ack |
| Binding | queue → exchange | routing key: `order.placed` |
| Message | `order.placed` | delivery mode 2 (persistent), `application/json` |

---

## Dependency Map

```
PluginNopStartup
    registers → IRabbitMqConnectionFactory (singleton)
    registers → IConsumer<OrderPlacedEvent> (scoped)

OrderPlacedConsumer
    depends on → IRabbitMqConnectionFactory
    depends on → RabbitMqSettings
    publishes  → exchange: verdemart.orders / key: order.placed

RabbitMqConnectionFactory
    depends on → RabbitMqSettings
```

---

## What Step 6 Will Do

Step 6 sketches the component view and records the architectural decisions this iteration produced.
