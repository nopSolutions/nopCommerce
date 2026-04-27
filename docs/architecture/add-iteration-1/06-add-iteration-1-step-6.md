# ADD Iteration 1 — Step 6: Sketch Views and Record Design Decisions

## What This Step Does

Step 6 produces two outputs: a component view of what was designed, and the ADRs that record why each decision was made. These are the durable artefacts of the iteration.

---

## Component View

```
┌─────────────────────────────────────────────────────────────┐
│  nopCommerce                                                 │
│                                                             │
│  OrderProcessingService                                     │
│       │ PlaceOrderAsync()                                   │
│       │ fires                                               │
│       ▼                                                     │
│  [ OrderPlacedEvent ]                                       │
│       │                                                     │
│       │ nopCommerce event system                            │
│       ▼                                                     │
│  ┌─────────────────────────────────────────────────┐        │
│  │  Nop.Plugin.Messaging.RabbitMq                  │        │
│  │                                                 │        │
│  │  OrderPlacedConsumer                            │        │
│  │       │ builds OrderPlacedMessage               │        │
│  │       │ calls CreateChannel()                   │        │
│  │       ▼                                         │        │
│  │  RabbitMqConnectionFactory (singleton)          │        │
│  │       │ reads RabbitMqSettings                  │        │
│  │       │ holds IConnection                       │        │
│  │       │ returns IModel (channel)                │        │
│  └───────┼─────────────────────────────────────────┘        │
│          │                                                   │
└──────────┼───────────────────────────────────────────────────┘
           │ BasicPublish()
           │ exchange: verdemart.orders
           │ routing key: order.placed
           │ delivery mode: persistent
           ▼
┌─────────────────────────────┐
│  RabbitMQ                   │
│                             │
│  exchange: verdemart.orders │
│       │ direct              │
│       │ durable             │
│       ▼                     │
│  queue: verdemart.orders    │
│         .openboxes          │
│       durable               │
│       manual ack            │
└─────────────────────────────┘
           │
           │ (future iterations)
           ▼
    OpenBoxes bridge service
    ERPNext bridge service
```

---

## ADR-001: RabbitMQ as Message Broker

**Status:** Accepted

**Context:**  
nopCommerce must notify OpenBoxes and ERPNext when an order is placed. A direct HTTP call from the checkout thread creates a hard dependency on those systems being available and fast. QAS-1 and QAS-3 explicitly forbid this.

**Decision:**  
Use RabbitMQ as the message broker for all cross-context event delivery. All bounded contexts communicate via RabbitMQ exchanges and queues, not via direct HTTP calls in the synchronous request path.

**Consequences:**
- Checkout is decoupled from downstream system availability
- Messages are durable — an OpenBoxes outage does not lose orders
- Adds RabbitMQ as a required infrastructure component
- Bridge services are needed for systems that do not natively consume AMQP

---

## ADR-002: Plugin Architecture as Integration Boundary

**Status:** Accepted

**Context:**  
nopCommerce core must not be modified. The integration logic must be addable and removable without touching the commerce engine.

**Decision:**  
All integration code lives inside nopCommerce plugins. Plugins use `IConsumer<T>` to react to domain events and `INopStartup` to register their services. The core has no reference to any plugin.

**Consequences:**
- Integration can be enabled or disabled from the admin panel
- Each bounded context gets its own plugin, keeping concerns separated
- Plugin boundaries enforce the rule that the commerce core does not know about external systems

---

## ADR-003: Durable Queues with Persistent Delivery and Manual Acknowledgement

**Status:** Accepted

**Context:**  
QAS-1 requires zero message loss during an OpenBoxes outage of up to 30 minutes. QAS-4 requires that all missed events self-heal after recovery with no operator action.

**Decision:**  
All queues are declared durable. All messages are published with delivery mode 2 (persistent). All consumers use manual acknowledgement — a message is acknowledged only after the downstream action succeeds.

**Consequences:**
- Messages survive a RabbitMQ broker restart
- Messages survive a consumer outage of any duration (bounded only by disk)
- Consumer logic must be idempotent — redelivered messages must not create duplicates
- `OrderGuid` is the idempotency key for all order-related messages

---

## What This Iteration Produced

| Artefact | Description |
|---|---|
| Plugin structure | `Nop.Plugin.Messaging.RabbitMq` with 6 named components |
| Wire contract | `OrderPlacedMessage` with `OrderGuid` as idempotency key |
| Topology | `verdemart.orders` exchange → `verdemart.orders.openboxes` queue |
| ADR-001 | RabbitMQ as message broker |
| ADR-002 | Plugin architecture as integration boundary |
| ADR-003 | Durable queues, persistent delivery, manual acknowledgement |

---

## What Iteration 2 Will Address

QAS-3 — Availability: checkout must complete when ERPNext is slow.

This introduces the **outbox pattern**: instead of publishing directly to RabbitMQ inside `HandleEventAsync`, the consumer writes the message to a database table first. A background worker reads the table and publishes to RabbitMQ. This protects against the case where RabbitMQ itself is temporarily unreachable at publish time.
