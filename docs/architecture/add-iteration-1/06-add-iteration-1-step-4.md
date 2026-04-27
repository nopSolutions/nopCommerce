# ADD Iteration 1 — Step 4: Instantiate Elements and Allocate Responsibilities

## What This Step Does

Step 4 turns the design concepts from Step 3 into concrete named components. Each component gets a name, a type, a location in the codebase, and a defined set of responsibilities.

---

## Plugin Identity

| Property | Value |
|---|---|
| Plugin name | `Nop.Plugin.Messaging.RabbitMq` |
| Location | `src/Plugins/Nop.Plugin.Messaging.RabbitMq/` |
| Type | nopCommerce plugin (`IPlugin`) |
| NuGet dependency | `RabbitMQ.Client` |

---

## Components

### 1. `RabbitMqPlugin`

**Type:** `BasePlugin` implementation  
**File:** `RabbitMqPlugin.cs`

**Responsibilities:**
- Plugin entry point — implements `IPlugin`
- On install: declares the exchange `verdemart.orders` and queue `verdemart.orders.openboxes` with durable bindings
- On uninstall: does not delete the exchange or queue (messages in flight must not be lost)

---

### 2. `RabbitMqConnectionFactory`

**Type:** Singleton service  
**File:** `Infrastructure/RabbitMqConnectionFactory.cs`

**Responsibilities:**
- Opens and holds the single `IConnection` to RabbitMQ on first use
- Reads connection settings (host, port, user, password) from `RabbitMqSettings`
- Exposes `CreateChannel()` for callers that need a short-lived `IModel`
- Disposes the connection cleanly on application shutdown

---

### 3. `OrderPlacedConsumer`

**Type:** `IConsumer<OrderPlacedEvent>` implementation  
**File:** `Consumers/OrderPlacedConsumer.cs`

**Responsibilities:**
- Subscribes to `OrderPlacedEvent` via nopCommerce's event system
- Builds the `OrderPlacedMessage` payload from the event data
- Calls `RabbitMqConnectionFactory.CreateChannel()`, publishes the message, disposes the channel
- Publishes to exchange `verdemart.orders` with routing key `order.placed`
- Sets delivery mode to persistent (2)
- Does not catch and swallow exceptions — failures surface to nopCommerce's error handling

---

### 4. `OrderPlacedMessage`

**Type:** Plain DTO (serialised to JSON)  
**File:** `Messages/OrderPlacedMessage.cs`

**Responsibilities:**
- Carries the minimum payload needed by downstream consumers
- Immutable — set at construction, never modified after publish

**Fields:**

| Field | Type | Source |
|---|---|---|
| `OrderId` | `int` | `Order.Id` |
| `OrderGuid` | `Guid` | `Order.OrderGuid` |
| `CustomerId` | `int` | `Order.CustomerId` |
| `OrderTotal` | `decimal` | `Order.OrderTotal` |
| `CreatedOnUtc` | `DateTime` | `Order.CreatedOnUtc` |
| `Items` | `List<OrderItemMessage>` | `Order.OrderItems` |

---

### 5. `RabbitMqSettings`

**Type:** `ISettings` implementation  
**File:** `RabbitMqSettings.cs`

**Responsibilities:**
- Strongly-typed settings persisted in the nopCommerce database
- Configurable from the admin panel without redeployment

**Fields:**

| Field | Default |
|---|---|
| `Host` | `localhost` |
| `Port` | `5672` |
| `Username` | `guest` |
| `Password` | `guest` |
| `ExchangeName` | `verdemart.orders` |
| `OrderPlacedQueueName` | `verdemart.orders.openboxes` |

---

### 6. `PluginNopStartup`

**Type:** `INopStartup` implementation  
**File:** `Infrastructure/PluginNopStartup.cs`

**Responsibilities:**

- Registers `RabbitMqConnectionFactory` as a singleton via `IServiceCollection`
- Registers `OrderPlacedConsumer` so nopCommerce's event system can resolve it

---

## Component Relationships

```
nopCommerce event system
        │
        │ fires OrderPlacedEvent
        ▼
OrderPlacedConsumer
        │ builds payload
        ▼
OrderPlacedMessage
        │ serialised to JSON
        ▼
RabbitMqConnectionFactory ──── RabbitMqSettings
        │ CreateChannel()
        ▼
   IModel (channel)
        │ BasicPublish()
        ▼
RabbitMQ exchange: verdemart.orders
        │ routing key: order.placed
        ▼
Queue: verdemart.orders.openboxes
```

---

## What Step 5 Will Do

Step 5 defines the interfaces between these components — method signatures, message contracts, and the configuration surface — making the design precise enough to implement directly.
