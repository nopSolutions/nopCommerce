# ADD Iteration 3 — Step 5: Define Interfaces

## What This Step Does

Step 5 defines how the components from Step 4 communicate — method signatures, message contracts, configuration shape, and HTTP wire formats. After this step the design is precise enough to implement directly.

---

## Part A — nopCommerce Plugin (`Nop.Plugin.Inventory.AllocationGate`)

### 1. `ProductReservation` Entity

```csharp
public class ProductReservation : BaseEntity
{
    public string ReservationKey { get; set; }
    public string ChannelKey { get; set; }
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
    public ReservationStatus Status { get; set; }
    public DateTime ReservedAtUtc { get; set; }
    public DateTime ReservedUntilUtc { get; set; }
    public DateTime? ConfirmedAtUtc { get; set; }
}

public enum ReservationStatus : byte
{
    Reserved  = 0,
    Committed = 1,
    Released  = 2,
    Expired   = 3
}
```

- Inherits `BaseEntity` to integrate with nopCommerce's `IRepository<TEntity>` layer
- `ReservationKey` is `OrderGuid.ToString()` for the web flow and an opaque session key for POS
- `ChannelKey ∈ { "web", "pos" }` is descriptive only — the gate logic does not branch on channel

---

### 2. `IProductReservationRepository`

```csharp
public interface IProductReservationRepository
{
    Task InsertAsync(
        ProductReservation reservation,
        CancellationToken cancellationToken = default);

    Task<int> GetActiveSumAsync(
        int productId,
        int warehouseId,
        CancellationToken cancellationToken = default);

    Task MarkCommittedAsync(
        string reservationKey,
        DateTime confirmedAtUtc,
        CancellationToken cancellationToken = default);

    Task MarkReleasedAsync(
        string reservationKey,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductReservation>> FetchExpiredAsync(
        int batchSize,
        CancellationToken cancellationToken = default);

    Task MarkExpiredAsync(
        long id,
        CancellationToken cancellationToken = default);
}
```

- `InsertAsync` enlists in the ambient transaction (uses `IRepository<ProductReservation>.InsertAsync` underneath)
- `GetActiveSumAsync` returns `SUM(Quantity) WHERE ProductId=@p AND WarehouseId=@w AND Status = Reserved AND ReservedUntilUtc > UtcNow`
- `MarkCommittedAsync` and `MarkReleasedAsync` operate by `ReservationKey` because a single key spans multiple item rows
- `FetchExpiredAsync` and `MarkExpiredAsync` are used by `ReleaseExpiredReservationsTask`; `FetchExpiredAsync` selects rows where `Status=Reserved` and `ReservedUntilUtc < UtcNow`

---

### 3. `IAllocationGate` and `AllocationResult`

```csharp
public interface IAllocationGate
{
    Task<AllocationResult> ReserveAsync(
        string reservationKey,
        string channelKey,
        IReadOnlyList<AllocationItem> items,
        TimeSpan? ttl = null,
        CancellationToken cancellationToken = default);

    Task ConfirmAsync(
        string reservationKey,
        CancellationToken cancellationToken = default);

    Task ReleaseAsync(
        string reservationKey,
        CancellationToken cancellationToken = default);
}

public record AllocationItem(
    int ProductId,
    int WarehouseId,
    int Quantity);

public record AllocationResult(
    bool Success,
    IReadOnlyList<AllocationFailure> Failures);

public record AllocationFailure(
    int ProductId,
    int WarehouseId,
    int RequestedQuantity,
    int AvailableQuantity);
```

- `ReserveAsync` runs the per-item lock-and-decrement. `ttl == null` falls back to `AllocationSettings.WebReservationTtlSeconds` (web) or `PosReservationTtlSeconds` (POS) based on `channelKey`
- The method **never throws** for "stock unavailable" — it returns a structured `AllocationResult` with the failed items so callers can surface specific messages. Exceptions are reserved for system errors (connection lost, etc.)
- `ConfirmAsync` is idempotent — calling it twice for the same key is a no-op after the first transition
- `ReleaseAsync` is also idempotent

---

### 4. `AllocationGateProductServiceDecorator`

