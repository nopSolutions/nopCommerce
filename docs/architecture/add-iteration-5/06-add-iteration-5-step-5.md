# ADD Iteration 5 — Step 5: Define Interfaces

## `IOpenBoxesClient` — New Method

```csharp
public interface IOpenBoxesClient
{
    Task<CreateFulfillmentResult> CreateFulfillmentAsync(
        OrderPlacedMessage message,
        CancellationToken cancellationToken = default);

    // NEW
    Task<IReadOnlyList<OpenBoxesFulfillmentOrder>> GetIssuedFulfillmentOrdersAsync(
        CancellationToken cancellationToken = default);
}
```

The implementation calls:
```
GET {OpenBoxesBaseUrl}/api/generic/shipment?status=ISSUED
```

Response is a JSON array of shipment objects. The `OrderGuid` is expected in the `referenceNumber` or equivalent field — exact field name confirmed by the OpenBoxes feasibility spike against a local instance.

---

## `OpenBoxesFulfillmentOrder` DTO

```csharp
public record OpenBoxesFulfillmentOrder(
    string FulfillmentId,
    Guid   OrderGuid,
    string Status,
    DateTime? IssuedAtUtc
);
```

---

## `OpenBoxesStatusPollerTask`

```csharp
public class OpenBoxesStatusPollerTask : IScheduleTask
{
    public OpenBoxesStatusPollerTask(
        IOpenBoxesClient openBoxesClient,
        IOrderService orderService,
        IOrderProcessingService orderProcessingService,
        IShipmentService shipmentService,
        IOutboxRepository outboxRepository,
        AllocationSettings settings,
        ILogger logger);

    public Task ExecuteAsync();
}
```

**`ExecuteAsync` logic:**

1. Call `GetIssuedFulfillmentOrdersAsync()` — fetch up to `PollerBatchSize` results
2. For each `OpenBoxesFulfillmentOrder`:
   a. Look up `Order` by `OrderGuid` via `IOrderService.GetOrderByGuidAsync()`
   b. If not found: log warning, skip
   c. If found and order already has a `Shipment` with `ExternalShipmentId` set: skip (carrier booking already confirmed — idempotent)
   d. If found and not yet confirmed: open a DB transaction, then:
      - Fetch order items via `IOrderService.GetOrderItemsAsync(order.Id)`
      - Create a `Shipment` record and one `ShipmentItem` per order item via `IShipmentService.InsertShipmentAsync` / `InsertShipmentItemAsync` — fires `ShipmentCreatedEvent` and produces the `ShipmentId` needed by the carrier booking consumer
      - Call `IOrderProcessingService.SetOrderStatusAsync(OrderStatus.Complete)` to transition the order status
      - Write an outbox row with `EventType = "carrier.booking.requested"` and `ShipmentId` in the payload
      - Commit the transaction — all three writes are atomic; on any failure the transaction rolls back and the next tick retries
3. Catch all exceptions internally; log and continue to next tick

---

## OpenBoxes API Request

```
GET /api/generic/shipment?status=ISSUED
Authorization: Basic {base64(username:password)}
```

Response (array):
```json
[
  {
    "id": "abc123",
    "referenceNumber": "550e8400-e29b-41d4-a716-446655440000",
    "status": "ISSUED",
    "lastUpdated": "2026-05-01T10:00:00Z"
  }
]
```

The `referenceNumber` field carries the `OrderGuid` set by the bridge when creating the fulfillment order. This mapping is confirmed by the feasibility spike.

---

## Updated `AllocationSettings`

```csharp
public class AllocationSettings : ISettings
{
    public int    WebReservationTtlSeconds   { get; set; } = 30;
    public int    PosReservationTtlSeconds   { get; set; } = 300;
    public int    ReleaseTaskBatchSize       { get; set; } = 200;
    public int    ReleaseTaskIntervalSeconds { get; set; } = 30;

    // NEW
    public string OpenBoxesBaseUrl          { get; set; } = "";
    public string OpenBoxesApiKey           { get; set; } = "";
    public int    PollerIntervalSeconds     { get; set; } = 30;
    public int    PollerBatchSize           { get; set; } = 50;
}
```

Step 6 sketches the updated component view and records the polling design decision.
