using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Integration;
using Nop.Services.Integration.RabbitMQ;

namespace Nop.Web.Framework.Infrastructure;

/// <summary>
/// Represents object for the configuring integration services on application startup
/// </summary>
public partial class IntegrationStartup : INopStartup
{
    /// <summary>
    /// Add and configure any of the middleware
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Configuration of the application</param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISpikeOutboxService, SpikeOutboxService>();
        services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();
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
