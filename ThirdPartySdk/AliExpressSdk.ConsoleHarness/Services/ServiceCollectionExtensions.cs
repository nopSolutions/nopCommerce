using Microsoft.Extensions.DependencyInjection;
using AliExpressSdk.Services;
using AliExpressSdk.ConsoleHarness.Commands;

namespace AliExpressSdk.ConsoleHarness.Services;

/// <summary>
/// Extension methods for registering application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Core services
        services.AddSingleton<AuthorizationUrlBuilder>();
        services.AddSingleton<ConsolePrompt>();
        services.AddSingleton<ApiCallPersistence>();
        services.AddSingleton<AuthenticationHandler>();
        
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<Configuration.AliExpressOptions>>();
            return new SignatureService(options.Value.AppSecret);
        });
        
        // Commands
        services.AddTransient<AuthorizeCommand>();
        services.AddTransient<ApiCallCommand>();
        services.AddTransient<ProductSearchCommand>();
        services.AddTransient<CreateOrderWorkflowCommand>();
        
        return services;
    }
}
