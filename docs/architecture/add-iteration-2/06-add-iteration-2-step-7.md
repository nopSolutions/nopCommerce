# ADD Iteration 2 — Step 7: Analyze Current Design

## What This Step Does

Step 7 closes the iteration. It checks whether the design produced satisfies the iteration goal, names what remains unresolved, and produces the input list for Iteration 3.

---

## Iteration Goal Recap

Iteration 2 set out to close the broker-down hole left open by Iteration 1 — to make the order commit and the message dispatch atomic so no order can exist in the database without a guaranteed eventual publish, and to remove RabbitMQ from the synchronous checkout path.

---

## Analysis Against the QAS-1 Response Measure (End-to-End)

| Failure mode | Iteration 1 result | Iteration 2 result |
|---|---|---|
| OpenBoxes (consumer) down for 30 min | Satisfied — broker buffers | Satisfied — unchanged |
| Order in OpenBoxes within 60 s of consumer recovery | Plausible (mechanism in place) | Same — dispatcher poll adds ≤ 1 s |
| RabbitMQ (broker) unreachable at commit time | **Unsatisfied** — message lost | **Satisfied** — outbox holds intent until broker returns |
| App crashes between order commit and publish | **Unsatisfied** — message lost | **Satisfied** — no "between" window; outbox row commits with the order |
| Checkout latency from inline broker call | **Open** — synchronous BasicPublish | **Closed** — broker is no longer in the request path |

---

## What Is Fully Satisfied

- QAS-1 across all three failure modes (consumer down, broker down, app crash)
- Checkout latency is bounded by DB performance only
- Recovery requires no operator action — the dispatcher resumes automatically on each tick
- Audit trail is preserved — `Outbox` rows retain a record of every published message with timestamps

---

## What Is Partially Satisfied

| Item | What is in place | What is missing |
|---|---|---|
| 60-second recovery clause | Mechanism (poll + auto-reconnect + durable queue) is in place | Empirical measurement — feasibility spike still required |
| At-least-once delivery (CON-5) | Publisher side honoured by design | Consumer side (idempotency in the OpenBoxes bridge) not yet implemented |

---

## What Is Not Satisfied — Residual Risks

| Residual risk | Why it remains | Where it goes |
|---|---|---|
| **Dispatcher single point of failure** | Single-instance dispatcher; if its host process is down, no publishing happens | Risks list (topic 8); reversible — can move to leader-elected multi-instance later |
| **Outbox table unbounded growth** | This iteration retains `Sent` rows for audit; no retention task | Operational concern; add a cleanup task in Iteration 3+ |
| **Consumer idempotency not implemented** | No consumer exists yet; ADR-003 mandates the policy but the OpenBoxes bridge will need to honour it | Iteration 3 — OpenBoxes bridge |
| **Message schema versioning still unaddressed** | `OrderPlacedMessage` has no version field, no compatibility policy | Cross-cutting concern; resolve before Iteration 3 |
| **No DLQ for poison messages** | If a row hits `MaxAttempts`, it stays in the table; no operator workflow yet | Iteration 3 (when consumer exists) — DLQ becomes meaningful only when there is a consumer to reject |
| **Polling interval vs DB load is unmeasured** | 1 s default is a guess — may be too aggressive on a busy DB | Feasibility spike (topic 10) |

---

## Inputs Carried Into Iteration 3

| Inherited input | Origin |
|---|---|
| **Primary driver:** QAS-2 (Consistency — last-unit oversell across web and POS) | Scenario QAS set; not yet addressed |
| **Build the OpenBoxes bridge consumer** | Required to make consumer-side idempotency real |
| **Settle message schema versioning policy** | Cross-cutting concern blocking Iteration 3 |
| **Element to refine:** the order placement path itself (concurrency control on `ProductWarehouseInventory`) | New element — not the publish path |
| **Candidate concept to evaluate:** pessimistic DB row lock with serializable transaction | Standard tactic for last-write-wins concurrency |
| **Candidate alternative to compare:** optimistic concurrency with row version + retry | Lighter-weight option; defensible alternative |

---

## Iteration Verdict

Iteration 2 closed the broker-down hole, removed RabbitMQ from the synchronous checkout path, and made the order commit and publish intent atomic. QAS-1 is now satisfied across all named failure modes, subject to the residual risks above (most notably the unmeasured 60 s recovery clause and the not-yet-implemented consumer idempotency).

One architectural decision was recorded:
- [ADR-004 — Transactional Outbox for Reliable Publish](../07-adrs/ADR-004-transactional-outbox.md)

The iteration is closed. Iteration 3 begins with QAS-2 and the OpenBoxes bridge consumer.
