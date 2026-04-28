# ADR-010 — Versioned Wire Contract with Tolerant Readers

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-28 |
| Produced by | ADD Iteration 3 — Step 6 (concern: CON-13) |

## Context

`OrderPlacedMessage` (Iter 1, Step 5) carries no version field today. Iteration 2 Step 7 named this as a cross-cutting concern blocking Iteration 3. As more consumers bind to `verdemart.orders` — this iteration adds the OpenBoxes bridge; ERPNext, search, and CRM bridges are foreseeable in later iterations — schema evolution risks silent breakage if any consumer reads strictly.

## Decision

The `OrderPlacedMessage` wire contract gains an explicit `Version` field of type `int`, with initial value `1`. The field is positional with a default in the C# `record` declaration, so the existing publish call site in `Nop.Plugin.Messaging.RabbitMq.Consumers.OrderPlacedConsumer` compiles unchanged; the publish path is updated to set the field explicitly when serialising.

Consumers read messages tolerantly:

- Unknown additional fields are ignored (the default behaviour of `System.Text.Json` deserialisation).
- Missing optional fields fall back to defaults.
- Additive changes to the contract do **not** bump the version.
- Breaking changes — field rename, type change, removal — require a new routing key (`order.placed.v2`) and a parallel queue binding for consumers that can handle the new shape. Old consumers continue to receive messages on the old routing key until migrated.

A consumer that receives a `Version` higher than it supports must NACK without requeue (route to DLQ), not silently process. This prevents accidental processing of an unknown contract.

## Rejected Alternatives

**No version field; consumers verify exact shape and reject unknown fields.**
*Rejected:* every additive change becomes a breaking change for at least one consumer, blocking schema evolution.

**Version embedded in routing key only (`order.placed.v1`) without a payload field.**
*Rejected:* the routing key carries the version for **breaking** changes; a payload field also gives consumers a defensive check independent of routing-key parsing, and lets log/audit tools inspect the version without resolving the binding.

**Schema registry (e.g. Confluent Schema Registry, Apicurio).**
*Rejected:* adds infrastructure for a property already addressable with a single field plus a routing-key convention. The brief warns against "too many technologies with shallow purpose"; a schema registry is over-investment at this scope.

## Consequences

- Schema evolution becomes safe within a version: additive changes do not require coordinated consumer updates.
- Breaking changes have an explicit, declared mechanism: new routing key + parallel binding. Versioned migration of consumers can proceed independently.
- Consumers gain a small defensive check (`if version > supported → DLQ`) preventing silent processing of unknown shapes. This pairs with ADR-009 — poison handling now covers contract drift, not only deserialisation failure.
- The Iter 1 plugin gains a one-line update to set the field; the change is backward-compatible because the field has a default value.
- The policy applies to **all** future events on `verdemart.orders` (and any future exchanges); it is not specific to `OrderPlacedMessage`.
