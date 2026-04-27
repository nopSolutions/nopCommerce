# ADD Iteration 1 — Step 3: Identify Design Concepts

## What This Step Does

Step 3 selects the architectural tactics and patterns that the RabbitMQ publisher plugin must apply to satisfy QAS-1. Each concept is chosen because a specific requirement in QAS-1 demands it.

---

## Design Concepts Selected

### 1. Event-Driven Integration via IConsumer\<T\>

**Driver:** QAS-1 requires that the publish happens automatically after every successful order placement, without modifying the core checkout path.

**Concept:** nopCommerce exposes `IConsumer<OrderPlacedEvent>` as its event hook. The plugin implements this interface. nopCommerce fires the event after `PlaceOrderAsync` succeeds — the plugin reacts to it.

**Why this and not a direct call:** Implementing `IConsumer<T>` keeps the plugin fully decoupled from the order processing service. The core never references the plugin. The plugin can be removed without touching any core code.

---

### 2. Durable Queue with Persistent Delivery

**Driver:** QAS-1 response measure — zero orders lost during an OpenBoxes outage of up to 30 minutes.

**Concept:** The RabbitMQ queue must be declared with `durable: true`. Messages must be published with `deliveryMode: 2` (persistent). Together these ensure:
- The queue survives a RabbitMQ broker restart
- Messages on the queue survive a broker restart
- OpenBoxes can reconnect and drain the queue without data loss

**What this rules out:** transient queues, auto-delete queues, and default (non-persistent) message delivery. These are architectural violations against QAS-1.

---

### 3. Manual Acknowledgement on the Consumer Side

**Driver:** QAS-4 (recoverability) depends on this, but it is established here because it must be declared at queue definition time.

**Concept:** The queue must be consumed with `autoAck: false`. The OpenBoxes bridge must explicitly acknowledge each message only after it has successfully created the fulfillment order. If processing fails, the message is requeued and retried.

**Note:** The publisher plugin does not control acknowledgement — that is the consumer's responsibility. But the publisher must declare the queue correctly to allow it.

---

### 4. Fanout via Direct Exchange with Routing Key

**Driver:** Multiple bounded contexts consume `order.placed` (OpenBoxes and ERPNext). The exchange topology must support this without the publisher knowing who is listening.

**Concept:** The plugin publishes to a **direct exchange** named `verdemart.orders` with routing key `order.placed`. Each consumer binds its own durable queue to this exchange with the same routing key. Adding a new consumer requires no change to the publisher.

**Why not fanout exchange:** A direct exchange with routing keys gives more control as the topology grows. A fanout would send every message to every bound queue regardless of type — impractical once more event types are added.

---

### 5. Connection Management via Singleton Channel Factory

**Driver:** nopCommerce is a multi-threaded web application. Multiple requests can fire `OrderPlacedEvent` concurrently. RabbitMQ channels are not thread-safe.

**Concept:** The plugin registers a singleton `IConnection` (one TCP connection to RabbitMQ) via `IDependencyRegistrar`. Each publish operation creates a short-lived `IModel` (channel) from that connection, uses it, and disposes it. This is the standard RabbitMQ .NET client pattern for concurrent publishers.

---

## Summary: Concepts → QAS-1 Requirements

| QAS-1 requirement | Design concept that satisfies it |
|---|---|
| Message must survive OpenBoxes outage | Durable queue + persistent delivery |
| No manual intervention on recovery | Manual acknowledgement + automatic requeue |
| Checkout must not be coupled to OpenBoxes | `IConsumer<OrderPlacedEvent>` decoupling |
| Multiple contexts can consume the same event | Direct exchange with routing key |
| Safe under concurrent order placement | Singleton connection, per-publish channel |

---

## What Step 4 Will Do

Step 4 instantiates the actual components — assigns names, types, and packages to the concepts identified here. The plugin structure, class names, and registration points will be defined.
