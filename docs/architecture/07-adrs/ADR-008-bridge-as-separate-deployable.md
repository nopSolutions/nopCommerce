# ADR-008 — OpenBoxes Bridge as a Separate Deployable Service

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-28 |
| Produced by | ADD Iteration 3 — Step 6 (concern: CON-9; brief constraint) |

## Context

The OpenBoxes bridge consumes `verdemart.orders.openboxes` from RabbitMQ and creates fulfillment orders in OpenBoxes. CON-9 (Iter 3 Step 1) flagged the hosting model as open: a nopCommerce plugin running in the web process (matching ADR-005's logic for the dispatcher), or a separate independently-deployable service.

The Group Assignment 02 brief lists "at least one independently deployable subsystem" as a required technical constraint. Until this iteration, the project has no extracted service.

ADR-002 states that "all integration code lives inside nopCommerce plugins". The bridge sits **downstream of the broker** — it does not react to nopCommerce events directly, only to messages that have already been published. ADR-002's force is therefore arguable here: the spirit of decoupling the commerce core from downstream specifics is better served by extraction, not by re-coupling at the assembly level.

## Decision

The OpenBoxes bridge is a small independent service (`VerdeMart.OpenBoxesBridge`) running in its own Docker container, separate from the nopCommerce web process. It implements `BackgroundService` from `Microsoft.Extensions.Hosting`, connects to RabbitMQ via the same topology as Iter 1, and calls the OpenBoxes API. It has no compile-time dependency on any nopCommerce assembly. The only shared contract is the RabbitMQ topology and the JSON wire shape of `OrderPlacedMessage`.

The implementation language is .NET 8 by default for stack consistency with the main nopCommerce codebase, but the choice is reversible — any language with RabbitMQ and HTTP support can host the bridge.

## Rejected Alternative

**Bridge as a nopCommerce plugin consuming RabbitMQ in-process.**
*Rejected:* (a) makes nopCommerce knowledgeable about an external system's API, weakening the boundary the plugin layer was meant to protect; (b) the project as a whole would not satisfy the brief's "independently deployable subsystem" requirement; (c) ADR-002's spirit was decoupling the **commerce core** from downstream specifics — pulling OpenBoxes back into the same process re-couples them at the assembly level, defeating that aim.

## Consequences

- The brief's independently-deployable-subsystem requirement is satisfied.
- A new operational artifact exists: a service to deploy, monitor, and restart. Adds operational cost; mitigated by the standard `BackgroundService` patterns in `Microsoft.Extensions.Hosting`.
- The bridge can be scaled, restarted, or even reimplemented in a different language without touching nopCommerce.
- Cross-process debugging is harder than in-process; mitigated by structured logging keyed on `OrderGuid` (the correlation identifier set by ADR-003).
- The bridge needs a small local store for dedup state (recorded in ADR-009), adding a minor deployment dependency.
- Manual ack semantics from ADR-003 are honoured by the bridge: messages remain on the queue until OpenBoxes confirms creation, satisfying CON-11.
