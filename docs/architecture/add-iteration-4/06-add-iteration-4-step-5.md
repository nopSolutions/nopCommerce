# ADD Iteration 4 — Step 5: Define Interfaces

## What This Step Does

Step 5 defines how the components from Step 4 communicate — method signatures, message contracts, configuration shape, and HTTP wire formats. After this step the design is precise enough to implement directly.

---

## 1. Entities

```csharp
public class CarrierWebhookEvent : BaseEntity
{
    public string? EventId          { get; set; }
    public DateTime ReceivedAtUtc   { get; set; }
    public string RawPayload        { get; set; } = "";
    public string RemoteIp          { get; set; } = "";
    public WebhookOutcome Outcome   { get; set; }
    public string? Notes            { get; set; }
}

public enum WebhookOutcome : byte
{
    Accepted  = 0,
    Duplicate = 1,
    Rejected  = 2,
    Unmatched = 3
}

public class ProcessedCarrierEvent : BaseEntity
{
    public string EventId           { get; set; } = "";  // unique
    public DateTime ProcessedAtUtc  { get; set; }
    public int ShipmentId           { get; set; }
}
```

The four new columns on `Shipment` are exposed via partial class extension in the plugin (`Nop.Core.Domain.Shipping.Shipment` is the existing class; the migration adds the columns and the plugin adds a partial class with the four properties so the data layer can map them).

```csharp
public partial class Shipment
{
    public string? ExternalShipmentId       { get; set; }
    public string? ExternalCarrierCode      { get; set; }
    public string? ExternalShippingStatus   { get; set; }
    public DateTime? LastStatusOccurredAtUtc { get; set; }
}
```

---

## 2. `ICarrierWebhookAuditService`

```csharp
public interface ICarrierWebhookAuditService
{
    Task<int> RecordReceiptAsync(
        string rawPayload,
        string remoteIp,
        string? eventId,
        WebhookOutcome outcome,
        string? notes,
        CancellationToken cancellationToken = default);

    Task AmendOutcomeAsync(
        int auditId,
        WebhookOutcome outcome,
        string? notes,
        CancellationToken cancellationToken = default);
}
```

- `RecordReceiptAsync` returns the inserted `Id` so the controller can pass it on the published message; the consumer uses it to amend the outcome
- `AmendOutcomeAsync` is idempotent — calling twice with the same arguments is a no-op after the first

---

## 3. `IProcessedCarrierEventRepository`

```csharp
public interface IProcessedCarrierEventRepository
{
    Task<bool> HasProcessedAsync(
        string eventId,
        CancellationToken cancellationToken = default);

    Task RecordProcessedAsync(
        string eventId,
        int shipmentId,
        CancellationToken cancellationToken = default);
}
```

- `RecordProcessedAsync` enlists in the ambient transaction; the unique PK constraint on `EventId` defends against concurrent races

---

## 4. `IExternalStatusMapper`

```csharp
public interface IExternalStatusMapper
{
    ShippingStatus? MapToInternal(string externalStatus);
}

public class ExternalStatusMapper : IExternalStatusMapper
{
    public ShippingStatus? MapToInternal(string externalStatus) =>
        externalStatus switch
        {
            "PICKED_UP"        => ShippingStatus.Shipped,
            "IN_TRANSIT"       => ShippingStatus.Shipped,
            "OUT_FOR_DELIVERY" => ShippingStatus.Shipped,
            "DELIVERED"        => ShippingStatus.Delivered,
            _                  => null
        };
}
```

- `null` means "no internal transition" — the consumer leaves `ShippingStatus` unchanged
- Singleton; pure function

---

## 5. `IWireMockClient`

```csharp
public interface IWireMockClient
{
    Task<BookingResult> BookShipmentAsync(
        BookingRequest request,
        CancellationToken cancellationToken);
}

public record BookingRequest(
    int ShipmentId,
    int OrderId,
    BookingAddress ShippingAddress,
    IReadOnlyList<BookingItem> Items);

public record BookingAddress(
    string AddressLine1,
    string City,
    string PostalCode,
    string CountryCode,
    string RecipientName);

public record BookingItem(
    string Sku,
    int Quantity,
    decimal WeightKg);

public record BookingResult(
    BookingStatus Status,
    string? CarrierTrackingId,
    string? CarrierCode,
    string? Error);

public enum BookingStatus
{
    Booked,
    TransientFailure,
    PermanentFailure
}
```

