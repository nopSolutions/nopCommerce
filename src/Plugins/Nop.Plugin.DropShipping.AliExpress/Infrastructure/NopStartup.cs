using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.DropShipping.AliExpress.Services;

namespace Nop.Plugin.DropShipping.AliExpress.Infrastructure;

/// <summary>
/// Represents object for the configuring services on application startup
/// </summary>
public class NopStartup : INopStartup
{
    /// <summary>
    /// Add and configure any of the middleware
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Configuration of the application</param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register services
        services.AddScoped<IAliExpressService, AliExpressService>();
        services.AddScoped<IAliExpressProductMappingService, AliExpressProductMappingService>();
        services.AddScoped<IAliExpressOrderTrackingService, AliExpressOrderTrackingService>();

        // Register event consumers
        // services.AddScoped<IConsumer<OrderPlacedEvent>, OrderPlacedEventConsumer>();
        // services.AddScoped<IConsumer<EntityInsertedEvent<Product>>, ProductInsertedEventConsumer>();
        // services.AddScoped<IConsumer<EntityUpdatedEvent<Product>>, ProductUpdatedEventConsumer>();
        // services.AddScoped<IConsumer<EntityDeletedEvent<Product>>, ProductDeletedEventConsumer>();
    }

    /// <summary>
    /// Configure the using of added middleware
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public void Configure(IApplicationBuilder application)
    {
    }

    /// <summary>
    /// Gets order of this startup configuration implementation
    /// </summary>
    public int Order => 3000;
}
