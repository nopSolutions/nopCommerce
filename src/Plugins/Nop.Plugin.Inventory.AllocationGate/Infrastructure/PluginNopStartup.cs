using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Inventory.AllocationGate.OpenBoxes;
using Nop.Plugin.Inventory.AllocationGate.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;

namespace Nop.Plugin.Inventory.AllocationGate.Infrastructure;

public class PluginNopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProductReservationRepository, ProductReservationRepository>();
        services.AddScoped<IAllocationGate, AllocationGateService>();
        services.AddScoped<ReleaseExpiredReservationsTask>();
        services.AddHttpClient<IOpenBoxesClient, OpenBoxesClient>();
        services.AddScoped<OpenBoxesStatusPollerTask>();

        // Descriptor swap: wrap IProductService with AllocationGateProductServiceDecorator.
        // NopStartup (Order=2000) registers IProductService before this plugin (Order=3100),
        // so the descriptor is guaranteed to exist here.
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IProductService));
        if (descriptor?.ImplementationType is not null)
        {
            services.Remove(descriptor);

            // Register the concrete type directly so the decorator can resolve it as its inner
            services.Add(ServiceDescriptor.Describe(
                descriptor.ImplementationType,
                descriptor.ImplementationType,
                descriptor.Lifetime));

            // Register IProductService as the decorator wrapping the concrete type
            services.Add(ServiceDescriptor.Describe(
                typeof(IProductService),
                sp => AllocationGateProductServiceDecorator.Create(
                    (IProductService)sp.GetRequiredService(descriptor.ImplementationType),
                    sp.GetRequiredService<IAllocationGate>(),
                    sp.GetRequiredService<ILogger<AllocationGateProductServiceDecorator>>(),
                    sp.GetRequiredService<ISettingService>(),
                    sp.GetRequiredService<IStoreContext>()),
                descriptor.Lifetime));
        }
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 3100;
}