- The implementation owns its bounded retry-with-backoff for `TransientFailure` (3 attempts with jitter); broader retry is owned by message redelivery
- `Booked` requires a non-null `CarrierTrackingId`

---

## 6. `/api/carrier/webhook` — HTTP Wire Contract

### Request

```
POST /api/carrier/webhook
Authorization: Bearer <token>
Content-Type: application/json

{
  "eventId":           "9b5f-...-uuid",
  "carrierTrackingId": "WM-ABC-12345",
  "carrierCode":       "WIREMOCK",
  "status":            "IN_TRANSIT",
  "statusDescription": "In transit to destination hub",
  "occurredAtUtc":     "2026-04-29T14:32:11Z",
  "location":          "Lisbon Distribution Center"
}
```

### Responses

| Code | When |
| --- |---|
| `200 OK` | Authenticated, parsed, queued for processing. Empty body. |
| `400 Bad Request` | Body missing required fields or fails JSON parsing. |
| `401 Unauthorized` | Missing or wrong bearer token. |
| `500 Internal Server Error` | Audit-row insert or queue publish failed. Carrier should retry. |

The 200 response is returned **before** processing completes; the carrier's view of success is "received and persisted to audit log + queue". Processing failures surface in the audit table and DLQ, not in the HTTP response.

---

## 7. Internal Queue Message — `CarrierStatusReceivedMessage`

Published by `CarrierWebhookController` to `verdemart.carrier.status` on routing key `carrier.status.received`:

```csharp
public record CarrierStatusReceivedMessage(
    string EventId,
    string CarrierTrackingId,
    string CarrierCode,
    string Status,
    string StatusDescription,
    DateTime OccurredAtUtc,
    string? Location,
    DateTime ReceivedAtUtc,
    int AuditRowId,
    int Version = 1);
```

- `AuditRowId` lets the consumer call `ICarrierWebhookAuditService.AmendOutcomeAsync` for `Duplicate`/`Unmatched` outcomes
- `Version` follows the wire-contract versioning policy established in Iteration 3

---

## 8. Outbox Event — `CarrierBookingRequestedMessage`

Inserted into the existing Outbox table by `ShipmentSentEventConsumer`:

```csharp
public record CarrierBookingRequestedMessage(
    int ShipmentId,
    int OrderId,
    BookingAddress ShippingAddress,
    IReadOnlyList<BookingItem> Items,
    int Version = 1);
```

- Outbox `EventType` column: `"carrier.booking.requested"`
- Routing key when published by the dispatcher: `"carrier.booking.requested"`
- Exchange: `verdemart.carrier.booking`

---

## 9. Consumer Outcome Types

```csharp
public enum ConsumeOutcome
{
    Ack,
    NackWithRequeue,
    NackToDeadLetter
}
```

Reused for both `CarrierStatusConsumer` and `CarrierBookingConsumer`.

The dispatcher loops in each `BackgroundService` read `x-death` from RabbitMQ to compute redelivery counts. Threshold values come from `CarrierWebhookSettings.MaxStatusRedeliveries` and `MaxBookingRedeliveries`.

---

## 10. `CarrierStatusConsumer` Handler Signature

```csharp
public class CarrierStatusConsumer
{
    public CarrierStatusConsumer(
        IShipmentService shipmentService,
        IProcessedCarrierEventRepository dedupRepository,
        IExternalStatusMapper statusMapper,
        ICarrierWebhookAuditService auditService,
        IWorkflowMessageService workflowMessageService,
        IDbContext dbContext,
        IOptions<CarrierWebhookSettings> settings,
        ILogger<CarrierStatusConsumer> logger);

    public Task<ConsumeOutcome> HandleAsync(
        CarrierStatusReceivedMessage message,
        int currentRedeliveryCount,
        CancellationToken cancellationToken);
}
```

The flow inside `HandleAsync` follows Step 4 component 4 verbatim. The DB transaction wraps: `Shipment` UPDATE, `ProcessedCarrierEvent` INSERT, `QueuedEmail` INSERT (the last via `IWorkflowMessageService` which goes through `IQueuedEmailService` underneath).

