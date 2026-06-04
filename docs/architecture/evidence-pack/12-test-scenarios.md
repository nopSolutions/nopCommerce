# Evidence Pack — Test Scenarios

**Scope:** Iterations 1–5.  
**Purpose:** Define the concrete test scenarios used to exercise the system. Each scenario names the QAS or architectural property it targets, the preconditions required, the stimulus applied, and the observable outcome expected. Results are recorded in `11-measurements.md`.

Scenarios are ordered from most critical (mandatory pressure point) to supporting evidence.

---

## TS-1 — Bridge Outage and Automatic Recovery

**Targets:** QAS-1 (Reliability), QAS-3 (Availability), QAS-4 (Recoverability)  
**Measurement:** M1 + M2  
**Status:** ✅ Run 2026-05-30  
**Why this is the mandatory pressure point:** The core architectural claim of VerdeMart is that the commerce core remains useful when surrounding systems are unavailable. This scenario directly challenges that claim by removing the warehouse integration layer entirely while customers continue to place orders.

### Preconditions

- All containers running (`docker compose up -d`)
- At least one product with `ManageInventoryMethodId = 1` and `StockQuantity ≥ 3`
- RabbitMQ Management open at `http://localhost:15672` → Queue `verdemart.orders.openboxes` showing `Ready=0`, `Consumers=1`

### Stimulus

1. Stop the bridge container (`docker stop verdemart_openboxes_bridge`) — simulates OpenBoxes becoming unreachable
2. Place 3 separate orders through the web storefront during the outage
3. Restart the bridge (`docker start verdemart_openboxes_bridge`) — simulates system recovery

### Observable outcomes

| Observation | When | What it proves |
| --- | --- | --- |
| Checkout completes immediately for each order | During outage (step 2) | QAS-3: the outbox decouples checkout from the bridge — the customer never waits on OpenBoxes |
| RabbitMQ queue depth rises to 3; `Consumers=0` throughout | During outage | QAS-1: messages persist durably on the broker; durable queue + persistent delivery (ADR-003) |
| Bridge logs show `Fulfillment created for OrderGuid=...` for all 3 orders | Within 60 s of bridge restart | QAS-1: zero orders lost; recovery is automatic |
| No operator action taken at any point | Throughout | QAS-4: the system self-heals |
| OpenBoxes shows 3 new stock movements | After recovery | End-to-end proof: order → outbox → RabbitMQ → bridge → OpenBoxes |

### Degradation path exercised

`bridge stopped → messages accumulate in durable queue → broker holds them on disk → bridge restarts → bridge reconnects and drains backlog → OpenBoxes receives all orders`

### Recovery path exercised

The bridge reconnects automatically (`AutomaticRecoveryEnabled=true`), consumes all pending messages in order, and acknowledges each after OpenBoxes confirms the fulfillment. No replay command is issued. No message is lost.

### Results (run 2026-05-30, evidence in M1 + M2)

| Metric | Required | Observed |
| --- | --- | --- |
| Orders lost during outage | 0 | ✅ 0 — queue reached `Ready=3, Persistent=3` while bridge was stopped |
| All 3 fulfillments in OpenBoxes after recovery | Within 60 s | ✅ All 3 `Fulfillment created` log lines within ≤ 60 s of bridge restart |
| Operator actions | 0 | ✅ 0 — bridge self-healed and drained automatically |
| DLQ messages | 0 | ✅ 0 |
| Checkout response time (during outage) | ≤ 3 s | ✅ 83–92 ms — 30× below threshold; outbox fully decouples customer path |

---

## TS-2 — Cross-Channel Oversell Prevention

**Targets:** QAS-2 (Consistency)  
**Measurement:** M3  
**Status:** ✅ Run 2026-05-30  
**Why this matters:** The scenario that motivated the entire Iteration 3 design — two channels competing for the same last unit. Without the allocation gate, both could succeed simultaneously, driving stock negative.

### Preconditions

- `Nop.Plugin.Inventory.AllocationGate` and `Nop.Plugin.Integration.Pos` installed and active
- Product HP Spectre XT Pro UltraBook (id=7, SKU=`HP_SPX_UB`) with `StockQuantity = 1`
- A POS system (simulated via `curl`) able to call `POST /api/inventory/reserve`

