using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Events;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Messaging.RabbitMq.Services;

namespace Nop.Plugin.Messaging.RabbitMq.Infrastructure;

public class PluginNopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
        services.AddScoped<IConsumer<OrderPlacedEvent>, OrderPlacedConsumer>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 3000;
}