---

## 11. `CarrierBookingConsumer` Handler Signature

```csharp
public class CarrierBookingConsumer
{
    public CarrierBookingConsumer(
        IShipmentService shipmentService,
        IWireMockClient wireMockClient,
        IOptions<CarrierWebhookSettings> settings,
        ILogger<CarrierBookingConsumer> logger);

    public Task<ConsumeOutcome> HandleAsync(
        CarrierBookingRequestedMessage message,
        int currentRedeliveryCount,
        CancellationToken cancellationToken);
}
```

Idempotency check: if the `Shipment` already has `ExternalShipmentId`, return `Ack` — the booking ran on a previous delivery.

---

## 12. `CarrierWebhookSettings`

```csharp
public class CarrierWebhookSettings : ISettings
{
    public string InboundBearerToken     { get; set; } = "";
    public string WireMockBaseUrl        { get; set; } = "";
    public int    WireMockTimeoutMs      { get; set; } = 3000;
    public int    MaxStatusRedeliveries  { get; set; } = 5;
    public int    MaxBookingRedeliveries { get; set; } = 5;
    public string CarrierCode            { get; set; } = "WIREMOCK";
}
```

Bound from configuration; sensitive values (`InboundBearerToken`, `WireMockBaseUrl`) overridden via environment variables in production.

---

## 13. `PluginNopStartup`

```csharp
public class PluginNopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICarrierWebhookAuditService,  CarrierWebhookAuditService>();
        services.AddScoped<IProcessedCarrierEventRepository, ProcessedCarrierEventRepository>();
        services.AddSingleton<IExternalStatusMapper, ExternalStatusMapper>();

        services.AddHttpClient<IWireMockClient, WireMockClient>();

        services.AddScoped<IConsumer<ShipmentSentEvent>, ShipmentSentEventConsumer>();

        services.AddHostedService<CarrierStatusConsumer>();
        services.AddHostedService<CarrierBookingConsumer>();
    }

    public void Configure(IApplicationBuilder application) { }

    public int Order => 4000;
}
```

- `BackgroundService` consumers are registered as `IHostedService`; the host starts them on app start
- `IConsumer<ShipmentSentEvent>` registration follows the existing nopCommerce convention; the framework discovers and invokes it when `ShipmentSentEvent` fires
- `AddHttpClient` enables typed-client features (named lifetime, default headers, Polly extensions if added later)

---

## 14. Schema Migration

```csharp
[NopMigration("2026-04-29 12:00:00", "Carrier webhook plugin: schema additions")]
public class CarrierWebhookSchemaMigration : Migration
{
    public override void Up()
    {
        Alter.Table("Shipment")
            .AddColumn("ExternalShipmentId").AsString(128).Nullable()
            .AddColumn("ExternalCarrierCode").AsString(32).Nullable()
            .AddColumn("ExternalShippingStatus").AsString(64).Nullable()
            .AddColumn("LastStatusOccurredAtUtc").AsCustom("datetime(6)").Nullable();

        Create.Index("IX_Shipment_ExternalShipmentId")
              .OnTable("Shipment")
              .OnColumn("ExternalCarrierCode").Ascending()
              .OnColumn("ExternalShipmentId").Ascending();

        Create.Table("CarrierWebhookEvent")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("EventId").AsString(64).Nullable()
            .WithColumn("ReceivedAtUtc").AsCustom("datetime(6)").NotNullable()
            .WithColumn("RawPayload").AsCustom("text").NotNullable()
            .WithColumn("RemoteIp").AsString(45).NotNullable()
            .WithColumn("Outcome").AsByte().NotNullable()
            .WithColumn("Notes").AsString(256).Nullable();

        Create.Index("IX_CarrierWebhookEvent_ReceivedAtUtc")
              .OnTable("CarrierWebhookEvent")
              .OnColumn("ReceivedAtUtc").Descending();

        Create.Index("IX_CarrierWebhookEvent_EventId")
              .OnTable("CarrierWebhookEvent")
              .OnColumn("EventId").Ascending();

        Create.Table("ProcessedCarrierEvent")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("EventId").AsString(64).NotNullable().Unique()
            .WithColumn("ProcessedAtUtc").AsCustom("datetime(6)").NotNullable()
            .WithColumn("ShipmentId").AsInt32().NotNullable();
    }

    public override void Down() { /* drop in reverse */ }
}
```

