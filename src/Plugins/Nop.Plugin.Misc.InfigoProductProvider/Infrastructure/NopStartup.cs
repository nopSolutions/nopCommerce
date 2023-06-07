using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.InfigoProductProvider.Api;
using Nop.Plugin.Misc.InfigoProductProvider.Services;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Misc.InfigoProductProvider.Infrastructure;

public class NopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IInfigoProductProviderService, InfigoProductProviderService>();
        services.AddHttpClient<InfigoProductProviderHttpClient>().WithProxy();
    }

    public void Configure(IApplicationBuilder application)
    {
        
    }

    public int Order => 3000;
}