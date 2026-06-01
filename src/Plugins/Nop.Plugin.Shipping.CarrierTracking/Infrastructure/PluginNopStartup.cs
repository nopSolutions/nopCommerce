using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Shipping.CarrierTracking.Services;

namespace Nop.Plugin.Shipping.CarrierTracking.Infrastructure;

public class PluginNopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IWireMockClient, WireMockClient>();
        services.AddHostedService<CarrierBookingConsumer>();
        services.AddSingleton<IExternalStatusMapper, ExternalStatusMapper>();
        services.AddScoped<CarrierStatusPollerTask>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 3200;
}
