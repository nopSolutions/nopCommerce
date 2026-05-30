using Omnichannel.Worker;

var builder = Host.CreateApplicationBuilder(args);

var options = new WorkerOptions();
builder.Configuration.GetSection(WorkerOptions.SectionName).Bind(options);
builder.Services.AddSingleton(options);

builder.Services.AddHttpClient<WmsClient>();
builder.Services.AddHostedService<OrderPlacedConsumer>();

var host = builder.Build();
host.Run();
