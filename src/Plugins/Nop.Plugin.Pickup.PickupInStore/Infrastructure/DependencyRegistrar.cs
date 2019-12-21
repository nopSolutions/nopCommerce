using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Pickup.PickupInStore.Data;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using Nop.Plugin.Pickup.PickupInStore.Factories;
using Nop.Plugin.Pickup.PickupInStore.Services;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Pickup.PickupInStore.Infrastructure
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
            services.AddScoped<IStorePickupPointService, StorePickupPointService>();
            services.AddScoped<IStorePickupPointModelFactory, StorePickupPointModelFactory>();

            //data context
            services.RegisterPluginDataContext<StorePickupPointObjectContext>("nop_object_context_pickup_in_store-pickup");

            //override required repository with our custom context
            services.AddScoped(typeof(IRepository<StorePickupPoint>), typeof(EfRepository<StorePickupPoint>));
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}