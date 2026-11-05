using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.PayPalCommerce.Factories;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Payments.PayPalCommerce.Infrastructure;

/// <summary>
/// Represents the object for the configuring services on application startup
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
        //remove the PayPal SDK assembly from MVC application parts to prevent
        //Autofac from trying to register SDK internal "Controller" types (e.g. BaseController)
        //which have no public constructors and are not MVC controllers
        var partManager = services
            .FirstOrDefault(d => d.ServiceType == typeof(ApplicationPartManager))
            ?.ImplementationInstance as ApplicationPartManager;
        var sdkPart = partManager?.ApplicationParts
            .FirstOrDefault(p => p.Name.Equals("PayPalServerSDK", StringComparison.OrdinalIgnoreCase));
        if (sdkPart != null)
            partManager.ApplicationParts.Remove(sdkPart);

        services.AddSingleton<PayPalSdkClientFactory>();
        services.AddHttpClient<OnboardingHttpClient>().WithProxy();
        services.AddHttpClient<PayPalCommerceHttpClient>().WithProxy();
        services.AddScoped<PayPalCommerceModelFactory>();
        services.AddScoped<PayPalCommerceServiceManager>();
        services.AddScoped<PayPalTokenService>();
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
    public int Order => 1;
}