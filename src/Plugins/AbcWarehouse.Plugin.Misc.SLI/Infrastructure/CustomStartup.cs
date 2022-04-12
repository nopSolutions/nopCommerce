using AbcWarehouse.Plugin.Misc.SLI.ViewEngines;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace AbcWarehouse.Plugin.Misc.SLI.Infrastructure
{
    public class CustomStartup : INopStartup
    {
        public int Order => int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CustomViewEngine());
            });
        }

        public void Configure(IApplicationBuilder application)
        {
        }
    }
}