```csharp
public class AllocationGateProductServiceDecorator : IProductService
{
    private readonly IProductService _inner;
    private readonly IAllocationGate _gate;
    private readonly IAmbientOrderContext _orderContext;  // exposes the OrderGuid of the in-flight PlaceOrder; see note below

    public AllocationGateProductServiceDecorator(
        IProductService inner,
        IAllocationGate gate,
        IAmbientOrderContext orderContext)
    { _inner = inner; _gate = gate; _orderContext = orderContext; }

    public async Task AdjustInventoryAsync(
        Product product,
        int quantityToChange,
        string attributesXml = "",
        string message = "")
    {
        if (quantityToChange < 0 && _orderContext.CurrentOrderGuid is { } orderGuid)
        {
            var result = await _gate.ReserveAsync(
                reservationKey: orderGuid.ToString(),
                channelKey: "web",
                items: new[] { new AllocationItem(product.Id, ResolveWarehouseId(product), -quantityToChange) });

            if (!result.Success)
                throw new NopException(BuildStockUnavailableMessage(result.Failures));
        }

        await _inner.AdjustInventoryAsync(product, quantityToChange, attributesXml, message);
    }

    // every other IProductService method delegates straight through to _inner
    public Task<Product> GetProductByIdAsync(int productId) => _inner.GetProductByIdAsync(productId);
    // ... etc — generated by IDE or written explicitly
}
```

- The decorator implements every method on `IProductService`; only `AdjustInventoryAsync` has new behaviour
- For `quantityToChange >= 0` (stock returns) the call is delegated straight through — gate runs only on decrements
- The thrown `NopException` is caught by `OrderProcessingService.PlaceOrderAsync`'s existing try/catch (verified at [OrderProcessingService.cs:1634](../../../src/Libraries/Nop.Services/Orders/OrderProcessingService.cs#L1634)) and surfaces as `PlaceOrderResult.Errors`
- `ResolveWarehouseId(product)` reuses nopCommerce's existing per-product warehouse-resolution logic; the resolution is not new behaviour

> **Note on `IAmbientOrderContext`**: this is a new contract introduced by the plugin to carry the `OrderGuid` of the in-flight `PlaceOrderAsync` call into the decorator. nopCommerce has no equivalent today (`IWorkContext` exposes the current customer, not the in-flight order). Two viable implementations: an `IHttpContextAccessor`-backed scoped service (writes `HttpContext.Items["OrderGuid"]` from the checkout controller before invoking `OrderProcessingService.PlaceOrderAsync`), or an `AsyncLocal<Guid?>`-backed ambient that the plugin exposes via an extension point. The contract above does not depend on the choice; the implementation is finalised at coding time and recorded as a small follow-up ADR if it surfaces a coupling issue.

---

### 5. `/api/inventory/*` — HTTP Wire Contract for POS

#### `POST /api/inventory/reserve`

```json
// Request
{
  "reservationKey": "9b5f...UUID...",
  "channelKey":     "pos",
  "items": [
    { "productId": 42, "warehouseId": 1, "quantity": 1 }
  ]
}

// 200 OK Response
{
  "success": true,
  "reservedUntilUtc": "2026-04-28T10:35:00Z"
}

// 409 Conflict Response
{
  "success": false,
  "failures": [
    { "productId": 42, "warehouseId": 1, "requestedQuantity": 1, "availableQuantity": 0 }
  ]
}
```

#### `POST /api/inventory/confirm`

```json
// Request
{ "reservationKey": "9b5f...UUID..." }

// 200 OK Response
{ "success": true }
```

For POS, `confirm` not only marks the reservation `Committed` but also calls `IProductService.AdjustInventoryAsync(product, -quantity)` for each item — POS does not flow through `OrderProcessingService`, so the actual `StockQuantity` decrement happens here. Inside the decorator path this would re-enter the gate, so the controller calls the **inner** `IProductService` (resolved by name) directly, bypassing the decorator. This is the only consumer permitted to bypass the decorator and the rationale is recorded in Step 6.

#### `POST /api/inventory/release`

```json
// Request
{ "reservationKey": "9b5f...UUID..." }

// 200 OK Response
{ "success": true }
```

