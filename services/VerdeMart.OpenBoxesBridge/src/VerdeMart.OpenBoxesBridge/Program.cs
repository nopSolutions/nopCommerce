using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VerdeMart.OpenBoxesBridge;
using VerdeMart.OpenBoxesBridge.Dedup;
using VerdeMart.OpenBoxesBridge.Messaging;
using VerdeMart.OpenBoxesBridge.OpenBoxes;
using VerdeMart.OpenBoxesBridge.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables(prefix: "BRIDGE__");

builder.Services.Configure<BridgeSettings>(builder.Configuration.GetSection("Bridge"));
builder.Services.Configure<BridgeSettings>(builder.Configuration);

builder.Services.AddSingleton<IDedupRepository, DedupRepository>();
builder.Services.AddSingleton<RabbitMqTopology>();
builder.Services.AddSingleton<RetryCounter>();
builder.Services.AddScoped<OrderPlacedMessageConsumer>();

builder.Services.AddHttpClient<IOpenBoxesClient, OpenBoxesClient>();

builder.Services.AddHostedService<BridgeWorker>();

var host = builder.Build();
await host.RunAsync();
