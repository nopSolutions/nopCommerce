# Target Architecture

**VerdeMart Omnichannel Commerce Core — Iterations 1–5**

---

## System Overview

```mermaid
graph TB
    subgraph CUSTOMER["Customer"]
        BROWSER["Browser / Order Page"]
    end

    subgraph POS["POS Terminal"]
        CURL["POS System\n(curl surrogate)"]
    end

    subgraph NOP["nopCommerce Process"]
        direction TB
        CORE["nopCommerce Core\nOrderProcessingService"]

        subgraph PLUGIN_MSG["Nop.Plugin.Messaging.RabbitMq"]
            CONSUMER["OrderPlacedConsumer"]
            OUTBOX["Outbox Table"]
            DISPATCHER["OutboxDispatcherTask\n(every 1s)"]
        end

        subgraph PLUGIN_GATE["Nop.Plugin.Inventory.AllocationGate"]
            GATE["AllocationGate\n(pessimistic row lock)"]
            API_INV["POST /api/inventory/reserve\nconfirm / release"]
            RELEASE["ReleaseExpiredReservationsTask\n(every 30s)"]
            POLLER["OpenBoxesStatusPollerTask\n(every 30s)"]
        end

        subgraph PLUGIN_CARRIER["Nop.Plugin.Shipping.CarrierWebhook"]
            WEBHOOK_IN["POST /api/carrier/webhook\n(bearer auth + audit)"]
            STATUS_CONSUMER["CarrierStatusConsumer"]
            BOOKING_CONSUMER["CarrierBookingConsumer"]
        end
    end

    subgraph INFRA["Infrastructure"]
        RABBIT["RabbitMQ\nverdemart.orders exchange\nverdemart.carrier.* exchanges"]
        MSSQL["MSSQL\nnopCommerce DB\n+ Outbox + Reservations"]
    end

    subgraph BRIDGE["VerdeMart.OpenBoxesBridge\n(separate Docker container)"]
        BRIDGE_CONSUMER["OrderPlacedMessageConsumer"]
        DEDUP["DedupRepository\n(processed_orders)"]
        OB_CLIENT_BRIDGE["IOpenBoxesClient"]
    end

    subgraph OPENBOXES["OpenBoxes\n(Warehouse)"]
        OB_API["REST API\n/api/generic/shipment"]
    end

    subgraph WIREMOCK["WireMock\n(Carrier Simulator)"]
        WM_BOOKING["POST /booking"]
        WM_WEBHOOK["fires shipment.status.updated"]
    end

    BROWSER -->|places order| CORE
    CURL -->|HTTP reserve/confirm/release| API_INV

    CORE --> GATE
    CORE --> CONSUMER
    CONSUMER --> OUTBOX
    OUTBOX --> DISPATCHER
    DISPATCHER -->|publishes OrderPlacedMessage| RABBIT

    API_INV --> GATE
    GATE --> MSSQL
    RELEASE --> MSSQL

    RABBIT -->|verdemart.orders.openboxes| BRIDGE_CONSUMER
    BRIDGE_CONSUMER --> DEDUP
    BRIDGE_CONSUMER --> OB_CLIENT_BRIDGE
    OB_CLIENT_BRIDGE -->|creates fulfillment order| OB_API

    POLLER -->|GET /api/generic/shipment?status=ISSUED| OB_API
    POLLER -->|updates order status + writes outbox row| MSSQL

    MSSQL --> BOOKING_CONSUMER
    BOOKING_CONSUMER -->|books shipment| WM_BOOKING
    WM_BOOKING -->|returns ExternalShipmentId| BOOKING_CONSUMER
    BOOKING_CONSUMER --> MSSQL

    WM_WEBHOOK -->|POST /api/carrier/webhook| WEBHOOK_IN
    WEBHOOK_IN --> STATUS_CONSUMER
    STATUS_CONSUMER -->|updates status + email| MSSQL
    MSSQL -->|order page updated| BROWSER
```

---

## Use Case 1 — Buy Online / Fulfill Through Another Channel

```mermaid
sequenceDiagram
    actor Customer
    participant nopCommerce
    participant Outbox
    participant RabbitMQ
    participant Bridge as OpenBoxes Bridge
    participant OpenBoxes

    Customer->>nopCommerce: Place order (checkout)
    nopCommerce->>nopCommerce: AllocationGate — reserve stock (pessimistic lock)
    nopCommerce->>Outbox: Write OrderPlacedMessage row (same DB transaction)
    nopCommerce-->>Customer: Order confirmed (instant)

    Note over nopCommerce,Outbox: Broker can be down — order is never lost

    Outbox->>RabbitMQ: OutboxDispatcherTask publishes (≤1s)
    RabbitMQ->>Bridge: Deliver to verdemart.orders.openboxes
    Bridge->>Bridge: Dedup check on OrderGuid
    Bridge->>OpenBoxes: POST — create fulfillment order
    OpenBoxes-->>Bridge: Fulfillment order created
    Bridge->>RabbitMQ: ACK message
```

