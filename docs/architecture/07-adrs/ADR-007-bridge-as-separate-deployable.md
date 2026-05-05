# ADR-007 — OpenBoxes Bridge as a Separate Deployable Service

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-28 |
| Produced by | ADD Iteration 3 — Step 6 (concern: CON-9; brief constraint) |

## Context

The OpenBoxes bridge consumes `verdemart.orders.openboxes` from RabbitMQ and creates fulfillment orders in OpenBoxes. CON-9 (Iter 3 Step 1) flagged the hosting model as open: a nopCommerce plugin running in the web process (as the outbox dispatcher does), or a separate independently-deployable service.

The Group Assignment 02 brief lists "at least one independently deployable subsystem" as a required technical constraint. Until this iteration, the project has no extracted service.

ADR-002 states that "all integration code lives inside nopCommerce plugins". The bridge sits **downstream of the broker** — it does not react to nopCommerce events directly, only to messages that have already been published. ADR-002's force is therefore arguable here: the spirit of decoupling the commerce core from downstream specifics is better served by extraction, not by re-coupling at the assembly level.

## Decision

The OpenBoxes bridge is a small independent service (`VerdeMart.OpenBoxesBridge`) running in its own Docker container, separate from the nopCommerce web process. It implements `BackgroundService` from `Microsoft.Extensions.Hosting`, connects to RabbitMQ via the same topology as Iter 1, and calls the OpenBoxes API. It has no compile-time dependency on any nopCommerce assembly. The only shared contract is the RabbitMQ topology and the JSON wire shape of `OrderPlacedMessage`.

The implementation language is .NET 8 by default for stack consistency with the main nopCommerce codebase, but the choice is reversible — any language with RabbitMQ and HTTP support can host the bridge.

## Rejected Alternatives

**Bridge as a nopCommerce plugin consuming RabbitMQ in-process.**
*Rejected:* (a) makes nopCommerce knowledgeable about an external system's API, weakening the boundary the plugin layer was meant to protect; (b) the project as a whole would not satisfy the brief's "independently deployable subsystem" requirement; (c) ADR-002's spirit was decoupling the **commerce core** from downstream specifics — pulling OpenBoxes back into the same process re-couples them at the assembly level, defeating that aim.

**Outbox dispatcher calls OpenBoxes REST API directly, bypassing RabbitMQ for this path.**
The `OutboxDispatcherTask` already polls `OutboxMessage` rows and could call the OpenBoxes HTTP API directly instead of publishing to the broker, eliminating the bridge and one infrastructure hop entirely. *Rejected:* this couples the nopCommerce scheduler loop to OpenBoxes availability. If OpenBoxes is slow or unreachable, the dispatcher stalls and all other outbox rows (carrier bookings, future integrations) are also delayed — a single slow external system contaminates the shared retry loop. This directly threatens QAS-1: a stalled dispatcher cannot guarantee delivery within 60 seconds of OpenBoxes recovery, and the retry behaviour under a prolonged outage is undefined rather than structurally safe. It also threatens QAS-3: although checkout itself is insulated by the outbox write, a dispatcher blocked on a dead OpenBoxes endpoint delays carrier booking rows regardless of carrier availability — a surrounding system the customer never interacted with disrupts an unrelated integration path. More fundamentally, it violates ADR-001: the explicit motivation for RabbitMQ was to remove direct HTTP calls from any nopCommerce processing path. The broker is the durability boundary; moving the HTTP call back inside the nopCommerce process erases it.

**nopCommerce `IScheduleTask` polling a staging table and pushing to OpenBoxes.**
A scheduled task could read orders with `SyncStatus = Pending` from a staging table and POST them to OpenBoxes on a fixed interval, with no broker involvement at all. *Rejected:* `02-current-state.md` documents that the `IScheduleTask` framework has no per-item progress tracking and no at-least-once guarantee — a task that crashes on the 51st row restarts from scratch. Achieving reliable at-least-once delivery on top of it requires implementing exactly the outbox pattern that ADR-004 already adopts, making this alternative a worse reimplementation of what is already in place. It also re-introduces the OpenBoxes-availability coupling that RabbitMQ was chosen to eliminate.

**RabbitMQ Shovel plugin forwarding messages as HTTP POST to OpenBoxes.**
RabbitMQ's Shovel plugin can forward messages between brokers or to AMQP endpoints. A hypothetical HTTP-delivery variant would let the broker call the OpenBoxes REST API directly, removing the need for any bridge process. *Rejected:* RabbitMQ Shovel transfers messages between AMQP endpoints, not to arbitrary HTTP REST APIs. Achieving HTTP delivery from RabbitMQ requires either the community `rabbitmq-web-dispatch` plugin (unmaintained, not production-grade) or replacing RabbitMQ with a broker that natively supports HTTP push (e.g. a webhook relay service). Either path changes the broker decision recorded in ADR-001 and adds a dependency with shallower community support than the chosen stack. The operational cost of debugging a misconfigured broker-level HTTP delivery is higher than debugging a purpose-built `BackgroundService`.

## Consequences

- The brief's independently-deployable-subsystem requirement is satisfied.
- A new operational artifact exists: a service to deploy, monitor, and restart. Adds operational cost; mitigated by the standard `BackgroundService` patterns in `Microsoft.Extensions.Hosting`.
- The bridge can be scaled, restarted, or even reimplemented in a different language without touching nopCommerce.
- Cross-process debugging is harder than in-process; mitigated by structured logging keyed on `OrderGuid` (the correlation identifier set by ADR-003).
- The bridge needs a small local store for dedup state (a local `processed_orders` table keyed on `OrderGuid`), adding a minor deployment dependency.
- Manual ack semantics from ADR-003 are honoured by the bridge: messages remain on the queue until OpenBoxes confirms creation, satisfying CON-11.