All three endpoints require an `Authorization: Bearer <token>` header. Token validation mechanism is operational config.

---

### 6. `ReleaseExpiredReservationsTask`

```csharp
public class ReleaseExpiredReservationsTask : IScheduleTask
{
    public ReleaseExpiredReservationsTask(
        IProductReservationRepository repository,
        AllocationSettings settings,
        ILogger logger);

    public Task ExecuteAsync();
}
```

- `ExecuteAsync` fetches up to `settings.ReleaseTaskBatchSize` expired rows and marks each `Expired`
- Catches all exceptions internally and logs — never throws, consistent with `OutboxDispatcherTask` (Iter 2)

---

### 7. `AllocationSettings`

```csharp
public class AllocationSettings : ISettings
{
    public int WebReservationTtlSeconds      { get; set; } = 30;
    public int PosReservationTtlSeconds      { get; set; } = 300;
    public int ReleaseTaskBatchSize          { get; set; } = 200;
    public int ReleaseTaskIntervalSeconds    { get; set; } = 30;
}
```

- Implements `ISettings` so nopCommerce persists it in the database
- Configurable from the admin panel without redeployment

---

### 8. `PluginNopStartup`

```csharp
public class PluginNopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAllocationGate, AllocationGate>();
        services.AddScoped<IProductReservationRepository, ProductReservationRepository>();
        services.AddScoped<ReleaseExpiredReservationsTask>();
        services.AddScoped<IAmbientOrderContext, AmbientOrderContext>();

        DecorateProductService(services);
    }

    public void Configure(IApplicationBuilder application) { }

    public int Order => 3500;  // after Iter 1 plugin (3000), before any consumer-side work
}
```

#### Decorator wiring

`IDependencyRegistrar` does not exist in this version of nopCommerce; the only DI extension point exposed to plugins is `INopStartup.ConfigureServices(IServiceCollection, IConfiguration)`. Autofac is wired conditionally on top of `IServiceCollection` (per `Nop.Web/Program.cs` — only when `CommonConfig.UseAutofac == true`). The decoration must therefore work at the `IServiceCollection` level so it is portable across both DI configurations.

The implementation is a manual descriptor swap:

```csharp
static void DecorateProductService(IServiceCollection services)
{
    var existing = services.FirstOrDefault(d => d.ServiceType == typeof(IProductService))
                   ?? throw new InvalidOperationException(
                       "IProductService not registered yet — adjust INopStartup.Order");

    // Keep the inner concrete type resolvable so the decorator can depend on it
    services.Add(new ServiceDescriptor(
        existing.ImplementationType!,
        existing.ImplementationType!,
        existing.Lifetime));

    // Replace the IProductService binding with the decorator
    services.Replace(new ServiceDescriptor(
        typeof(IProductService),
        sp => new AllocationGateProductServiceDecorator(
            (IProductService)sp.GetRequiredService(existing.ImplementationType!),
            sp.GetRequiredService<IAllocationGate>(),
            sp.GetRequiredService<IAmbientOrderContext>()),
        existing.Lifetime));
}
```

This depends only on `Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.Replace`, already available in the framework. It works when `UseAutofac=true` because Autofac inherits the MS DI registrations.

The schedule-task entry is registered through the plugin's `InstallAsync` hook:

```csharp
await _scheduleTaskRepository.InsertAsync(new ScheduleTask
{
    Name        = "VerdeMart Release Expired Reservations",
    Type        = "Nop.Plugin.Inventory.AllocationGate.ScheduleTasks.ReleaseExpiredReservationsTask",
    Seconds     = settings.ReleaseTaskIntervalSeconds,
    Enabled     = true,
    StopOnError = false
});
```

---

### Schema Migration

A FluentMigrator migration creates the `ProductReservation` table on plugin install, with the schema shown in Step 4. Indexes:

```
CREATE UNIQUE INDEX IX_ProductReservation_ReservationKey
    ON ProductReservation (ReservationKey, ProductId, WarehouseId);

CREATE INDEX IX_ProductReservation_Status_ReservedUntilUtc
    ON ProductReservation (Status, ReservedUntilUtc);

CREATE INDEX IX_ProductReservation_Product_Status
    ON ProductReservation (ProductId, WarehouseId, Status);
```

