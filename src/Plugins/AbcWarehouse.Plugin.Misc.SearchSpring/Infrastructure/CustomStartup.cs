using AbcWarehouse.Plugin.Misc.SearchSpring;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace AbcWarehouse.Plugin.Misc.SearchSpring.Infrastructure
{
    public class CustomStartup : INopStartup
    {
        public int Order => int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {

            });
        }

        public void Configure(IApplicationBuilder application)
        {
        }
    }
}