---

## 15. RabbitMQ Topology Declaration

Declared by the plugin on startup (called from `PluginNopStartup.Configure` via a hosted-service `IStartupTask`):

```csharp
channel.ExchangeDeclare("verdemart.carrier.booking", "direct", durable: true, autoDelete: false);
channel.QueueDeclare("verdemart.carrier.booking.requested", durable: true, exclusive: false, autoDelete: false);
channel.QueueBind("verdemart.carrier.booking.requested", "verdemart.carrier.booking", "carrier.booking.requested");

channel.ExchangeDeclare("verdemart.carrier.status",     "direct", durable: true, autoDelete: false);
channel.ExchangeDeclare("verdemart.carrier.status.dlx", "direct", durable: true, autoDelete: false);

var statusArgs = new Dictionary<string, object>
{
    { "x-dead-letter-exchange", "verdemart.carrier.status.dlx" }
};
channel.QueueDeclare("verdemart.carrier.status.received", durable: true, exclusive: false, autoDelete: false, arguments: statusArgs);
channel.QueueBind  ("verdemart.carrier.status.received", "verdemart.carrier.status", "carrier.status.received");

channel.QueueDeclare("verdemart.carrier.status.dlq", durable: true, exclusive: false, autoDelete: false);
channel.QueueBind  ("verdemart.carrier.status.dlq", "verdemart.carrier.status.dlx", "carrier.status.received");
```

The booking flow's DLQ topology is symmetric (omitted for brevity).

---

## 16. Email Template

A new `MessageTemplate` row inserted on plugin install:

| Field | Value |
| --- |---|
| `Name` | `ShipmentStatusUpdated.CustomerNotification` |
| `Subject` | `Update on your order %Order.OrderNumber%` |
| `Body` (excerpt) | `Hello %Customer.FullName%, your order is now: %Shipment.ExternalStatus%. %Shipment.ExternalStatusDescription%.` |

`%Shipment.ExternalStatus%` and `%Shipment.ExternalStatusDescription%` are new tokens registered by the plugin's `IMessageTokenProvider` extension. Existing tokens (`%Order.OrderNumber%`, `%Customer.FullName%`) are reused without change.

---

## Interface Dependency Map

```
[ Outbound — admin "create shipment" flow ]

ShipmentSentEvent
    │
    ▼
ShipmentSentEventConsumer       (IConsumer<ShipmentSentEvent>)
    depends on → IRepository<OutboxMessage>   (Iter 2)

[ Iter 2 OutboxDispatcherTask publishes — unchanged ]

CarrierBookingConsumer          (IHostedService — RabbitMQ)
    depends on → IShipmentService            (existing nopCommerce)
    depends on → IWireMockClient
    depends on → IOptions<CarrierWebhookSettings>

WireMockClient                  (typed HttpClient)
    depends on → HttpClient
    depends on → IOptions<CarrierWebhookSettings>


[ Inbound — carrier status update ]

CarrierWebhookController        (ASP.NET Core controller)
    depends on → ICarrierWebhookAuditService
    depends on → RabbitMQ channel (publish-only)
    depends on → IOptions<CarrierWebhookSettings>

CarrierStatusConsumer           (IHostedService — RabbitMQ)
    depends on → IShipmentService
    depends on → IProcessedCarrierEventRepository
    depends on → IExternalStatusMapper
    depends on → ICarrierWebhookAuditService
    depends on → IWorkflowMessageService     (existing nopCommerce)
    depends on → IDbContext                  (transaction scope)
    depends on → IOptions<CarrierWebhookSettings>


[ Shared ]

Shipment partial class with four new properties
RabbitMQ topology declared once on plugin startup
```

---

## What Step 6 Will Do

Step 6 produces the updated views (component + sequence) showing the outbound booking path and the inbound status-update path, then records the four architectural decisions this iteration produced as ADRs in `07-adrs/`.
