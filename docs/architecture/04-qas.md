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
| Artifact | The order placement event and the downstream fulfilment request |
| Response | The order event is preserved and delivered to OpenBoxes when it recovers; no manual intervention is required |
| Response measure | Zero orders lost during an OpenBoxes outage of up to 30 minutes; order appears in OpenBoxes within 60 seconds of recovery |

---

## QAS-2 - Consistency: Last-unit oversell must not occur across channels

| Field | Value |
|---|---|
| Quality attribute | Consistency |
| Stimulus | Two orders for the last unit of a product are placed simultaneously - one from the web storefront, one from the POS |
| Source | nopCommerce web checkout and POS sale event |
| Environment | Normal operation, both systems active |
| Artifact | The inventory record for the product and the order processing pipeline |
| Response | Exactly one order succeeds; the other receives a stock-unavailable response; inventory never goes below zero |
| Response measure | Zero confirmed oversell events under concurrent load; the losing order is rejected within the same request cycle |

---

## QAS-3 - Availability: Checkout must complete when surrounding systems are slow

| Field | Value |
|---|---|
| Quality attribute | Availability |
| Stimulus | A customer completes checkout while a surrounding system (warehouse, ERP, or carrier) is responding slowly (>5 s per request) |
| Source | Any surrounding system under load or network degradation |
| Environment | nopCommerce is processing the order; surrounding system sync is part of the post-placement flow |
| Artifact | nopCommerce order placement and the customer-facing confirmation page |
| Response | The order is confirmed to the customer immediately; surrounding system synchronisation completes independently; the customer never waits on any surrounding system |
| Response measure | Checkout response time remains under 3 seconds regardless of surrounding system latency; no checkout failures attributable to surrounding system slowness |

---

## QAS-4 - Recoverability: Missed events must self-heal after outage

| Field | Value |
|---|---|
| Quality attribute | Recoverability |
| Stimulus | OpenBoxes recovers after a 30-minute outage during which 12 orders were placed |
| Source | OpenBoxes process restart |
| Environment | RabbitMQ was running throughout the outage; messages were durably queued |
| Artifact | The queued order events and the order records in OpenBoxes |
| Response | OpenBoxes processes all 12 held order events in order; all fulfilment tasks are created; nopCommerce order state is updated accordingly |
| Response measure | All orders placed during the outage are fully processed within 5 minutes of OpenBoxes recovery; no operator action required |

---

## QAS-5 - Visibility: External state changes must reach the customer quickly

| Field | Value |
|---|---|
| Quality attribute | Visibility (cross-channel state propagation) |
| Stimulus | A surrounding system changes the state of an order or shipment outside nopCommerce — either OpenBoxes marks a fulfillment order as `ISSUED`, or the carrier updates a shipment's tracking status |
| Source | OpenBoxes (warehouse) or carrier system (WireMock) |
| Environment | Normal operation; order has been placed and is progressing through the fulfillment lifecycle |
| Artifact | The order detail page in nopCommerce and the customer notification email |
| Response | nopCommerce detects the external state change and reflects it internally; the order status is updated; a notification email is queued |
| Response measure | Carrier tracking status and OpenBoxes fulfillment state both visible in nopCommerce within 30 seconds of the change occurring in the external system; email queued within the same window |
