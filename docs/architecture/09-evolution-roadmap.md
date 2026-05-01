# Evolution Roadmap

The implementation follows the ADD iteration sequence directly. Each phase is shippable and demonstrable on its own.

---

## Phase 1: Durable Publish Path (Iteration 1, QAS-1)

**What gets built:**
- `Nop.Plugin.Messaging.RabbitMq` plugin: `IConsumer<OrderPlacedEvent>` that publishes `OrderPlacedMessage` to RabbitMQ.
- RabbitMQ topology: durable exchange `verdemart.orders`, queues `verdemart.orders.openboxes` and `verdemart.orders.erpnext`, persistent delivery.

**Outcome:** Order placement events are durably queued. OpenBoxes and ERPNext can consume them when they come online.

---

## Phase 2: Transactional Outbox (Iteration 2, QAS-1 full closure)

**What gets built:**
- `OutboxMessage` table (FluentMigrator migration, same nopCommerce DB).
- `OrderPlacedConsumer` updated to write to the outbox inside the same transaction that commits the order; inline `BasicPublish` is removed.
- `OutboxDispatcherTask` (`IScheduleTask`, 1-second poll) reads pending outbox rows and publishes them to RabbitMQ.

**Outcome:** RabbitMQ is off the checkout thread. No order can be committed without a guaranteed eventual publish, even if the broker is down at the time of the order.

---

## Phase 3: Allocation Gate + OpenBoxes Bridge (Iteration 3, QAS-2)

**What gets built:**
- `AllocationGateDecorator` inside nopCommerce: pessimistic row lock on `ProductWarehouseInventory` at checkout; rejects with a structured error if stock is unavailable.
- `POST /api/inventory/reserve` endpoint: exposes the same gate to the POS over HTTP.
- `ProductReservation` table and `ReleaseExpiredReservationsTask`: automatic TTL-based release of held stock.
- **OpenBoxes Bridge**: separately deployable Docker container that consumes `verdemart.orders.openboxes`, creates fulfillment orders in OpenBoxes, and deduplicates on `OrderGuid`.

**Outcome:** Web and POS share one allocation authority. Oversell is impossible by construction. OpenBoxes receives every order exactly once.

---

## Phase 4: Carrier Integration (Iteration 4, QAS-5)

**What gets built:**
- `Nop.Plugin.Shipping.CarrierWebhook` plugin.
- Schema additions on `Shipment`: `ExternalShipmentId`, `ExternalCarrierCode`, `ExternalShippingStatus`, `LastStatusOccurredAtUtc`.
- `ShipmentSentConsumer`: writes a `carrier.booking.requested` row to the existing outbox when a shipment is dispatched.
- `CarrierBookingConsumer`: drains those outbox rows, calls WireMock, stores the returned tracking ID on the shipment.
- `POST /api/carrier/webhook` controller: authenticates (bearer), audits to `CarrierWebhookEvent` table, hands off to an in-process queue.
- `CarrierStatusConsumer`: correlates by `ExternalShipmentId`, applies an out-of-order timestamp guard, updates status and enqueues customer notification email in one DB transaction.

**Outcome:** The customer sees live carrier tracking status within 10 seconds of a webhook arriving. The full QAS set is structurally complete.

---

## Coexistence During Transition

| Component | Ph 1 | Ph 2 | Ph 3 | Ph 4 |
|---|---|---|---|---|
| nopCommerce monolith (core unmodified) | Active | Active | Active | Active |
| Inline `BasicPublish` in consumer | Active | **Removed** | | |
| `Nop.Plugin.Messaging.RabbitMq` + outbox dispatcher | | Active | Active | Active |
| RabbitMQ + `verdemart.orders` topology | Active | Active | Active | Active |
| `verdemart.orders.openboxes` queue (accumulating) | Accumulating | Accumulating | **Drained** | Drained |
| Allocation gate + `/api/inventory/reserve` | | | Active | Active |
| OpenBoxes Bridge container | | | Active | Active |
| `Nop.Plugin.Shipping.CarrierWebhook` + WireMock | | | | Active |

The only breaking transition is from Phase 1 to Phase 2: the inline broker publish is replaced by the outbox write inside `OrderPlacedConsumer`. Everything else is additive; each phase layers on top of the previous one without removing running components.
