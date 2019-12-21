using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Tax.Avalara.Data;
using Nop.Plugin.Tax.Avalara.Domain;
using Nop.Plugin.Tax.Avalara.Factories;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Tax.Avalara.Infrastructure
{
    /// <summary>
    /// Dependency registrar of the Avalara tax provider services
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(IServiceCollection services, ITypeFinder typeFinder, NopConfig config)
        {
            //register overridden services and factories
            services.AddScoped<IOrderProcessingService, OverriddenOrderProcessingService>();
            services.AddScoped<IOrderTotalCalculationService, OverriddenOrderTotalCalculationService>();
            services.AddScoped<Web.Factories.IShoppingCartModelFactory, OverriddenShoppingCartModelFactory>();
            services.AddScoped<ITaxModelFactory, OverriddenTaxModelFactory>();

            //register custom services
            services.AddScoped<AvalaraTaxManager>();
            services.AddScoped<TaxTransactionLogService>();

            //register custom data context
            services.RegisterPluginDataContext<TaxTransactionLogObjectContext>(AvalaraTaxDefaults.ObjectContextName);
            services.AddScoped(typeof(IRepository<TaxTransactionLog>), typeof(EfRepository<TaxTransactionLog>));
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 4;
    }
}