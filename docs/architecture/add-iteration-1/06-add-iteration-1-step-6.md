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

## Decisions Recorded

This iteration produced three architectural decisions. The decisions are authored here (during ADD step 6) but live as standalone documents under `07-adrs/` so the consolidated ADR set can be read end-to-end by a reviewer.

- [ADR-001 — RabbitMQ as Message Broker](../07-adrs/ADR-001-rabbitmq-as-broker.md)
- [ADR-002 — Plugin Architecture as Integration Boundary](../07-adrs/ADR-002-plugin-as-integration-boundary.md)
- [ADR-003 — Durable Queues with Persistent Delivery and Manual Acknowledgement](../07-adrs/ADR-003-durable-queues-and-manual-ack.md)

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
