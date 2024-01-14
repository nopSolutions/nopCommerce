using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.POS.Kaching.Models;
using Nop.Plugin.POS.Kaching.Services;

namespace Nop.Plugin.POS.Kaching.Infrastructure
{
    /// <summary>
    /// Represents a plugin dependency registrar
    /// </summary>
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //register custom services            
            
            services.AddScoped<IModelValidation, ModelValidation>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IReconciliationService, ReconciliationService>();
            services.AddScoped<IPOSKachingService, POSKachingService>();
            services.AddScoped<ISalesService, SalesService>();
            
        }

        public void Configure(IApplicationBuilder application)
        {

        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 2;
    }
}