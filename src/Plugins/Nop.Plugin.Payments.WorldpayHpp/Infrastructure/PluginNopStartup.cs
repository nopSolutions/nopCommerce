using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.WorldpayHpp.Handlers;
using Nop.Plugin.Payments.WorldpayHpp.Services;

namespace Nop.Plugin.Payments.WorldpayHpp.Infrastructure;
public class PluginNopStartup : INopStartup
{
    /// <summary>
    /// Add and configure any of the middleware
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Configuration of the application</param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
       
        services.AddScoped<WorldpayHppService>();
        services.AddScoped<WorldpayHppPaymentProcessor>();
        services.AddScoped<IWorldpayWebhookHandler, WorldpayWebhookHandler>();
        services.AddScoped<IPaymentEventHandler, PaymentEventHandler>();
        services.AddScoped<ITokenCreatedEventHandler, TokenCreatedEventHandler>();
    }

    /// <summary>
    /// Configure the using of added middleware
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public void Configure(IApplicationBuilder application)
    {
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute("WorldpayHpp.Success",
            "WorldpayHpp/Success",
            new { controller = "WorldpayHpp", action = "Success" });

        endpointRouteBuilder.MapControllerRoute("WorldpayHpp.Cancel",
            "WorldpayHpp/Cancel",
            new { controller = "WorldpayHpp", action = "Cancel" });

        endpointRouteBuilder.MapControllerRoute("WorldpayHpp.Callback",
            "WorldpayHpp/Callback",
            new { controller = "WorldpayHpp", action = "Callback" });

        endpointRouteBuilder.MapControllerRoute("WorldpayHpp.Configure",
            "Admin/WorldpayHpp/Configure",
            new { controller = "WorldpayHpp", action = "Configure" });
    }

    /// <summary>
    /// Gets order of this startup configuration implementation
    /// </summary>
    public int Order => 3000;
}