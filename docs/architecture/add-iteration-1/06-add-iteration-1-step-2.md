# ADD Iteration 1 — Step 2: Choose Element to Decompose

## What This Step Does

Step 2 selects the element of the system to design in this iteration. Everything produced from Step 3 onward applies to this element only.

---

## Selected Element: The RabbitMQ Publisher Plugin

The element to decompose is the **nopCommerce plugin responsible for publishing `order.placed` to RabbitMQ** when an order is successfully placed.

This is the smallest element that directly satisfies QAS-1. It sits at the exact boundary between the Commerce Context and the message broker.

---

## Where It Lives in the System

```
nopCommerce core
│
├── OrderProcessingService.PlaceOrderAsync()
│       └── fires OrderPlacedEvent
│
└── [Plugin] Nop.Plugin.Messaging.RabbitMq
        └── IConsumer<OrderPlacedEvent>
                └── publishes order.placed → RabbitMQ
```

The plugin is the only element that changes in this iteration. nopCommerce core is untouched. RabbitMQ is infrastructure — it is configured, not built.

---

## Responsibilities of This Element

| Responsibility | In scope |
|---|---|
| Subscribe to `OrderPlacedEvent` via `IConsumer<T>` | Yes |
| Serialize the order payload to a message | Yes |
| Publish to RabbitMQ with durable, persistent delivery | Yes |
| Declare the exchange and queue on startup | Yes |
| Register itself via `IDependencyRegistrar` | Yes |

## Responsibilities Explicitly Outside This Element

| Responsibility | Owner |
|---|---|
| Consuming `order.placed` from RabbitMQ | OpenBoxes bridge service (later iteration) |
| Retrying if RabbitMQ is unreachable at publish time | Outbox pattern — Iteration 2 |
| Publishing any other event (`order.paid`, `order.cancelled`) | Later iterations |
| Deserializing or acting on any inbound message | Separate consumer plugin |

---

## Why This Element First

- It is the publish point — nothing downstream can work until messages are on the queue
- It has a single, well-defined trigger: `OrderPlacedEvent`
- It can be built and verified in isolation — OpenBoxes does not need to be running
- It is the minimum viable piece that proves QAS-1 is satisfiable

---

## What Step 3 Will Do

Step 3 will identify the design concepts — the specific tactics and patterns — that this element must apply to satisfy QAS-1. This includes how the connection is managed, how the message is declared durable, and how the plugin handles a RabbitMQ connection that is available but slow.
