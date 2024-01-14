using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Admin.Accounting.Services;

namespace Nop.Plugin.Admin.Accounting
{
    public class NopStartup : INopStartup
    {
        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 2; }
        }

        void INopStartup.Configure(IApplicationBuilder application)
        {
            
        }

        void INopStartup.ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEconomicGateway, EconomicGateway>();                       
        }
    }
}
