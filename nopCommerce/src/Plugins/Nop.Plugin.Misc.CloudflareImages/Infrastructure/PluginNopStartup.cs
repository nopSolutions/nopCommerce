using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.CloudflareImages.Services;
using Nop.Services.Media;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Misc.CloudflareImages.Infrastructure;

/// <summary>
/// Represents the object for the configuring services on application startup
/// </summary>
public class PluginNopStartup : INopStartup
{
    /// <summary>
    /// Add and configure any of the middleware
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Configuration of the application</param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<CloudflareImagesHttpClient>().WithProxy();
        services.AddTransient<CloudflareThumbService>();
        services.AddTransient<ThumbService>();
        services.AddTransient<IThumbService>(provider =>
        {
            var settings = provider.GetRequiredService<CloudflareImagesSettings>();

            if (settings.Enabled && !string.IsNullOrEmpty(settings.AccessToken))
                return provider.GetRequiredService<CloudflareThumbService>();

            return provider.GetRequiredService<ThumbService>();
        });
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