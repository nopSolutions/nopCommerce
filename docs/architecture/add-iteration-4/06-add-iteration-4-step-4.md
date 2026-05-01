# ADD Iteration 4 — Step 4: Instantiate Elements and Allocate Responsibilities

## What This Step Does

Step 4 turns the eight design concepts from Step 3 into concrete named components. Each gets a name, a type, a location in the codebase, and a defined set of responsibilities. The iteration produces a single new plugin (`Nop.Plugin.Shipping.CarrierWebhook`) plus schema additions on the existing `Shipment` entity and new RabbitMQ topology. No new deployable is introduced.

---

## Plugin Identity

| Property | Value |
| --- |---|
| Plugin name | `Nop.Plugin.Shipping.CarrierWebhook` |
| Location | `src/Plugins/Nop.Plugin.Shipping.CarrierWebhook/` |
| Type | nopCommerce plugin (`IPlugin` via `BasePlugin`) |
| Naming convention | Matches existing convention (`Nop.Plugin.Shipping.*`) |

---

## Schema Additions

### Update to existing `Shipment` (in `Nop.Core.Domain.Shipping`)

Four new columns, all nullable so existing rows are unaffected:

| Column | Type | Notes |
| --- |---| --- |
| `ExternalShipmentId` | `varchar(128)`, nullable, indexed | Carrier-issued tracking identifier; correlation key for inbound webhooks |
| `ExternalCarrierCode` | `varchar(32)`, nullable | E.g. `"WIREMOCK"`. Future-proofs multi-carrier even though only one is wired now |
| `ExternalShippingStatus` | `varchar(64)`, nullable | Carrier vocabulary verbatim, e.g. `"OUT_FOR_DELIVERY"` |
| `LastStatusOccurredAtUtc` | `datetime(6)`, nullable | Timestamp of the most recent applied status update; used by the out-of-order guard |

Index: `IX_Shipment_ExternalShipmentId` on `(ExternalCarrierCode, ExternalShipmentId)` — used by the inbound consumer's correlation lookup.

The schema is added by a FluentMigrator migration inside the plugin. The columns are added to the existing `Shipment` table — no parallel side table — because the data is intrinsic to the shipment, not metadata about it.

---

### New table: `CarrierWebhookEvent`

Audit log for every webhook receipt:

| Column | Type | Notes |
| --- |---| --- |
| `Id` | `int`, PK, identity | inherited via `BaseEntity` |
| `EventId` | `varchar(64)`, nullable, indexed | Carrier's UUID; null only when the payload could not be parsed |
| `ReceivedAtUtc` | `datetime(6)`, not null | Server clock at controller entry |
| `RawPayload` | `text`, not null | Full JSON body as received |
| `RemoteIp` | `varchar(45)`, not null | IPv4/IPv6, for triage |
| `Outcome` | `tinyint`, not null | `0=Accepted, 1=Duplicate, 2=Rejected, 3=Unmatched` |
| `Notes` | `varchar(256)`, nullable | Short reason on `Rejected`/`Unmatched`; null otherwise |

Index on `(ReceivedAtUtc DESC)` for operator queries; index on `EventId` for cross-referencing the dedup table during disputes.

---

### New table: `ProcessedCarrierEvent`

Dedup state for the inbound consumer:

| Column | Type | Notes |
| --- |---| --- |
| `EventId` | `varchar(64)`, PK | Carrier's UUID |
| `ProcessedAtUtc` | `datetime(6)`, not null | When the consumer applied the event |
| `ShipmentId` | `int`, not null | Convenience for joining; not a strict FK to keep the table append-only |

Both new tables are created by the same FluentMigrator migration as the `Shipment` columns.

---

## RabbitMQ Topology Additions

| Element | Name | Properties |
| --- |---| --- |
| Exchange | `verdemart.carrier.booking` | direct, durable, auto-delete: false |
| Queue | `verdemart.carrier.booking.requested` | durable, manual ack |
| Binding | queue ← exchange | routing key: `carrier.booking.requested` |
| Exchange | `verdemart.carrier.status` | direct, durable, auto-delete: false |
| Queue | `verdemart.carrier.status.received` | durable, manual ack, `x-dead-letter-exchange = verdemart.carrier.status.dlx` |
| Binding | queue ← exchange | routing key: `carrier.status.received` |
| Exchange | `verdemart.carrier.status.dlx` | direct, durable |
| Queue | `verdemart.carrier.status.dlq` | durable, no auto-delete |
| Binding | DLQ ← DLX | routing key: `carrier.status.received` |

