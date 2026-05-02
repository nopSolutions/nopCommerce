# ADD Iteration 5 — Step 4: Instantiate Elements and Allocate Responsibilities

## Plugin

The polling task is added to the existing `Nop.Plugin.Inventory.AllocationGate` plugin. No new plugin is introduced — the task is a natural extension of the allocation gate's role as the warehouse-visibility boundary inside nopCommerce.

---

## New Components

### 1. `OpenBoxesStatusPollerTask`

**Type:** `IScheduleTask` implementation
**File:** `ScheduleTasks/OpenBoxesStatusPollerTask.cs`

**Responsibilities:**
- Runs on a configurable interval (default 30 s)
- Calls `IOpenBoxesClient.GetIssuedFulfillmentOrdersAsync()` — returns all fulfillment orders in `ISSUED` state since last checked
- For each result: look up the nopCommerce order by `OrderGuid`; if found and not already marked fulfilled:
  1. Create a `Shipment` record via `IShipmentService.InsertShipmentAsync` (with `ShipmentItem` rows for all order items) — this fires `ShipmentCreatedEvent` and gives the carrier booking a row to write `ExternalShipmentId` back to
  2. Call `IOrderProcessingService.SetOrderStatusAsync(OrderStatus.Complete)` to transition the order status
  3. Write an outbox row with `EventType = "carrier.booking.requested"` and the new `ShipmentId` in the payload
- All three writes (`InsertShipmentAsync`, `SetOrderStatusAsync`, outbox row insert) execute inside a single DB transaction — if any step fails, all roll back and the next tick retries cleanly
- Idempotent: if the order already has a `Shipment` with `ExternalShipmentId` set (carrier booking already confirmed), skips; a `Shipment` without `ExternalShipmentId` means a previous tick created it but the outbox write failed — the outbox row is re-queued
- Catches all exceptions internally, logs, and lets the next tick retry — consistent with `OutboxDispatcherTask` and `ReleaseExpiredReservationsTask`

---

### 2. `IOpenBoxesClient` (extended)

**File:** `Services/IOpenBoxesClient.cs` (existing interface, new method added)

**New method:**
```csharp
Task<IReadOnlyList<OpenBoxesFulfillmentOrder>> GetIssuedFulfillmentOrdersAsync(
    CancellationToken cancellationToken = default);
```

Returns all fulfillment orders currently in `ISSUED` state. The implementation calls `GET /api/generic/shipment?status=ISSUED` on the OpenBoxes REST API.

---

### 3. `OpenBoxesFulfillmentOrder`

**Type:** DTO
**File:** `Services/OpenBoxesFulfillmentOrder.cs`

**Fields:**

| Field | Type | Notes |
| --- | --- | --- |
| `FulfillmentId` | `string` | OpenBoxes internal identifier |
| `OrderGuid` | `Guid` | Correlation key — sent by the bridge at creation |
| `Status` | `string` | Raw status string from OpenBoxes |
| `IssuedAtUtc` | `DateTime?` | When OpenBoxes transitioned to ISSUED |

---

### 4. `IShipmentService` (existing, injected)

**File:** `Nop.Services.Shipping.IShipmentService` (core nopCommerce service)

The poller depends on `IShipmentService` to create the `Shipment` and `ShipmentItem` rows before writing the carrier booking outbox row. Without this, `CarrierBookingConsumer` (Iter 4) has no `Shipment` row to write `ExternalShipmentId` back to.

---

### 5. `OpenBoxesPollerSettings`

**Type:** `ISettings` extension to `AllocationSettings`
**File:** `AllocationSettings.cs` (new fields added)

| Field | Default | Notes |
| --- | --- | --- |
| `OpenBoxesBaseUrl` | env-driven | Base URL for the OpenBoxes REST API |
| `OpenBoxesApiKey` | env-driven | Credentials for the OpenBoxes API |
| `PollerIntervalSeconds` | `30` | How often the task polls |
| `PollerBatchSize` | `50` | Max fulfillment orders fetched per tick |

---

## Registration

`PluginNopStartup` registers the task:

```csharp
await _scheduleTaskRepository.InsertAsync(new ScheduleTask
{
    Name        = "VerdeMart OpenBoxes Status Poller",
    Type        = "Nop.Plugin.Inventory.AllocationGate.ScheduleTasks.OpenBoxesStatusPollerTask",
    Seconds     = settings.PollerIntervalSeconds,
    Enabled     = true,
    StopOnError = false
});
```

Step 5 defines the precise interfaces and wire contracts.
