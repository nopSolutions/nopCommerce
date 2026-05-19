# ADR-0006 - Idempotency Strategy: messageId Inbox + sourceVersion

## Status

Accepted.

## Context

ADR-0003 establishes at-least-once delivery via RabbitMQ. The inbox/projection edge (Iteration 2) must reject duplicate deliveries and ignore stale POS stock updates without dropping legitimate state changes. QA-2 requires duplicate detection within 50 ms and zero duplicate fulfillment side effects.

## Decision

Two complementary mechanisms:

1. **`messageId` inbox**. Every external message carries a UUIDv4 `messageId`. The plugin records each `messageId` in `OmniInboxMessage` before applying side effects. A second arrival with the same `messageId` is rejected without re-applying the change.
2. **`sourceVersion` for stock**. POS stock events carry a monotonic `sourceVersion` per `(productId, warehouseId)`. The plugin compares against the last applied version in `OmniStockSyncState` and ignores updates with a lower or equal version.

`messageId` is the transport-level idempotency key; `sourceVersion` is the domain-level stale-detection key. Both are required.

## Consequences

- Duplicate fulfillment requests do not produce duplicate fulfillment rows.
- Stale POS updates do not overwrite newer state.
- Inbox table grows linearly with traffic; retention/pruning is a Part 2 concern (out of scope for the demo).

## Tradeoffs

- Inbox lookup adds one indexed read per message.
- Producers must generate `messageId` correctly; a buggy producer that reuses IDs would silently mask real updates.
- Choosing `sourceVersion` semantics (per-key monotonic vs. timestamp) commits us to monotonic source clocks; clock skew or POS bugs can cause legitimate updates to be ignored.

## Rejected Alternatives

- **Queue-level idempotency only** (e.g., RabbitMQ deduplication header). Rejected: not portable across brokers; ties idempotency to infra rather than domain.
- **Natural-key deduplication** (e.g., `OrderGuid + state`). Rejected: doesn't handle re-delivery of the same logical event with the same payload but different intent.
- **Last-write-wins on stock**. Rejected: silently corrupts state when out-of-order delivery happens, which is the failure mode QA-2 is designed to prevent.
