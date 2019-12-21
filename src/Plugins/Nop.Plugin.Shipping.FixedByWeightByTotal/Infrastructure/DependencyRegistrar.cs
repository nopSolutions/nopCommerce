using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Data;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Services;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Infrastructure
{
    /// <summary>
    /// Dependency registrar
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
            services.AddScoped<IShippingByWeightByTotalService, ShippingByWeightByTotalService>();

            //data context
            services.RegisterPluginDataContext<ShippingByWeightByTotalObjectContext>("nop_object_context_shipping_weight_total_zip");

            //override required repository with our custom context
            services.AddScoped(typeof(IRepository<ShippingByWeightByTotalRecord>), typeof(EfRepository<ShippingByWeightByTotalRecord>));
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}