### Stimulus

1. POS calls `POST /api/inventory/reserve` with `productId=7, quantity=1` — simulating a customer at the till holding the last unit
2. Web customer navigates to the same product page and attempts to add to cart or complete checkout

### Observable outcomes

| Observation | What it proves |
| --- | --- |
| POS reserve returns HTTP 200 `{"message":"reserved"}` | Gate accepted the POS channel as the winner |
| Web storefront shows "Out of stock" on the product page | Effective availability (`StockQuantity − active_reservations = 0`) propagates to the display layer; web channel sees the product as unavailable before even reaching checkout |
| `StockQuantity` in the DB never goes below 1 | Zero oversell — the gate prevents decrement until `confirm` is called |
| POS release returns `{"released":true}` | Reservation lifecycle works correctly: reserve → hold → release |

### Degradation path exercised

`POS reserve → atomic UPDATE on Product.StockQuantity → StockQuantity = 0 → web channel sees "Out of stock"`

### Architecture note

Both web checkout and POS reach the **same** `IAllocationGate` via different entry points: the web channel through the `AllocationGateProductServiceDecorator` (which intercepts `IProductService.AdjustInventoryAsync`), and the POS channel through the HTTP adapter in `Nop.Plugin.Integration.Pos` (which delegates to the same `IAllocationGate`). A single authority prevents oversell regardless of channel.

### Results (run 2026-05-30 + 2026-05-31 + 2026-06-02, evidence in M3)

| Scenario | Winners | Losers | `StockQuantity` < 0? | QAS-2 satisfied? |
| --- | --- | --- | --- | --- |
| A: POS + web (primary, 1 unit) | 1 (POS HTTP 200) | 1 (web "Out of stock") | ✅ No | ✅ Yes |
| B: 5 concurrent POS requests (stress) | 1 (HTTP 200) | 4 (HTTP 409) | ✅ No | ✅ Yes — zero oversell holds; all losers receive structured `insufficient-stock` rejection |

The allocation gate blocks the web channel at the product listing level (before cart), not only at checkout confirm. `StockQuantity` remained ≥ 1 throughout both scenarios.

---

## TS-3 — At-Least-Once Delivery with Idempotent Consumer

**Targets:** ADR-003 (Durable queues + idempotent consumer)  
**Measurement:** M6  
**Status:** ✅ Run 2026-05-30  
**Why this matters:** The outbox guarantees at-least-once delivery. Under real conditions (broker restart, consumer crash mid-processing), a message may be delivered more than once. The bridge must absorb duplicates without creating duplicate fulfillments in OpenBoxes.

### Preconditions

- Bridge running and connected
- At least one `OrderGuid` already present in the bridge's `processed_orders` dedup table (i.e. at least one order previously fulfilled — available from TS-1)

### Stimulus

Re-publish the original `OrderPlacedMessage` for an already-processed `OrderGuid` directly to the `verdemart.orders.openboxes` queue via the RabbitMQ Management console (`http://localhost:15672` → Queues → Publish message).

### Observable outcomes

| Observation | What it proves |
| --- | --- |
| Bridge logs: `OrderGuid=... already processed — ack and skip` | Dedup table (`processed_orders`, keyed on `OrderGuid`) was consulted before calling OpenBoxes |
| No new stock movement appears in OpenBoxes | OpenBoxes API was not called — duplicate absorbed silently |
| Message acknowledged (queue returns to 0) | The message is not stuck or requeued; it is cleanly consumed |

### Architecture note

The dedup check happens **before** the OpenBoxes call in `OrderPlacedMessageConsumer.HandleAsync`. The message is ACK'd regardless of whether the OpenBoxes call is made — the duplicate is not requeued, it is discarded with an ACK. This ensures that redeliveries do not accumulate on the queue.

### Results (run 2026-05-30, evidence in M6)

Duplicate `OrderGuid=f69aee6c-adbe-4f1a-a3c7-7490ce5e5a93` re-published via RabbitMQ Management UI.

