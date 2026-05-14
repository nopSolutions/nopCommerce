using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Inventory.AllocationGate.Services;

namespace Nop.Plugin.Inventory.AllocationGate.Infrastructure;

public class PluginNopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProductReservationRepository, ProductReservationRepository>();
        services.AddScoped<IAllocationGate, AllocationGateService>();
        services.AddScoped<ReleaseExpiredReservationsTask>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 3100;
}
