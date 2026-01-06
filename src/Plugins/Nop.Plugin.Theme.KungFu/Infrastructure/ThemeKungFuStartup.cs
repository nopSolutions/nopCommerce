using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Theme.KungFu.Seeding.Category;
using Nop.Plugin.Theme.KungFu.Seeding.Email;
using Nop.Plugin.Theme.KungFu.Seeding.JkooSword;
using Nop.Plugin.Theme.KungFu.Services;

namespace Nop.Plugin.Theme.KungFu.Infrastructure;

public class ThemeKungFuStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddScoped<IThemeKungFuService, ThemeKungFuService>();
        services.AddScoped<IAISageService, AISageService>();
        services.AddScoped<ICategorySeeds, CategorySeeds>();
        services.AddScoped<IEmailClientSeeds, EmailClientSeeds>();
        services.AddScoped<IJkooStoreSyncService, JkooStoreSyncService>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 3000;
}
