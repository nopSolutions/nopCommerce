using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.Paystack.Services;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Payments.Paystack.Infrastructure;

/// <summary>
/// Paystack plugin startup
/// </summary>
public class NopStartup : INopStartup
{
    /// <inheritdoc />
    public int Order => 100;

    /// <inheritdoc />
    public void Configure(IApplicationBuilder application)
    {
    }

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // HTTP clients
        services.AddHttpClient<PaystackPaymentClient>().WithProxy();

        // Settings (loaded per-request where needed; controller uses ISettingService for load/save)
        services.AddScoped<PaystackPaymentSettings>();

        // Services
        services.AddScoped<IPaystackTransactionService, PaystackTransactionService>();
    }
}
