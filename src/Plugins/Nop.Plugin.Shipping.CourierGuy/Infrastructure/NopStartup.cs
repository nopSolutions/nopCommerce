using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Shipping.CourierGuy.Domain.NopEntityMappers;
using Nop.Plugin.Shipping.CourierGuy.Services;

namespace Nop.Plugin.Shipping.CourierGuy.Infrastructure;

public class NopStartup : INopStartup
{
    public void ConfigureServices(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ICourierGuyNopEntityMapper, CourierGuyNopEntityMapper>();
        services.AddSingleton<ICourierShipmentService, CourierGuyShipmentService>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order { get; } = 3000;
}