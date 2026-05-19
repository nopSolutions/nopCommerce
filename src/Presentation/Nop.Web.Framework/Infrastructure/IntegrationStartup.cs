using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Integration;
using Nop.Services.Integration.RabbitMQ;

namespace Nop.Web.Framework.Infrastructure;

/// <summary>
/// Wires the omnichannel integration layer: outbox writer, RabbitMQ publisher,
/// and the inbound stock-update consumer.
/// </summary>
public partial class IntegrationStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(_ => Singleton<AppSettings>.Instance.Get<IntegrationConfig>());

        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();

        services.AddHostedService<StockUpdateConsumerBackgroundService>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 3000;
}
