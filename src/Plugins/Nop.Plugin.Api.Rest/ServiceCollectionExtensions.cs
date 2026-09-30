using Microsoft.Extensions.DependencyInjection;

namespace Nop.Plugin.Api.Rest
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNopApiRest(this IServiceCollection services)
        {
            // register plugin-scoped services here, e.g.
            // services.AddScoped<IYourPluginService, YourPluginService>();
            return services;
        }
    }
}
