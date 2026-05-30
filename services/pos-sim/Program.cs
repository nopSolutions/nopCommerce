using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

const string demoTokenHeaderName = "X-Demo-Token";
const string eventType = "pos.stock.changed.v1";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("nopcommerce", (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["NopCommerce:BaseUrl"] ?? "http://localhost";
    var demoToken = configuration["OmnichannelCore:DemoToken"] ?? "omni-demo-token";

    client.BaseAddress = new Uri(baseUrl.TrimEnd('/'));
    client.DefaultRequestHeaders.Add(demoTokenHeaderName, demoToken);
});

var app = builder.Build();
var simulatorState = new PosSimulatorState();
var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    simulator = "pos-sim",
    mode = simulatorState.Mode
}));

app.MapGet("/mode", () => Results.Ok(new
{
    mode = simulatorState.Mode
}));

app.MapPost("/mode/{mode}", (string mode) =>
{
    if (!PosModes.IsValid(mode))
        return Results.BadRequest(new { error = $"Unsupported mode '{mode}'" });

    simulatorState.Mode = mode;
    return Results.Ok(new { mode = simulatorState.Mode });
});

app.MapPost("/emit", async (PosEmitRequest request, IHttpClientFactory httpClientFactory, IConfiguration configuration) =>
{
    var mode = string.IsNullOrWhiteSpace(request.Mode) ? simulatorState.Mode : request.Mode;

    if (!PosModes.IsValid(mode))
        return Results.BadRequest(new { error = $"Unsupported mode '{mode}'" });

    var endpoint = configuration["NopCommerce:PosStockChangedPath"] ?? "/omnichannel/callbacks/pos/stock-changed";
    var events = BuildEvents(mode, request, simulatorState);
    var client = httpClientFactory.CreateClient("nopcommerce");
    var results = new List<PosPostResult>();

    foreach (var stockEvent in events)
        results.Add(await PostEventAsync(client, endpoint, stockEvent, jsonOptions));

    return Results.Ok(new
    {
        mode,
        target = $"{client.BaseAddress}{endpoint.TrimStart('/')}",
        results
    });
});

app.Run();

static IReadOnlyList<PosStockChangedEvent> BuildEvents(string mode, PosEmitRequest request, PosSimulatorState simulatorState)
{
    var sourceVersion = request.SourceVersion ?? simulatorState.NextSourceVersion();
    var firstEvent = CreateEvent(request, sourceVersion, request.QuantityOnHand);

    return mode switch
    {
        PosModes.Normal => [firstEvent],
        PosModes.Duplicate => [firstEvent, firstEvent],
        PosModes.Stale => [firstEvent, CreateEvent(request, Math.Max(0, sourceVersion - 1), request.QuantityOnHand + 7)],
        _ => throw new InvalidOperationException($"Unsupported mode '{mode}'")
    };
}

static PosStockChangedEvent CreateEvent(PosEmitRequest request, long sourceVersion, int quantityOnHand)
{
    var messageId = Guid.NewGuid();

    return new PosStockChangedEvent
    {
        MessageId = messageId,
        CorrelationId = request.CorrelationId ?? messageId.ToString("D"),
        EventType = eventType,
        OccurredOnUtc = DateTime.UtcNow,
        Source = request.Source,
        SourceVersion = sourceVersion,
        ProductId = request.ProductId,
        Sku = request.Sku,
        WarehouseId = request.WarehouseId,
        QuantityOnHand = quantityOnHand
    };
}

static async Task<PosPostResult> PostEventAsync(HttpClient client,
    string endpoint,
    PosStockChangedEvent stockEvent,
    JsonSerializerOptions jsonOptions)
{
    using var response = await client.PostAsJsonAsync(endpoint, stockEvent, jsonOptions);
    var body = await response.Content.ReadAsStringAsync();

    return new PosPostResult
    {
        MessageId = stockEvent.MessageId,
        SourceVersion = stockEvent.SourceVersion,
        QuantityOnHand = stockEvent.QuantityOnHand,
        StatusCode = (int)response.StatusCode,
        Success = response.StatusCode is HttpStatusCode.OK,
        ResponseBody = body
    };
}

static class PosModes
{
    public const string Normal = "normal";
    public const string Duplicate = "duplicate";
    public const string Stale = "stale";

    public static bool IsValid(string mode)
    {
        return mode is Normal or Duplicate or Stale;
    }
}

sealed class PosSimulatorState
{
    private long _sourceVersion = 40;

    public string Mode { get; set; } = PosModes.Normal;

    public long NextSourceVersion()
    {
        return Interlocked.Increment(ref _sourceVersion);
    }
}

sealed class PosEmitRequest
{
    public string Mode { get; set; } = string.Empty;

    public string CorrelationId { get; set; } = string.Empty;

    public string Source { get; set; } = "store-pos-porto";

    public long? SourceVersion { get; set; }

    public int ProductId { get; set; } = 15;

    public string Sku { get; set; } = "LAPTOP-15";

    public int WarehouseId { get; set; } = 2;

    public int QuantityOnHand { get; set; } = 3;
}

sealed class PosStockChangedEvent
{
    public Guid MessageId { get; set; }

    public string CorrelationId { get; set; } = string.Empty;

    public string EventType { get; set; } = string.Empty;

    public DateTime OccurredOnUtc { get; set; }

    public string Source { get; set; } = string.Empty;

    public long SourceVersion { get; set; }

    public int ProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public int QuantityOnHand { get; set; }
}

sealed class PosPostResult
{
    public Guid MessageId { get; set; }

    public long SourceVersion { get; set; }

    public int QuantityOnHand { get; set; }

    public int StatusCode { get; set; }

    public bool Success { get; set; }

    public string ResponseBody { get; set; } = string.Empty;
}
