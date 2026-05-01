# Evolution Roadmap

The implementation follows the ADD iteration sequence directly. Each phase is shippable and demonstrable on its own.

---

## Phase 1: Durable Publish Path (Iteration 1, QAS-1)

**What gets built:**
- `Nop.Plugin.Messaging.RabbitMq` plugin: `IConsumer<OrderPlacedEvent>` that publishes `OrderPlacedMessage` to RabbitMQ.
- RabbitMQ topology: durable exchange `verdemart.orders`, queue `verdemart.orders.openboxes`, persistent delivery.

**Outcome:** Order placement events are durably queued. OpenBoxes can consume them when it comes online. The exchange topology supports additional consumers (e.g. ERPNext) as future queue bindings — no publisher change required.

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

**Outcome:** The customer sees live carrier tracking status within 10 seconds of a webhook arriving. QAS-5's carrier clause is structurally satisfied.

---

---

## Phase 5: OpenBoxes State Visibility (Iteration 5, QAS-5 full closure)

**What gets built:**
- `OpenBoxesStatusPollerTask` (`IScheduleTask`, 30-second poll) added to `Nop.Plugin.Inventory.AllocationGate`.
- `IOpenBoxesClient` extended with `GetIssuedFulfillmentOrdersAsync()` — polls `GET /api/generic/shipment?status=ISSUED`.
- On `ISSUED` detection: updates nopCommerce order status + writes outbox row to trigger carrier booking automatically.

**Outcome:** OpenBoxes fulfillment state is visible in nopCommerce within 30 seconds — no operator action. Carrier booking is fully automated. The full QAS set is structurally complete.

---

## Coexistence During Transition

| Component | Ph 1 | Ph 2 | Ph 3 | Ph 4 | Ph 5 |
| --- | --- | --- | --- | --- | --- |
| nopCommerce monolith (core unmodified) | Active | Active | Active | Active | Active |
| Inline `BasicPublish` in consumer | Active | **Removed** | | | |
| `Nop.Plugin.Messaging.RabbitMq` + outbox dispatcher | | Active | Active | Active | Active |
| RabbitMQ + `verdemart.orders` topology | Active | Active | Active | Active | Active |
| `verdemart.orders.openboxes` queue | Accumulating | Accumulating | **Drained** | Drained | Drained |
| Allocation gate + `/api/inventory/reserve` | | | Active | Active | Active |
| OpenBoxes Bridge container | | | Active | Active | Active |
| `Nop.Plugin.Shipping.CarrierWebhook` + WireMock | | | | Active | Active |
| `OpenBoxesStatusPollerTask` | | | | | Active |

The only breaking transition is from Phase 1 to Phase 2. Everything else is additive.
