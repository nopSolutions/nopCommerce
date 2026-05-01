# ADD Iteration 1 — Step 7: Analyze Current Design

## What This Step Does

Step 7 closes the iteration. It checks whether the design produced in steps 2–6 actually satisfies the iteration goal set in step 1, names what remains unresolved, and produces the input list for iteration 2.

---

## Iteration Goal Recap

Iteration 1 set out to satisfy **QAS-1 (Reliability)** by introducing a durable async path between nopCommerce and the OpenBoxes bridge, replacing what would otherwise be a direct, blocking HTTP call from the checkout thread.

---

## Analysis Against the QAS-1 Response Measure

QAS-1's response measure has two clauses. Each is checked separately against the design.

| Response measure clause | How the design addresses it | Verdict |
|---|---|---|
| Zero orders lost during a 30-minute OpenBoxes outage | Queue is declared durable; messages are published with delivery mode 2 (persistent); consumer uses manual ack so unprocessed messages are requeued | **Satisfied, provided RabbitMQ itself stays up** |
| Order appears in OpenBoxes within 60s of recovery | Consumer reconnects automatically; RabbitMQ delivers the backlog in order; throughput depends on consumer processing speed | **Plausible but unmeasured**: verifiable only via a feasibility spike |

---

## What Is Fully Satisfied

- The publish point exists and is decoupled from the commerce core (ADR-002)
- The queue topology survives a broker restart (ADR-003)
- The consumer can drain a backlog after recovery without operator action (ADR-003)
- Multiple bounded contexts can subscribe to `order.placed` independently via the direct exchange topology (Step 3)

---

## What Is Partially Satisfied

| Item | What is in place | What is missing |
|---|---|---|
| 60-second recovery clause | The mechanism (auto-reconnect + durable backlog) is in place | No measurement exists — only a spike can confirm it |
| CON-1 (checkout must not block on downstream availability) | Publish is async to OpenBoxes — checkout never waits on the bridge | Publish is **synchronous to RabbitMQ** inside `HandleEventAsync`; if the broker is slow, checkout slows |

---

## What Is Not Satisfied — Residual Risks

| Residual risk | Why it remains | Where it goes |
|---|---|---|
| **Broker-down hole (dual-write problem)** — if RabbitMQ is unreachable when `OrderPlacedConsumer` fires, the message is lost while the order is committed to the database | Explicitly deferred at Step 1 (CON-2). Inline publish from the consumer cannot solve it | Iteration 2 — Transactional Outbox |
| **Consumer idempotency** — redelivered messages could create duplicate fulfillment orders downstream | The OpenBoxes bridge does not exist yet; ADR-003 only mandates the policy that consumers must be idempotent | Iteration 3 — OpenBoxes bridge service |
| **No message versioning** — the `OrderPlacedMessage` schema cannot evolve without breaking existing consumers | Not addressed in this iteration; no version field, no schema registry, no compatibility policy | Cross-cutting concern — must be resolved before Iteration 3 |
| **Connection failure handling** — `RabbitMqConnectionFactory` opens the connection lazily; behaviour on first-call failure is undefined | Out of scope for the QAS-1 framing in this iteration | Add to risks list (topic 8) |
| **Recovery time unmeasured** — the 60-second clause in QAS-1 cannot be claimed without evidence | No spike has been run yet | Feasibility spike (topic 10) |

---

## Inputs Carried Into Iteration 2

Iteration 2 inherits the following from this analysis:

| Inherited input | Origin |
|---|---|
| **Primary driver:** QAS-3 (Availability — checkout completes when surrounding systems are slow) | Scenario QAS set |
| **Carried-over driver:** QAS-1 remains partly open via the broker-down hole | Residual risk above |
| **New constraint:** the publish path must not block on RabbitMQ availability either (closes CON-2) | Promoted from concern to constraint |
| **Element to refine:** the publish path itself — specifically, where the message lives between `OrderPlacedEvent` firing and arriving on the broker | Identified as the source of the broker-down hole |
| **Candidate design concept to evaluate:** transactional outbox — write the message to a local DB table inside the same transaction that commits the order; a separate dispatcher publishes asynchronously to RabbitMQ | Industry-standard tactic for the dual-write problem |
| **Candidate alternative to compare:** at-least-once publish-with-retry (no outbox) — accepting message loss only if both DB commit and broker fail at the exact same moment | Lighter-weight option to consider in Iteration 2 step 4 |

---

## Iteration Verdict

Iteration 1 produced a working design for the publish path that satisfies QAS-1 **under the explicit assumption that RabbitMQ is available**. The broker-down case is a known residual risk, deferred to Iteration 2 by design rather than overlooked.

Three architectural decisions were recorded:
- [ADR-001 — RabbitMQ as Message Broker](../07-adrs/ADR-001-rabbitmq-as-broker.md)
- [ADR-002 — Plugin Architecture as Integration Boundary](../07-adrs/ADR-002-plugin-as-integration-boundary.md)
- [ADR-003 — Durable Queues with Persistent Delivery and Manual Acknowledgement](../07-adrs/ADR-003-durable-queues-and-manual-ack.md)

The iteration is closed. Iteration 2 begins with the inputs listed above.
