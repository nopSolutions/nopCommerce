using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AliExpressSdk.ConsoleHarness.Configuration;

/// <summary>
/// Extension methods for configuring application settings.
/// </summary>
public static class ConfigurationExtensions
{
    public static IServiceCollection AddAppConfiguration(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<AliExpressOptions>(
            configuration.GetSection(AliExpressOptions.SectionName));
        
        services.Configure<OutputOptions>(
            configuration.GetSection(OutputOptions.SectionName));
        
        return services;
    }
}
