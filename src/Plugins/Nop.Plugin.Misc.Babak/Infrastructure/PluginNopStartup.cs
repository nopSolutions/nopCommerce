using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.Babak.Services;

namespace Nop.Plugin.Misc.Babak.Infrastructure;

/// <summary>
/// Represents the object for the configuring services on application startup.
/// </summary>
public class PluginNopStartup : INopStartup
{
    /// <summary>
    /// Add and configure any of the middleware.
    /// </summary>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IBabakItemService, BabakItemService>();
    }

    /// <summary>
    /// Configure the using of added middleware.
    /// </summary>
    public void Configure(IApplicationBuilder application)
    {
    }

    /// <summary>
    /// Gets order of this startup configuration implementation.
    /// </summary>
    public int Order => 3000;
}
