using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace AbcWarehouse.Plugin.Misc.Redirect.Infrastructure
{
    public class CustomStartup : INopStartup
    {
        public int Order => int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseMiddleware<RedirectMiddleware>();
        }
    }
}
