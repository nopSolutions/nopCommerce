# Quality Attribute Scenarios

Each scenario is written as: **Stimulus → Source → Environment → Artifact → Response → Response Measure**

---

## QAS-1 - Reliability: Order event must survive a warehouse outage

| Field | Value |
|---|---|
| Quality attribute | Reliability |
| Stimulus | A customer places an order on the web storefront |
| Source | nopCommerce checkout |
| Environment | OpenBoxes is temporarily unavailable (process down or network partition) |
| Artifact | The `order.placed` message published to RabbitMQ |
| Response | The message is durably queued; OpenBoxes consumes and processes it when it recovers; no manual intervention is required |
| Response measure | Zero orders lost during an OpenBoxes outage of up to 30 minutes; order appears in OpenBoxes within 60 seconds of recovery |

**Design decision forced:** The nopCommerce → OpenBoxes integration must use a durable message queue (RabbitMQ with persistent queues and acknowledgements), not a direct HTTP call. Direct HTTP fails silently or blocks the checkout thread.

---

## QAS-2 - Consistency: Last-unit oversell must not occur across channels

| Field | Value |
|---|---|
| Quality attribute | Consistency |
| Stimulus | Two orders for the last unit of a product are placed simultaneously - one from the web storefront, one from the POS |
| Source | nopCommerce web checkout and POS sale event |
| Environment | Normal operation, both systems active |
| Artifact | `StockQuantity` in nopCommerce and the fulfillment queue in OpenBoxes |
| Response | Exactly one order succeeds; the other receives a stock-unavailable response; inventory never goes below zero |
| Response measure | Zero confirmed oversell events under concurrent load; the losing order is rejected within the same request cycle |

**Design decision forced:** Inventory decrement at checkout must be protected by a serializable database transaction or a per-product lock. The current `AdjustInventoryAsync` path has no such gate - it must be added.

---

## QAS-3 - Availability: Checkout must complete when ERPNext is slow

| Field | Value |
|---|---|
| Quality attribute | Availability |
| Stimulus | A customer completes checkout while ERPNext is responding slowly (>5 s per request) |
| Source | ERPNext under load or network degradation |
| Environment | nopCommerce is processing the order; ERPNext sync is part of the post-placement flow |
| Artifact | nopCommerce order placement and the customer-facing confirmation page |
| Response | The order is confirmed to the customer immediately; ERPNext synchronisation happens asynchronously via RabbitMQ; the customer never waits on ERPNext |
| Response measure | Checkout response time remains under 3 seconds regardless of ERPNext latency; no checkout failures attributable to ERPNext slowness |

**Design decision forced:** ERPNext must never be in the synchronous checkout path. The `IConsumer<OrderPlacedEvent>` that notifies ERPNext must publish to RabbitMQ and return immediately. Any blocking HTTP call to ERPNext during checkout is an architectural violation.

---

## QAS-4 - Recoverability: Missed events must self-heal after outage

| Field | Value |
|---|---|
| Quality attribute | Recoverability |
| Stimulus | OpenBoxes recovers after a 30-minute outage during which 12 orders were placed |
| Source | OpenBoxes process restart |
| Environment | RabbitMQ was running throughout the outage; messages were durably queued |
| Artifact | The fulfillment order queue in RabbitMQ and the order records in OpenBoxes |
| Response | OpenBoxes consumes the 12 queued `order.placed` messages in order; all fulfillment tasks are created; nopCommerce order state is updated accordingly |
| Response measure | All orders placed during the outage are fully processed within 5 minutes of OpenBoxes recovery; no operator action required |

**Design decision forced:** RabbitMQ queues must be declared as durable with manual acknowledgement. OpenBoxes consumption must be idempotent - replaying a message for an already-created fulfillment order must not create a duplicate.

---

## QAS-5 - Visibility: Carrier tracking update must reach the customer quickly

| Field | Value |
|---|---|
| Quality attribute | Visibility (cross-channel state propagation) |
| Stimulus | WireMock (carrier) sends a `shipment.status.updated` webhook - status changes to "In Transit" |
| Source | Carrier system (WireMock) |
| Environment | Normal operation; shipment has been dispatched from OpenBoxes |
| Artifact | The order detail page in nopCommerce and the customer notification email |
| Response | nopCommerce receives and processes the webhook; the tracking status is updated in the order record; a notification email is queued |
| Response measure | Order tracking status visible to the customer within 10 seconds of the webhook being received; email queued within the same window |

**Design decision forced:** nopCommerce must expose a webhook endpoint that accepts carrier callbacks, verifies the payload, and maps the external shipment status to the internal `ShippingStatus`. The `Shipment` entity needs an `ExternalShipmentId` field to correlate the incoming webhook to the correct order.
