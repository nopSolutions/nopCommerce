using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.InfigoProductProvider.AutoMapperProfiles;
using Nop.Plugin.Misc.InfigoProductProvider.Factories;
using Nop.Plugin.Misc.InfigoProductProvider.Models;

namespace Nop.Plugin.Misc.InfigoProductProvider.Infrastructure;

public class NopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IConfigurationModelFactory, ConfigurationModelFactory>();

        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<ConfigurationProfile>();
        });
    }

    public void Configure(IApplicationBuilder application)
    {
        
    }

    public int Order => 3000;
}