# ADD Iteration 1 — Step 4: Instantiate Elements and Allocate Responsibilities

## Plugin Identity

| Property | Value |
|---|---|
| Plugin name | `Nop.Plugin.Messaging.RabbitMq` |
| Location | `src/Plugins/Nop.Plugin.Messaging.RabbitMq/` |
| NuGet dependency | `RabbitMQ.Client` |

---

## Components

| Component | Type | Responsibility |
|---|---|---|
| `RabbitMqPlugin` | `BasePlugin` | Plugin entry point; declares exchange and queue on install; does not tear down topology on uninstall |
| `RabbitMqConnectionFactory` | Singleton service | Holds the single AMQP connection; exposes short-lived channels to callers; disposes cleanly on shutdown |
| `OrderPlacedConsumer` | `IConsumer<OrderPlacedEvent>` | Reacts to `OrderPlacedEvent`; builds `OrderPlacedMessage`; publishes to `verdemart.orders` with key `order.placed` and delivery mode 2 |
| `OrderPlacedMessage` | JSON DTO | Immutable wire payload carrying `OrderId`, `OrderGuid`, `CustomerId`, `OrderTotal`, `CreatedOnUtc`, and line items; `OrderGuid` is the downstream idempotency key |
| `RabbitMqSettings` | `ISettings` | Persisted settings for host, port, credentials, exchange name, and queue name; editable from the admin panel without redeployment |
| `PluginNopStartup` | `INopStartup` | Registers `IRabbitMqConnectionFactory` (singleton) and `OrderPlacedConsumer` (scoped) |

---

## Component Relationships

```
nopCommerce event system
        │ fires OrderPlacedEvent
        ▼
OrderPlacedConsumer
        │ builds payload → OrderPlacedMessage
        │ reads → RabbitMqSettings
        ▼
RabbitMqConnectionFactory
        │ CreateChannel()
        ▼
RabbitMQ exchange: verdemart.orders
        │ routing key: order.placed
        ▼
Queue: verdemart.orders.openboxes
```

---

## What Step 5 Will Do

Step 5 defines the interfaces between these components and the RabbitMQ topology contract.