| Metric | Expected | Observed |
| --- | --- | --- |
| Bridge log | `already processed — ack and skip` | ✅ Exact match |
| New stock movements in OpenBoxes | 0 | ✅ 0 — OpenBoxes API never called |
| Message acknowledged | Yes (queue → 0) | ✅ ACK'd immediately; queue depth returned to 0 |

---

## TS-4 — Carrier Status Propagation via Scheduled Polling

**Targets:** QAS-5 — carrier half  
**Measurement:** M4  
**Status:** ✅ Run 2026-05-31

### Preconditions

- `Nop.Plugin.Shipping.CarrierTracking` installed and active
- A nopCommerce `Shipment` with `ExternalShipmentId` populated (format: `WIRE-XXXXX`) — this is set by `CarrierBookingConsumer` after calling WireMock
- WireMock running at `http://localhost:8090` with shipment state machine configured (`Started → IN_TRANSIT → OUT_FOR_DELIVERY → DELIVERED`)

### Stimulus

WireMock advances the shipment state on each poll (`GET /api/shipments/{id}/status`). The `CarrierStatusPollerTask` runs every 30 seconds and compares the returned status against the last-known `Shipment.ExternalShippingStatus`.

### Observable outcomes

| Observation | What it proves |
| --- | --- |
| `Shipment.ExternalShippingStatus` changes within ≤ 30 s of WireMock advancing state | QAS-5 carrier clause: status propagation bounded by poll interval |
| A `QueuedEmail` row is created within the same DB transaction as the status update | Customer notification is atomic with the status change |
| No operator action required | Polling is fully automated |

### Results (run 2026-05-31, evidence in M4)

