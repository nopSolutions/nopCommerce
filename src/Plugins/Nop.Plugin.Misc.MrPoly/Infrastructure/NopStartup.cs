using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.MrPoly.Catalog;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.MrPoly.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => 2;

        public void Configure(IApplicationBuilder application)
        {
            
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IProductService, PolyProductService>();
        }
    }
}
