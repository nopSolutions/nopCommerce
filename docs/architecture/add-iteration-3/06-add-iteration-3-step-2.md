# ADD Iteration 3 — Step 2: Choose Element to Decompose

## What This Step Does

Step 2 selects the element of the system to design in this iteration. Everything from Step 3 onwards applies to this element only.

---

## Selected Element: The Cross-Channel Order Intake Path

The element to decompose is the **path a sale travels from the moment a channel begins committing it (web checkout entering `OrderProcessingService.PlaceOrderAsync`, or POS finalising an in-store sale) to the moment OpenBoxes holds an authoritative fulfillment task**.

This path has two phases that are designed together but execute on different threads:

1. **Synchronous phase — the allocation gate.** Inside the request that places the order, a per-product authority decides whether the unit is still available and either reserves it or refuses the sale. The losing channel sees a stock-unavailable response within the same request cycle (QAS-2 response measure).
2. **Asynchronous phase — the bridge consumer.** Downstream of the broker, the OpenBoxes bridge consumes `order.placed`, deduplicates by `OrderGuid`, and creates a fulfillment order in OpenBoxes.

The two phases collaborate by design: the synchronous phase produces the allocation that the asynchronous phase honours. Decomposing one without the other leaves QAS-2 unsatisfiable — either the gate exists but OpenBoxes never sees the order, or the bridge exists but oversell still happens at intake.

---

## Where It Lives in the System

```
WEB CHECKOUT                                       POS
   │                                                │
   │ OrderProcessingService.PlaceOrderAsync          │ pos.sale.completed
   │   → MoveShoppingCartItemsToOrderItemsAsync      │  (existing contract,
   │     → AdjustInventoryAsync (existing seam)      │   per 03-bounded-contexts.md)
   ▼                                                ▼
┌────────────────────────────────────────────────────┐
│              Allocation Gate                       │   ← SYNC, NEW
│   (location, mechanism, and convergence            │
│    decided at Step 3)                              │
└────────────────────────────────────────────────────┘
          │ allocation succeeds              │
          ▼                                  ▼
   Order committed                     POS sale committed
   (OrderPlacedEvent fires)
          │
          ▼
   Outbox row written (Iter 2 — unchanged)
          │
          ▼
   OutboxDispatcherTask → BasicPublish (Iter 2 — unchanged)
          │
          ▼
   ┌──────────────────────────────────┐
   │  RabbitMQ                        │
   │  exchange: verdemart.orders      │
   │  queue: …openboxes (durable)     │
   └──────────────────────────────────┘
          │
          ▼
┌────────────────────────────────────────────────────┐
│         OpenBoxes Bridge Consumer                  │   ← ASYNC, NEW
│   (host, dedup mechanism, DLQ topology             │
│    decided at Step 3)                              │
└────────────────────────────────────────────────────┘
          │ create fulfillment order
          ▼
   OpenBoxes (external system)
```

The two boxes labelled "NEW" are the only structural additions. The publish path between them — proven in Iterations 1 and 2 — is reused unchanged.

---

## Responsibilities In Scope for This Iteration

| Responsibility | In scope |
|---|---|
| Decide where the allocation gate lives and how web and POS both reach it (CON-15) | Yes |
| Implement the allocation-decrement path so exactly one of two concurrent attempts wins | Yes |
| Identify the precise insertion point in `OrderProcessingService` for the web flow — the candidates being `PreparePlaceOrderDetailsAsync`, the call site of `SaveOrderDetailsAsync`, and the existing seam at `AdjustInventoryAsync` | Yes |
| Define reservation timeout and release semantics for abandoned checkouts (CON-16) | Yes |
| Build the OpenBoxes bridge consumer | Yes |
| Decide bridge hosting — separate process vs nopCommerce plugin (CON-9) | Yes |
| Implement consumer-side idempotency on `OrderGuid` (closes ADR-003 mandate; CON-10) | Yes |
| Define DLQ routing for poison messages (CON-12) | Yes |
| Add a version field to `OrderPlacedMessage` and a forward-compatibility policy (CON-13) | Yes |

---

## Responsibilities Explicitly Outside This Iteration

| Responsibility | Owner |
|---|---|
| The publish path itself — Outbox, dispatcher, RabbitMQ topology | Iterations 1 + 2 — reused unchanged |
| ERPNext consumer of `order.placed` | Mechanical — bind another queue to the existing exchange; no architectural decision required |
| WireMock carrier integration and shipment-tracking webhook | Later iteration — addresses QAS-5 |
| The POS commerce engine itself | Out of scope — POS is an external system providing `pos.sale.completed`, per `03-bounded-contexts.md` |
| Recurring-payment subscription orders (`ProcessNextRecurringPaymentAsync`, which has its own `AdjustInventoryAsync` and `OrderPlacedEvent` sites) | Out of scope — different concurrency profile; QAS-2 is framed against web + POS first-time orders |
| Cleanup of expired reservations beyond the timeout primitive | Operational concern — later iteration |
| Outbox row retention policy | Carried from Iter 2 — operational, later |
| Multi-instance bridge consumers with leader election | Deferred — single instance is the simpler default |
| Cross-channel customer profile reconciliation | Documented out of scope in `03-bounded-contexts.md` (Key Ownership Tensions) |

---

## Why This Element

- It is the smallest element that can satisfy QAS-2 — QAS-2 fails if either phase is missing
- The element is **bounded on both sides by the existing publish path**: upstream by the Outbox (Iter 2), downstream by RabbitMQ (Iter 1). Nothing between those boundaries is touched
- The seam where stock physically decrements during web checkout is named in `02-current-state.md`: the call to `AdjustInventoryAsync` inside `MoveShoppingCartItemsToOrderItemsAsync`, invoked from `PlaceOrderAsync`. Two related observations narrow Step 3's design space without pre-deciding it:
  - `OrderPlacedEvent` fires **after** the stock decrement and **after** `SaveOrderDetailsAsync` has inserted the `Order` row. By the time the Outbox sees the event, the order is committed.
  - `PlaceOrderAsync` does not visibly wrap its writes in a single transaction. A gate that rejects late therefore needs explicit rollback semantics; a gate that runs earlier (before `SaveOrderDetailsAsync`) avoids that need entirely. Both are valid options for Step 3
- Choosing this element forces the two structural questions deferred from Step 1 (CON-9 bridge hosting, CON-15 shared gate) into Step 3, where they can be evaluated against named alternatives

---

## What Step 3 Will Do

Step 3 identifies the design concepts and tactics this element applies, each paired with the alternative considered and the reason for rejection. The open decisions Step 3 must close include:

- Where the allocation gate physically lives, and how web and POS both reach it
- Pessimistic DB lock vs optimistic concurrency vs distributed shared cache (the candidate concepts inherited from Iter 2 Step 7)
- Bridge hosting — separate process vs nopCommerce plugin
- Consumer-side dedup mechanism — bridge dedup table, OpenBoxes natural idempotency on `OrderGuid`, or both
- DLQ topology and the message-versioning policy
