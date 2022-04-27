using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.PolyCommerce.Factories;
using Nop.Web.Areas.Admin.Factories;

namespace Nop.Plugin.Misc.PolyCommerce.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => 2;

        public void Configure(IApplicationBuilder application)
        {
            
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IProductModelFactory, PolyCommerceProductModelFactory>();
        }
    }
}
