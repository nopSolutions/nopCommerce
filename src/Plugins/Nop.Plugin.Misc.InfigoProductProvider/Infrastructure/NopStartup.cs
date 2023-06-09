using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.InfigoProductProvider.Api;
using Nop.Plugin.Misc.InfigoProductProvider.Mapping;
using Nop.Plugin.Misc.InfigoProductProvider.Services;

namespace Nop.Plugin.Misc.InfigoProductProvider.Infrastructure;

public class NopStartup : INopStartup
{
    private readonly ILogger<NopStartup> _logger;

    public NopStartup(ILogger<NopStartup> logger)
    {
        _logger = logger;
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        _logger.LogInformation("Configuring services");
        
        services.AddScoped<IInfigoProductProviderService, InfigoProductProviderService>();
        services.AddScoped<IProductMapper, ProductMapper>();
        services.AddHttpClient<InfigoProductProviderHttpClient>();
    }

    public void Configure(IApplicationBuilder application)
    {
        
    }

    public int Order => 3000;
}