The plugin declares all of the above on startup so it owns its own consumption topology — same pattern used by the OpenBoxes bridge (ADR-008's spirit, adapted for in-process). The booking flow does not need a DLQ for QAS-5 — booking failures land in the existing Outbox retry mechanics from Iter 2; if WireMock is permanently misconfigured, the outbox row is the operator-visible signal.

---

## Components

### 1. `CarrierWebhookPlugin`

**Type:** `BasePlugin` implementation
**File:** `CarrierWebhookPlugin.cs`

**Responsibilities:**
- Plugin entry point — implements `IPlugin`
- On install: run the FluentMigrator migration; declare the RabbitMQ topology; register the message-template token (if a new email template is added)
- On uninstall: remove the schedule-task entries (none in this plugin) and any plugin-installed message tokens; the audit table and dedup table are left in place for forensic continuity

---

### 2. `CarrierWebhookEvent` (entity) and `ProcessedCarrierEvent` (entity)

**Type:** Entities mapped to the new tables
**Files:** `Domain/CarrierWebhookEvent.cs`, `Domain/ProcessedCarrierEvent.cs`

**Responsibilities:**
- Inherit `BaseEntity`
- Mirror the table schemas
- `WebhookOutcome` enum: `Accepted=0, Duplicate=1, Rejected=2, Unmatched=3`

---

### 3. `CarrierWebhookController`

**Type:** ASP.NET Core controller exposing the inbound endpoint
**File:** `Controllers/CarrierWebhookController.cs`
**Route:** `POST /api/carrier/webhook`

**Responsibilities:**
- Read `Authorization` header, compare to configured token using a constant-time comparison; on mismatch, write an audit row with `Outcome=Rejected, Notes="auth"` and return `401`
- Read body, attempt to deserialise into `CarrierStatusPayload`; on parse failure, write `Outcome=Rejected, Notes="malformed"` and return `400`
- Write the audit row with `Outcome=Accepted` (provisional — final outcome may be amended by the consumer's later action; the audit table captures *receipt*, not *processing result*)
- Publish a `CarrierStatusReceivedMessage` to `verdemart.carrier.status` on routing key `carrier.status.received`
- Return `200 OK` with an empty body
- Does not call any business logic synchronously; processing is owned by the consumer

---

### 4. `CarrierStatusConsumer`

**Type:** RabbitMQ consumer running as `BackgroundService`
**File:** `Consumers/CarrierStatusConsumer.cs`

**Responsibilities:**
- Subscribes to `verdemart.carrier.status.received` with `autoAck: false`
- For each message:
  1. Deserialise into `CarrierStatusReceivedMessage`
  2. Check `IProcessedCarrierEventRepository.HasProcessedAsync(eventId)` — if true, ack and skip (return `Duplicate` outcome to audit log via `ICarrierWebhookAuditService.AmendOutcomeAsync`)
  3. Look up `Shipment` by `(ExternalCarrierCode, ExternalShipmentId)`. If null:
     - Increment redelivery count from `x-death` header
     - If under threshold (default 5), NACK with requeue
     - If over threshold, NACK without requeue (routes to DLQ); amend audit outcome to `Unmatched`
  4. If `payload.OccurredAtUtc <= shipment.LastStatusOccurredAtUtc`, ack and skip — older event, no state change
  5. Otherwise, in one DB transaction:
     - Update `Shipment.ExternalShippingStatus`, `LastStatusOccurredAtUtc`, and (via `IExternalStatusMapper`) the internal `ShippingStatus` if a transition is implied
     - Insert `ProcessedCarrierEvent` row
     - Enqueue customer notification via `IWorkflowMessageService.SendShipmentStatusUpdatedNotificationAsync` (a new message type; details below)
  6. Ack
- Catches all exceptions; transient failures NACK with requeue; permanent (e.g. DB unique-constraint after concurrent dedup race) NACK without requeue

---

### 5. `IExternalStatusMapper` / `ExternalStatusMapper`

**Type:** Pure-function service
**File:** `Services/ExternalStatusMapper.cs`

**Responsibilities:**
- `MapToInternal(string externalStatus) → ShippingStatus?` — returns `Delivered` for `"DELIVERED"`, `Shipped` for `"PICKED_UP" | "IN_TRANSIT" | "OUT_FOR_DELIVERY"`, and `null` for all others (no internal transition implied)
- The mapping is hard-coded per Step 3 concept 4
- Stateless; registered as singleton

---

### 6. `ICarrierWebhookAuditService` / `CarrierWebhookAuditService`

**Type:** Service over the audit table
**File:** `Services/CarrierWebhookAuditService.cs`

**Responsibilities:**
- `RecordReceiptAsync(rawBody, remoteIp, eventId?, outcome, notes?) → Id` — used by the controller
- `AmendOutcomeAsync(auditId, outcome, notes?)` — used by the consumer to mark a receipt that was provisionally `Accepted` as `Duplicate` or `Unmatched` after the asynchronous processing decides
- `AmendOutcomeAsync` writes a new row rather than updating in place if the audit table is configured append-only (default: in-place update for simplicity; append-only is a residual decision)

---

### 7. `IProcessedCarrierEventRepository` / `ProcessedCarrierEventRepository`

**Type:** Data access over `IRepository<ProcessedCarrierEvent>`
**File:** `Repositories/ProcessedCarrierEventRepository.cs`

**Responsibilities:**
- `HasProcessedAsync(eventId)` — `SELECT 1 FROM ProcessedCarrierEvent WHERE EventId = @e`
- `RecordProcessedAsync(eventId, shipmentId)` — INSERT inside the ambient transaction; the PK constraint catches concurrent races

---

### 8. `ShipmentSentEventConsumer`

**Type:** `IConsumer<ShipmentSentEvent>` — outbound trigger
**File:** `Consumers/ShipmentSentEventConsumer.cs`

**Responsibilities:**
- Listens for the existing `ShipmentSentEvent`
- Builds a `CarrierBookingRequestedMessage` from the shipment + order + address
- Inserts an Outbox row with `EventType="carrier.booking.requested"` and the JSON payload
- Returns immediately; the existing `OutboxDispatcherTask` (Iter 2) handles publishing

The consumer uses **only the existing Iter 2 Outbox primitives** — no new dispatcher, no new schedule task.

---

### 9. `CarrierBookingConsumer`

**Type:** RabbitMQ consumer running as `BackgroundService`
**File:** `Consumers/CarrierBookingConsumer.cs`

**Responsibilities:**
- Subscribes to `verdemart.carrier.booking.requested` with `autoAck: false`
- For each message:
  1. Deserialise to `CarrierBookingRequestedMessage`
  2. Look up the `Shipment` row; if it already has `ExternalShipmentId`, ack (idempotent — booking happened on a previous delivery)
  3. Call `IWireMockClient.BookShipmentAsync(...)`; on transient failure (timeout, 5xx), NACK with requeue
  4. On success, in a DB transaction: update `Shipment.ExternalShipmentId` and `ExternalCarrierCode`; commit; ack
  5. On permanent failure (4xx), NACK without requeue → DLQ (booking flow uses its own DLQ following the same pattern as the status flow; topology mirrored for symmetry)

---

### 10. `IWireMockClient` / `WireMockClient`

**Type:** Typed HTTP client for WireMock
**File:** `Carriers/WireMockClient.cs`

**Responsibilities:**
- `BookShipmentAsync(BookingRequest) → BookingResult` — POSTs to WireMock's `/shipments` endpoint, parses the response, returns the carrier tracking ID
- Owns retry-with-backoff for transient HTTP errors (3 in-call retries with jitter); broader retry handled by message redelivery
- Reads `WireMockBaseUrl` and credentials from `CarrierWebhookSettings`
- The WireMock contract (request/response shape) is defined by the plugin and configured into WireMock at deploy time; exact JSON fixed in Step 5

---

### 11. `CarrierWebhookSettings`

**Type:** `ISettings` implementation
**File:** `CarrierWebhookSettings.cs`

**Fields:**

| Field | Default | Notes |
| --- |---| --- |
| `InboundBearerToken` | env-driven | Token expected on inbound webhooks |
| `WireMockBaseUrl` | env-driven | E.g. `http://wiremock:8080` |
| `WireMockTimeoutMs` | `3000` | Per-call timeout for booking |
| `MaxStatusRedeliveries` | `5` | Above this → DLQ on the inbound status path |
| `MaxBookingRedeliveries` | `5` | Above this → DLQ on the outbound booking path |
| `CarrierCode` | `"WIREMOCK"` | Stored on bookings to populate `ExternalCarrierCode` |

---

### 12. `PluginNopStartup`

**Type:** `INopStartup` implementation
**File:** `Infrastructure/PluginNopStartup.cs`

**Responsibilities:**
- Registers `IExternalStatusMapper`, `ICarrierWebhookAuditService`, `IProcessedCarrierEventRepository`, `IWireMockClient`
- Registers `ShipmentSentEventConsumer` as `IConsumer<ShipmentSentEvent>`
- Registers `CarrierStatusConsumer` and `CarrierBookingConsumer` as `IHostedService` (background services)
- Declares the RabbitMQ topology on startup
- `Order` set after the Iter 1 plugin (3000) and after the Iter 3 plugin (3500); chosen value `4000`

---

### 13. New email template — `ShipmentStatusUpdatedNotification`

**Type:** Plugin-installed `MessageTemplate`
**File:** registered via the plugin's `InstallAsync`

**Responsibilities:**
- New template token: `%Shipment.ExternalStatus%` resolves to the carrier vocabulary string
- Existing tokens (`%Order.OrderNumber%`, `%Customer.FullName%`) reused
- The plugin reuses `IWorkflowMessageService` infrastructure; the template is installed alongside on plugin install and removed on uninstall

A new template (rather than reusing `OrderShipped.CustomerNotification`) is needed because the customer should receive an update on every meaningful status transition, not just initial dispatch. `OrderShipped` is fired once.

---

## Wire Contracts

### Outbox event payload — `CarrierBookingRequestedMessage`

```
{
  "shipmentId": int,
  "orderId":    int,
  "shippingAddress": {
    "addressLine1": string,
    "city":         string,
    "postalCode":   string,
    "countryCode":  string,
    "recipientName": string
  },
  "items": [
    { "sku": string, "quantity": int, "weightKg": decimal }
  ],
  "version": 1
}
```

The `version` field follows ADR-010's policy.

### Inbound webhook payload — `CarrierStatusPayload`

```
{
  "eventId":          string (UUID),
  "carrierTrackingId": string,
  "carrierCode":       string,
  "status":            string  // BOOKED|PICKED_UP|IN_TRANSIT|OUT_FOR_DELIVERY|DELIVERED|EXCEPTION|RETURNED
  "statusDescription": string,
  "occurredAtUtc":     string (ISO 8601),
  "location":          string?  // optional
}
```

### Internal queue message — `CarrierStatusReceivedMessage`

The same payload re-published verbatim onto `verdemart.carrier.status` plus a `receivedAtUtc` field added by the controller. ADR-010's `version` field applies.

### WireMock booking request/response

Defined in Step 5.

---

## Component Relationships

```
[ Outbound — admin "create shipment" flow ]

ShipmentSentEvent (existing)
    │
    ▼
ShipmentSentEventConsumer  ──────────▶ Outbox table (Iter 2 — unchanged)
                                              │
                                              │ OutboxDispatcherTask (Iter 2)
                                              ▼
                                      RabbitMQ
                                      verdemart.carrier.booking
                                              │
                                              ▼
                                  CarrierBookingConsumer (BackgroundService)
                                       │
                                       ├── IWireMockClient → POST /shipments → WireMock
                                       │
                                       └── update Shipment.ExternalShipmentId + ExternalCarrierCode

[ Inbound — carrier status update ]

WireMock ─── HTTP POST /api/carrier/webhook ───▶ CarrierWebhookController
                                                       │
                                                       ├── ICarrierWebhookAuditService.RecordReceiptAsync
                                                       │
                                                       └── publish to verdemart.carrier.status
                                                                       │
                                                                       ▼
                                                          CarrierStatusConsumer (BackgroundService)
                                                                ├── IProcessedCarrierEventRepository (dedup)
                                                                ├── lookup Shipment by ExternalShipmentId
                                                                ├── out-of-order guard
                                                                ├── IExternalStatusMapper
                                                                ├── update Shipment + INSERT ProcessedCarrierEvent
                                                                └── IWorkflowMessageService.SendShipmentStatusUpdatedNotification
```

All new code lives inside `Nop.Plugin.Shipping.CarrierWebhook/`. The only `Nop.Core` change is the four columns on `Shipment`, added by FluentMigrator from the plugin.

---

## What Step 5 Will Do

Step 5 defines the precise interfaces — method signatures, data types, configuration surfaces, and HTTP wire formats. Specific contracts to settle:

- The exact `CarrierStatusConsumer` and `CarrierBookingConsumer` method signatures and outcome types
- The `/api/carrier/webhook` request body, headers, and response shape
- The `IWireMockClient.BookShipmentAsync` request/response JSON
- The exact `INopStartup.ConfigureServices` registration block including `IHostedService` registration order
- The new `ShipmentStatusUpdatedNotification` message-template tokens
