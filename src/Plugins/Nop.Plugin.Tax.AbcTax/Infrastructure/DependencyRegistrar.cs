using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Tax.AbcTax.Services;
using Nop.Services.Tax;
using Nop.Services.Orders;

namespace Nop.Plugin.Tax.AbcTax.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="appSettings">App settings</param>
        public virtual void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            services.AddScoped<ITaxProvider, AbcTaxProvider>();
            services.AddScoped<IAbcTaxService, AbcTaxService>();
            services.AddScoped<ITaxjarRateService, TaxjarRateService>();
            services.AddScoped<IWarrantyTaxService, WarrantyTaxService>();
            services.AddScoped<IOrderProcessingService, CustomOrderProcessingService>();
            services.AddScoped<IOrderTotalCalculationService, CustomOrderTotalCalculationService>();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}