---

## Part B — Bridge Service (`VerdeMart.OpenBoxesBridge`)

### 9. `BridgeWorker`

```csharp
public class BridgeWorker : BackgroundService
{
    public BridgeWorker(
        IServiceProvider services,
        IOptions<BridgeSettings> settings,
        ILogger<BridgeWorker> logger);

    protected override Task ExecuteAsync(CancellationToken stoppingToken);
}
```

- Establishes the RabbitMQ connection on startup; tears it down on shutdown
- Resolves an `OrderPlacedMessageConsumer` per delivery via a scoped service scope (one scope per message)
- The connection is lazy: on connection failure, the worker logs and retries with exponential backoff up to a cap; if the broker is down for the iteration's lifetime, no messages flow but the worker stays healthy

---

### 10. `OrderPlacedMessageConsumer`

```csharp
public class OrderPlacedMessageConsumer
{
    public OrderPlacedMessageConsumer(
        IOpenBoxesClient openBoxesClient,
        IDedupRepository dedupRepository,
        IOptions<BridgeSettings> settings,
        ILogger<OrderPlacedMessageConsumer> logger);

    public Task<ConsumeOutcome> HandleAsync(
        OrderPlacedMessage message,
        int currentRedeliveryCount,
        CancellationToken cancellationToken);
}

public enum ConsumeOutcome
{
    Ack,                  // success or already-processed
    NackWithRequeue,      // transient failure, retry later
    NackToDeadLetter      // poison or threshold exceeded
}
```

- The dispatcher loop in `BridgeWorker` reads the `x-death` header (RabbitMQ adds this on each redelivery) to compute `currentRedeliveryCount`
- `HandleAsync` runs the flow described in Step 4 component 11 and returns the outcome the worker uses to ack or nack the channel

---

### 11. `IOpenBoxesClient`

```csharp
public interface IOpenBoxesClient
{
    Task<CreateFulfillmentResult> CreateFulfillmentAsync(
        OrderPlacedMessage message,
        CancellationToken cancellationToken);
}

public record CreateFulfillmentResult(
    CreateFulfillmentStatus Status,
    string? OpenBoxesFulfillmentId,
    string? Error);

public enum CreateFulfillmentStatus
{
    Created,
    Duplicate,           // OpenBoxes detected an existing fulfillment for the OrderGuid
    TransientFailure,    // 5xx, timeout — retry-able
    PermanentFailure     // 4xx, malformed — DLQ
}
```

- The implementation owns its own bounded retry-with-backoff for `TransientFailure` (e.g. up to 3 in-call retries with jitter); the broader retry budget is owned by message redelivery
- The exact OpenBoxes endpoint URL and JSON shape depend on the spike outcome flagged in Step 1; the `IOpenBoxesClient` interface insulates the consumer from those details

---

### 12. `IDedupRepository`

```csharp
public interface IDedupRepository
{
    Task<bool> HasProcessedAsync(
        Guid orderGuid,
        CancellationToken cancellationToken);

    Task RecordProcessedAsync(
        Guid orderGuid,
        string? openBoxesFulfillmentId,
        CancellationToken cancellationToken);
}
```

- `HasProcessedAsync` returns `true` if a row with `OrderGuid` exists
- `RecordProcessedAsync` uses `INSERT ... ON CONFLICT DO NOTHING` (PostgreSQL) or `INSERT IGNORE` (MySQL/SQLite) so a redelivered message that races itself does not double-insert

---

### 13. `BridgeSettings`

```csharp
public class BridgeSettings
{
    public string RabbitMqHost           { get; set; } = "localhost";
    public int    RabbitMqPort           { get; set; } = 5672;
    public string RabbitMqUsername       { get; set; } = "guest";
    public string RabbitMqPassword       { get; set; } = "guest";
    public string ExchangeName           { get; set; } = "verdemart.orders";
    public string OrderQueueName         { get; set; } = "verdemart.orders.openboxes";
    public string DeadLetterExchangeName { get; set; } = "verdemart.orders.dlx";
    public int    MaxRedeliveryAttempts  { get; set; } = 5;
    public string OpenBoxesBaseUrl       { get; set; } = "";
    public string OpenBoxesApiKey        { get; set; } = "";
    public int    OpenBoxesTimeoutMs     { get; set; } = 5000;
    public string DedupConnectionString  { get; set; } = "";
}
```

