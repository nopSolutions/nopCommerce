# ADD Iteration 1 — Step 1: Review Inputs

## Iteration Goal

Satisfy **QAS-1 (Reliability)** as the primary driver.

QAS-1 forces the hardest structural decision in the system: the nopCommerce → OpenBoxes integration must not use a direct HTTP call. It must use a durable message queue. This decision establishes the integration backbone that every other bounded context will depend on.

---

## Inputs

### Design Purpose

This is a greenfield integration layer on top of an existing, fixed commerce core (nopCommerce). The purpose of this iteration is to produce an architectural structure for the plugin layer that bridges nopCommerce and RabbitMQ, satisfying the reliability requirement.

---

### Primary Driver: QAS-1

| Field | Value |
|---|---|
| Quality attribute | Reliability |
| Stimulus | A customer places an order on the web storefront |
| Source | nopCommerce checkout |
| Environment | OpenBoxes is temporarily unavailable |
| Artifact | The `order.placed` message published to RabbitMQ |
| Response | The message is durably queued; OpenBoxes consumes it when it recovers |
| Response measure | Zero orders lost during an OpenBoxes outage of up to 30 minutes; processed within 60 seconds of recovery |

---

### Secondary Drivers Considered (but not solved in this iteration)

| QAS | Quality Attribute | Relationship to QAS-1 |
|---|---|---|
| QAS-3 | Availability | Uses the same RabbitMQ infrastructure — addressed in Iteration 2 |
| QAS-4 | Recoverability | Depends on durable queuing being in place — QAS-1 is a prerequisite |

---

### Constraints

| Constraint | Source |
|---|---|
| nopCommerce is the fixed commerce core — it cannot be replaced or fundamentally altered | Business decision |
| RabbitMQ is the chosen message broker | ADR-001 (forthcoming) |
| Integration logic must live outside the nopCommerce core — inside a plugin | nopCommerce plugin architecture |
| The `PlaceOrderAsync` method is the canonical order placement hook | nopCommerce codebase |

---

### Architectural Concerns

| Concern | Description |
|---|---|
| CON-1 | The checkout thread must not block on downstream system availability |
| CON-2 | Messages must not be lost if the broker is temporarily unreachable at publish time |
| CON-3 | The plugin must not couple the commerce core to any specific downstream system |
| CON-4 | The solution must be testable in isolation — the plugin must be deployable without OpenBoxes running |

---

### Relevant Existing Structures

| Element | Role |
|---|---|
| `IConsumer<OrderPlacedEvent>` | nopCommerce event hook — the publish point for `order.placed` |
| `IPlugin` / `IDependencyRegistrar` | Plugin registration interfaces |
| `PlaceOrderAsync` in `OrderProcessingService` | Fires `OrderPlacedEvent` after a successful order |
| `OrderPlacedEvent` | The domain event carrying the order payload |

---

## What Step 1 Establishes

This step confirms that the inputs are complete and consistent before design begins. The following are confirmed:

- The primary quality attribute driver is identified: **Reliability**
- The stimulus and failure mode are concrete: **OpenBoxes is down when an order is placed**
- The artifact at risk is identified: **the `order.placed` message**
- The constraints narrow the solution space: **RabbitMQ, plugin boundary, no blocking HTTP**
- The existing hook point is known: **`IConsumer<OrderPlacedEvent>`**

Step 2 will select the element to decompose and identify the design concepts that satisfy QAS-1.
