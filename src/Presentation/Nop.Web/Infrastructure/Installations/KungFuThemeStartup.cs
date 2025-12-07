using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Web.Framework.Themes;

namespace Nop.Web.Infrastructure.Installations;

public partial class KungFuThemeStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<KungFuTopicSeeder>();
    }

    public void Configure(IApplicationBuilder application)
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        using var scope = application.ApplicationServices.CreateScope();
        var themeContext = scope.ServiceProvider.GetRequiredService<IThemeContext>();
        var workingTheme = themeContext.GetWorkingThemeNameAsync().GetAwaiter().GetResult();

        if (!string.Equals(workingTheme, "KungFu", StringComparison.OrdinalIgnoreCase))
            return;

        var seeder = scope.ServiceProvider.GetRequiredService<KungFuTopicSeeder>();
        seeder.SeedAsync().GetAwaiter().GetResult();
    }

    public int Order => 2100;
}