Bound from configuration via `IOptions<BridgeSettings>`. Sensitive values (passwords, API keys) are environment-driven in production.

---

## Part C — RabbitMQ Topology Additions

Declared by `BridgeWorker` on startup so the bridge owns its consumption topology:

| Element | Name | Properties |
|---|---|---|
| Exchange | `verdemart.orders.dlx` | type: direct, durable: true, auto-delete: false |
| Queue | `verdemart.orders.openboxes.dlq` | durable: true, auto-delete: false |
| Binding | DLQ ← DLX | routing key: `order.placed` |
| Argument added to existing queue | `verdemart.orders.openboxes` | `x-dead-letter-exchange = verdemart.orders.dlx` |

---

## Part D — Wire Contract Update

```csharp
public record OrderPlacedMessage(
    int OrderId,
    Guid OrderGuid,
    int CustomerId,
    decimal OrderTotal,
    DateTime CreatedOnUtc,
    IReadOnlyList<OrderItemMessage> Items,
    int Version = 1
);
```

- `Version` is a positional parameter with default `1` — Iter 1's existing publish call site compiles unchanged
- The Iter 1 plugin's `OrderPlacedConsumer` is updated to set the field explicitly when serialising
- Consumers (the bridge in this iteration; future ERPNext, search, CRM bridges) read tolerantly:
  - Unknown additional fields are ignored (`System.Text.Json` default with `JsonSerializerOptions.IgnoreReadOnlyProperties` left at defaults)
  - Missing optional fields fall back to defaults
  - A `Version` higher than supported routes the message to the DLQ (no silent processing of an unknown contract)

---

## Interface Dependency Map

```
[ nopCommerce process ]

OrderProcessingService
    │ MoveShoppingCartItemsToOrderItemsAsync
    │   → IProductService.AdjustInventoryAsync          (resolved by Autofac)
    ▼
AllocationGateProductServiceDecorator   (registered via descriptor swap in INopStartup)
    depends on → IProductService (inner, resolved via concrete type registration)
    depends on → IAllocationGate
    depends on → IAmbientOrderContext (carries the in-flight OrderGuid)

AllocationGate
    depends on → IProductReservationRepository
    depends on → IRepository<ProductWarehouseInventory>  (existing nopCommerce)
    depends on → AllocationSettings

ProductReservationRepository
    depends on → IRepository<ProductReservation>

AllocationApiController
    depends on → IAllocationGate
    depends on → IProductService (inner, resolved by named binding to bypass decorator)

ReleaseExpiredReservationsTask
    depends on → IProductReservationRepository
    depends on → AllocationSettings


[ Bridge service — separate process ]

BridgeWorker
    depends on → IOptions<BridgeSettings>
    creates    → per-message scope, resolves OrderPlacedMessageConsumer

OrderPlacedMessageConsumer
    depends on → IOpenBoxesClient
    depends on → IDedupRepository

OpenBoxesClient
    depends on → HttpClient (typed)
    depends on → IOptions<BridgeSettings>

DedupRepository
    depends on → connection string from BridgeSettings


[ Shared contracts ]

OrderPlacedMessage  (JSON wire)
    Version=1 — produced by Iter 1 plugin, consumed by the bridge

RabbitMQ topology
    verdemart.orders            (exchange, direct, durable — Iter 1)
    verdemart.orders.openboxes  (queue, durable, manual ack — Iter 1, this iteration adds DLX argument)
    verdemart.orders.dlx        (exchange, direct, durable — this iteration)
    verdemart.orders.openboxes.dlq (queue, durable — this iteration)
```

---

## What Step 6 Will Do

Step 6 produces the updated views (component + sequence) showing the synchronous gate path for both web and POS, and the asynchronous bridge consumer path. It then records the architectural decisions this iteration produced as ADRs in the consolidated `07-adrs/` set.