Shipment `WIRE-15655` (Order #18). T₀ = 14:54:21 UTC.

| Transition | T_detected | Elapsed | ≤ 30 s? |
| --- | --- | --- | --- |
| `Started → IN_TRANSIT` | 14:54:33 | 12 s | ✅ |
| `IN_TRANSIT → OUT_FOR_DELIVERY` | 14:55:03 | 30 s | ✅ |
| `OUT_FOR_DELIVERY → DELIVERED` | 14:55:33 | 30 s | ✅ |

Three `QueuedEmail` rows (ids 39, 40, 41) created in the same second as the corresponding status writes. No operator action at any point. The 30 s worst-case detection latency equals the poll interval; all transitions satisfied the QAS-5 ≤ 30 s threshold.

---

## TS-5 — OpenBoxes Fulfillment State Propagation via Scheduled Polling

**Targets:** QAS-5 — warehouse half  
**Measurement:** M5  
**Status:** ✅ Run 2026-05-31

### Preconditions

- `Nop.Plugin.Fulfillment.OpenBoxes` installed and active
- A stock movement exists in OpenBoxes whose `description` field contains a nopCommerce `OrderGuid` (created by the bridge during TS-1)
- The `OpenBoxesStatusPollerTask` is running (30 s interval, confirmed via schedule task list)

### Stimulus

Operator manually advances the stock movement to `ISSUED` in OpenBoxes. The `OpenBoxesStatusPollerTask` polls `GET /api/generic/shipment?status=ISSUED` on its next tick.

### Observable outcomes

| Observation | What it proves |
| --- | --- |
| nopCommerce order `OrderStatusId` transitions to Complete within ≤ 30 s | QAS-5 warehouse clause: fulfillment state propagation bounded by poll interval |
| A `Shipment` row is created automatically in nopCommerce | Poller creates the shipment record without any admin action |
| A `carrier.booking.requested` row appears in `OutboxMessage` | Poller triggers the Iteration 4 carrier booking chain automatically |
| No operator action in nopCommerce required | Polling is fully automated; the QAS-5 "no operator action" clause is satisfied |

### Results (run 2026-05-31, evidence in M5)

Order #19 (`OrderGuid = D3C0C3EC-8C73-4B26-BE8D-5C497AEFC350`). T₀ = ~15:29:15 UTC (ISSUED set manually in OpenBoxes).

| Metric | Required | Observed |
| --- | --- | --- |
| `OrderStatusId = Complete` | ≤ 30 s from T₀ | ✅ ~18 s — detected at 15:29:33 UTC |
| `Shipment` row created automatically | Yes | ✅ Shipment #9, `ExternalShipmentId = WIRE-62343` |
| `carrier.booking.requested` outbox row | Yes | ✅ Written at 15:29:33.547; triggered full carrier booking chain |
| Operator actions in nopCommerce | 0 | ✅ 0 — poller handled everything |

**End-to-end cascade triggered automatically (no operator action at any step):**  
`OpenBoxes ISSUED` → `Order #19 Complete + Shipment #9` → `carrier.booking.requested` → `WIRE-62343 booked` → `IN_TRANSIT → OUT_FOR_DELIVERY → DELIVERED` (final state 15:31:03).

---

## TS-6 — Happy Path End-to-End (Baseline)

**Targets:** All QAS (baseline validation)  
**Measurement:** Pre-existing evidence + M1 happy path  
**Status:** ✅ Run 2026-05-30  
**Why this matters:** Before exercising pressure points, the system must work correctly under normal conditions. This scenario establishes the baseline that all other scenarios deviate from.

### Preconditions

- All containers running and healthy
- At least one product with a non-empty `Sku` and `StockQuantity ≥ 1`

### Stimulus

A customer places a single order through the web storefront with no external systems under stress.

### Observable outcomes

| Step | Observation | Mechanism |
| --- | --- | --- |
| Checkout | Order confirmation page appears in < 3 s | Outbox write is the only synchronous DB operation |
| Within 1 s | `OutboxMessage` row with `Status=Sent` in DB | `OutboxDispatcherTask` (1 s poll) publishes to RabbitMQ |
| Within seconds | Bridge logs `Fulfillment created for OrderGuid=...` | Bridge consumes, deduplicates, calls OpenBoxes |
| OpenBoxes | Stock movement appears under Outbound → List Outbound Movements | OpenBoxes received the fulfillment request |
| OpenBoxes | Product with SKU from the order visible under Products | Bridge auto-provisioned product (and category/location if first order) |

### Architecture note

The product, category, and destination location in OpenBoxes are created on demand by the bridge if they do not yet exist. This lazy provisioning means a fresh OpenBoxes instance does not require any manual seeding before the first order can be fulfilled.

### Results (run 2026-05-30, confirmed in M1 setup)

All steps observed as expected. Checkout < 3 s. `OutboxMessage` dispatched within 1 s. Bridge fulfilled all orders; stock movements appeared in OpenBoxes. Product, category, and destination location auto-provisioned on first order (subsequent orders reused them). No errors at any step.

---

## Scenario Coverage Summary

All six scenarios were executed. Each scenario body above contains a **Results** section with the concrete outcomes observed. Full measurement data, procedures, screenshots, and pass criteria are in `11-measurements.md`.

| Scenario | QAS / property | Pressure applied | Measurement | Outcome |
| --- | --- | --- | --- | --- |
| TS-1 — Bridge outage + recovery | QAS-1, QAS-3, QAS-4 | Bridge stopped during 3 orders | M1 + M2 | ✅ 0 lost; drain ≤ 60 s; checkout 83–92 ms |
| TS-2 — Cross-channel oversell | QAS-2 | POS reserves last unit; web attempts checkout | M3 | ✅ 0 oversell; web blocked; clean 409 in all scenarios |
| TS-3 — Idempotent redelivery | ADR-003 | Duplicate message published to live queue | M6 | ✅ `ack and skip`; 0 new OpenBoxes movements |
| TS-4 — Carrier status propagation | QAS-5 carrier | WireMock state machine advances on poll | M4 | ✅ All 3 transitions ≤ 30 s; emails queued atomically |
| TS-5 — Warehouse fulfillment state | QAS-5 warehouse | OpenBoxes ISSUED; poller detects | M5 | ✅ Complete in ~18 s; Shipment + outbox row created automatically |
| TS-6 — Happy path baseline | All QAS | None — normal operation | M1 setup | ✅ Checkout < 3 s; end-to-end chain confirmed |
