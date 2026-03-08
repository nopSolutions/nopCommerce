using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.Momo.Services;
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
        //http clients
        services.AddHttpClient<MtnMomoConfigHttpClient>().WithProxy();
        services.AddHttpClient<MomoPaymentClient>().WithProxy();

        //settings
        services.AddScoped<MomoPaymentSettings>();

        //services
        services.AddScoped<IMomoTransactionService, MomoTransactionService>();
        services.AddScoped<IMomoPaymentService, MomoPaymentService>();
    }
}
