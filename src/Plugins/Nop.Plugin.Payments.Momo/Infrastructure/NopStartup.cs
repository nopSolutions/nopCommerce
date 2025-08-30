using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Payments.Momo.Infrastructure;

public partial class NopStartup : INopStartup
{
    public int Order => 100;
    public void Configure(IApplicationBuilder application)
    {
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<MtnMomoConfigHttpClient>().WithProxy();
    }
}