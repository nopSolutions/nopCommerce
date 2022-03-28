using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.AbcFrontend.ViewEngines;

namespace Nop.Plugin.Misc.AbcFrontend.Infrastructure
{
    public class CustomStartup : INopStartup
    {
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

        public int Order
        {
            get { return int.MaxValue; }
        }
    }
}
