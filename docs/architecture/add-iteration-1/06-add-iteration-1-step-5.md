# ADD Iteration 1 — Step 5: Define Interfaces

## What This Step Does

Step 5 defines how the components from Step 4 talk to each other — method signatures, message contracts, and configuration shape. After this step the design is precise enough to implement directly.

---

## 1. `IRabbitMqConnectionFactory`

```csharp
public interface IRabbitMqConnectionFactory
{
    IModel CreateChannel();
}
```

- `CreateChannel()` returns a new `IModel` from the shared `IConnection`
- The caller is responsible for disposing the channel after use
- The implementation opens the `IConnection` lazily on first call

---

## 2. `IConsumer<OrderPlacedEvent>` — implemented by `OrderPlacedConsumer`

This interface is defined by nopCommerce core. The plugin implements it:

```csharp
public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    public Task HandleEventAsync(OrderPlacedEvent eventMessage);
}
```

- `HandleEventAsync` is the single entry point — fired by the nopCommerce event system after every successful order placement
- Must complete without blocking the checkout thread
- Must not swallow exceptions — let them propagate so nopCommerce can log them

---

## 3. `OrderPlacedMessage` — wire contract

The JSON payload published to RabbitMQ. This is the contract between the publisher and all downstream consumers.

```csharp
public record OrderPlacedMessage(
    int OrderId,
    Guid OrderGuid,
    int CustomerId,
    decimal OrderTotal,
    DateTime CreatedOnUtc,
    IReadOnlyList<OrderItemMessage> Items
);

public record OrderItemMessage(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice
);
```

- Use `record` for immutability — the message must not be mutated after construction
- Serialised to JSON using `System.Text.Json` before publish
- `OrderGuid` is the idempotency key for downstream consumers — they use it to detect duplicate deliveries

---

## 4. `RabbitMqSettings` — configuration contract

```csharp
public class RabbitMqSettings : ISettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "verdemart.orders";
    public string OrderPlacedQueueName { get; set; } = "verdemart.orders.openboxes";
}
```

- Implements `ISettings` so nopCommerce persists it in the database
- Readable from the admin panel without redeployment
- `ExchangeName` and `OrderPlacedQueueName` are configurable so the topology can be adjusted per environment

---

## 5. `PluginNopStartup` — registration contract

```csharp
public class PluginNopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
        services.AddScoped<IConsumer<OrderPlacedEvent>, OrderPlacedConsumer>();
    }

    public void Configure(IApplicationBuilder application) { }

    public int Order => 3000;
}
```

- `IRabbitMqConnectionFactory` is singleton — one TCP connection for the lifetime of the application
- `OrderPlacedConsumer` is scoped — resolved per request, consistent with how nopCommerce resolves other consumers

---

## 6. RabbitMQ Topology Contract

Declared by `RabbitMqPlugin.InstallAsync()` and by `RabbitMqConnectionFactory` on first channel creation:

| Element | Name | Properties |
|---|---|---|
| Exchange | `verdemart.orders` | type: direct, durable: true, auto-delete: false |
| Queue | `verdemart.orders.openboxes` | durable: true, auto-delete: false, exclusive: false |
| Binding | queue → exchange | routing key: `order.placed` |
| Message | `order.placed` | delivery mode: 2 (persistent), content type: `application/json` |

---

## Interface Dependency Map

```
PluginNopStartup
    registers → IRabbitMqConnectionFactory (singleton)
    registers → IConsumer<OrderPlacedEvent> (scoped)

OrderPlacedConsumer
    depends on → IRabbitMqConnectionFactory
    depends on → RabbitMqSettings
    produces   → OrderPlacedMessage (serialised to JSON)
    calls      → IRabbitMqConnectionFactory.CreateChannel()
    publishes  → exchange: verdemart.orders / key: order.placed

RabbitMqConnectionFactory
    depends on → RabbitMqSettings
    implements → IRabbitMqConnectionFactory
```

---

## What Step 6 Will Do

Step 6 sketches the component view and records the architectural decisions (ADRs) that this iteration produced.
