# ADD Iteration 2 — Step 1: Review Inputs

## Iteration Goal

Close the broker-down hole left open by Iteration 1 by making the order commit and the outbound message dispatch atomic. After this iteration, no order can exist in the database without a guaranteed eventual publish to RabbitMQ — regardless of whether the broker is reachable at commit time.

This iteration also removes RabbitMQ from the synchronous checkout path entirely, so checkout latency is bounded by database performance only.

---

## Inputs

### Primary Driver: QAS-1 (Full Coverage)

Iteration 1 satisfied QAS-1 *under the explicit assumption that RabbitMQ is up*. Step 7 of Iteration 1 recorded this as the residual broker-down hole. Iteration 2 lifts that assumption.

| Field | Value |
|---|---|
| Quality attribute | Reliability |
| Stimulus | A customer places an order |
| Source | nopCommerce checkout |
| Environment | RabbitMQ is unreachable, slow, or the application crashes between order commit and publish |
| Artifact | The `order.placed` message and the underlying Order row |
| Response | The order commit and the publish intent are atomic; the dispatcher publishes when the broker is reachable |
| Response measure | Zero orders missing a published message, even when the broker is unavailable for up to 30 minutes; recovery within 60 s of broker return |

---

### Explicitly Not a Driver: QAS-3

QAS-3 (checkout completes when surrounding systems are slow) is **already satisfied** by Iteration 1. Surrounding systems are consumers of RabbitMQ events; the publish path does not know who is listening. No surrounding system is ever on the synchronous checkout thread. This driver is recorded here as resolved, not as a target for this iteration.

---

### Inherited from Iteration 1 Step 7

| Inherited input | Source |
|---|---|
| Constraint: the publish path must not block on RabbitMQ availability | Promoted from CON-2 (Iteration 1 Step 1) |
| Element to refine: the publish path between order commit and broker | Identified in Iteration 1 Step 7 |
| Candidate concept to evaluate: transactional outbox + background dispatcher | Iteration 1 Step 7 inputs |
| Candidate alternative to compare: publish-with-retry off-thread | Iteration 1 Step 7 inputs |

---

### Constraints

| Constraint | Source |
|---|---|
| nopCommerce remains the fixed commerce core | Carried from Iteration 1 |
| All integration code lives inside plugins | ADR-002 |
| RabbitMQ is the only message broker | ADR-001 |
| The outbox storage must reuse the existing nopCommerce database | The brief's "no shared-database shortcut" applies *across extracted services*; the monolith reusing its own database is the only way to get atomicity with the order commit |
| Dispatcher must run without operator action, including after a process restart | QAS-1 response measure |

---

### Architectural Concerns

| Concern | Description |
|---|---|
| CON-5 | At-least-once delivery becomes mandatory; consumers must be idempotent (already mandated by ADR-003) |
| CON-6 | The dispatcher is a new component to run, observe, and restart |
| CON-7 | Outbox table polling must not contend with order-write traffic |
| CON-8 | Outbox row growth must be bounded over time |

---

### Relevant Existing Structures

| Element | Role |
|---|---|
| `Nop.Plugin.Messaging.RabbitMq` | Existing plugin from Iteration 1 — extended, not replaced |
| `OrderPlacedConsumer` | Existing — only the body of `HandleEventAsync` changes |
| `RabbitMqConnectionFactory` | Existing — reused by the new dispatcher |
| `IScheduleTask` framework | Existing nopCommerce extension point — host the dispatcher inside it |
| FluentMigrator | Existing schema migration tool used by nopCommerce — used to create the Outbox table |

---

## What Step 1 Establishes

- The iteration's purpose is **completion of QAS-1**, not a new driver
- The element of change is bounded to the publish path inside the existing plugin
- Reuse of the nopCommerce database for the outbox table is justified by the atomicity requirement and is **not** a "shared database across extracted services"
- The candidate tactic and its alternative are pre-named — Step 4 will compare them properly

Step 2 selects the element to decompose.
