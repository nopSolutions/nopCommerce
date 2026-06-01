using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Fulfillment.OpenBoxes.Services;

namespace Nop.Plugin.Fulfillment.OpenBoxes.Infrastructure;

public class PluginNopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IOpenBoxesClient, OpenBoxesClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer(),
                AllowAutoRedirect = false
            });
        services.AddScoped<OpenBoxesStatusPollerTask>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 3110;
}
