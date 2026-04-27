# ADD Iteration 2 — Step 2: Choose Element to Decompose

## What This Step Does

Step 2 picks the element of the system to refine in this iteration. Everything from Step 3 onward applies to this element only.

---

## Selected Element: The Publish Path inside `Nop.Plugin.Messaging.RabbitMq`

The element to refine is the **flow of an `order.placed` message from the moment `OrderPlacedEvent` fires to the moment the message lands on the RabbitMQ exchange**.

In Iteration 1 this flow was a single inline `BasicPublish` call inside `OrderPlacedConsumer.HandleEventAsync`. This iteration splits it into two phases:

1. **Synchronous phase, inside the order transaction** — record the publish intent in a local `Outbox` table
2. **Asynchronous phase, outside the request thread** — a dispatcher reads the table and publishes to RabbitMQ

---

## Where It Lives in the System

```
nopCommerce
│
├── OrderProcessingService.PlaceOrderAsync()          ← unchanged
│       └── DB transaction commits (Order, StockQty)
│       └── fires OrderPlacedEvent
│
└── [Plugin] Nop.Plugin.Messaging.RabbitMq
        ├── OrderPlacedConsumer                        ← BODY CHANGES
        │       └── writes to Outbox table             ← NEW behaviour
        │
        ├── Outbox table (in nopCommerce DB)           ← NEW
        │
        └── OutboxDispatcherTask : IScheduleTask       ← NEW component
                └── polls Outbox → BasicPublish to RabbitMQ
```

---

## Responsibilities In Scope for This Iteration

| Responsibility | In scope |
|---|---|
| Define the Outbox table schema | Yes |
| Migrate the Outbox table on plugin install | Yes |
| Replace inline publish in `OrderPlacedConsumer` with an outbox write | Yes |
| Build the dispatcher as an `IScheduleTask` | Yes |
| Define dispatcher polling, batching, and status-update logic | Yes |
| Reuse `RabbitMqConnectionFactory` for the dispatcher's actual publish | Yes |

## Responsibilities Explicitly Outside This Iteration

| Responsibility | Owner |
|---|---|
| Consumer-side idempotency implementation | OpenBoxes bridge: Iteration 3 |
| Message schema versioning policy | Cross-cutting: to be settled before Iteration 3 |
| Outbox-table retention/cleanup | Operational concern: later iteration |
| Multi-instance dispatcher with leader election | Deferred: single instance is sufficient at VerdeMart's scale |
| Inbound webhook ingestion | Different element entirely: later iteration |

---

## Why This Element

- It is the smallest change that satisfies the iteration goal — only the publish path moves
- The plugin boundary established by ADR-002 holds; nopCommerce core is untouched
- The new `Outbox` table sits inside the same database as the Order — that atomicity is the whole point
- The dispatcher reuses the existing `IScheduleTask` framework — no new deployable, no new process

---

## What Step 3 Will Do

Step 3 selects the design concepts (tactics, patterns, sub-decisions) the new publish path applies, and pairs each with the alternative that was rejected.