---

## Use Case 2 — Cross-Channel State Visibility

```mermaid
sequenceDiagram
    participant OpenBoxes
    participant Poller as OpenBoxesStatusPollerTask
    participant nopCommerce
    participant Outbox
    participant Booking as CarrierBookingConsumer
    participant WireMock as WireMock (Carrier)
    participant Webhook as CarrierWebhook Plugin
    actor Customer

    Note over OpenBoxes: Warehouse picks & packs → ISSUED

    loop every 30 seconds
        Poller->>OpenBoxes: GET /api/generic/shipment?status=ISSUED
        OpenBoxes-->>Poller: [ { OrderGuid, status: ISSUED } ]
        Poller->>nopCommerce: Update order status → Complete
        Poller->>Outbox: Write carrier.booking.requested row
    end

    Outbox->>Booking: OutboxDispatcherTask publishes
    Booking->>WireMock: POST /booking (book shipment)
    WireMock-->>Booking: { trackingId, ExternalShipmentId }
    Booking->>nopCommerce: Save ExternalShipmentId on Shipment

    Note over WireMock: Carrier ships package → fires webhook

    WireMock->>Webhook: POST /api/carrier/webhook (status updated)
    Webhook->>Webhook: Auth + audit + async queue handoff
    Webhook-->>WireMock: 200 OK (instant)
    Webhook->>nopCommerce: Update ExternalShippingStatus + enqueue email
    nopCommerce-->>Customer: Order page shows tracking status (≤10s)
```

---

## POS Stock Contention (QAS-2)

```mermaid
sequenceDiagram
    actor WebCustomer as Web Customer
    actor POSTerminal as POS Terminal
    participant Gate as AllocationGate\n(nopCommerce)
    participant DB as ProductWarehouseInventory

    par Web checkout and POS sale — simultaneous, last unit
        WebCustomer->>Gate: AdjustInventoryAsync (decrement)
        Gate->>DB: SELECT FOR UPDATE (pessimistic lock)
        DB-->>Gate: Lock acquired — 1 unit available
        Gate->>DB: Insert ProductReservation
        Gate-->>WebCustomer: ✓ Reserved — order proceeds

    and
        POSTerminal->>Gate: POST /api/inventory/reserve
        Gate->>DB: SELECT FOR UPDATE (waits for lock)
        DB-->>Gate: Lock acquired — 0 units available
        Gate-->>POSTerminal: 409 Conflict — stock unavailable
    end
```

---

## Quality Attribute Traceability

| QAS | Mechanism | ADRs |
| --- | --- | --- |
| QAS-1 Reliability — order survives OpenBoxes outage | Transactional outbox + durable queues + idempotent bridge consumer | ADR-003, ADR-004, ADR-005, ADR-009 |
| QAS-2 Consistency — zero oversell web + POS | Pessimistic row lock + shared allocation gate | ADR-006, ADR-007 |
| QAS-3 Availability — checkout <3s under surrounding system slowness | Outbox decouples all surrounding systems from checkout thread | ADR-001, ADR-004 |
| QAS-4 Recoverability — backlog drains after consumer outage | Durable queue + idempotent consumer + automatic redelivery | ADR-003, ADR-009 |
| QAS-5 Visibility — warehouse state ≤30s, carrier tracking ≤10s | OpenBoxes polling task + carrier webhook plugin | ADR-011, ADR-014, ADR-015 |

---

## ADR Index

| ADR | Decision |
| --- | --- |
| ADR-001 | RabbitMQ as message broker |
| ADR-002 | Plugin as integration boundary |
| ADR-003 | Durable queues + manual acknowledgement |
| ADR-004 | Transactional outbox |
| ADR-005 | Outbox dispatcher via IScheduleTask |
| ADR-006 | Allocation gate inside nopCommerce |
| ADR-007 | Cross-channel allocation via synchronous HTTP |
| ADR-008 | OpenBoxes bridge as separate deployable |
| ADR-009 | Idempotent consumer + DLQ |
| ADR-010 | Versioned wire contract with tolerant readers |
| ADR-011 | Webhook ingestion via async queue handoff |
| ADR-012 | External shipment correlation via ExternalShipmentId |
| ADR-013 | External status preserved as string |
| ADR-014 | Outbound carrier booking via existing outbox |
| ADR-015 | OpenBoxes fulfillment state via scheduled polling |
