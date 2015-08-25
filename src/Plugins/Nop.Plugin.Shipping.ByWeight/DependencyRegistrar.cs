using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Shipping.ByWeight.Data;
using Nop.Plugin.Shipping.ByWeight.Domain;
using Nop.Plugin.Shipping.ByWeight.Services;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Shipping.ByWeight
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<ShippingByWeightService>().As<IShippingByWeightService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<ShippingByWeightObjectContext>(builder, "nop_object_context_shipping_weight_zip");

            //override required repository with our custom context
            builder.RegisterType<EfRepository<ShippingByWeightRecord>>()
                .As<IRepository<ShippingByWeightRecord>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_shipping_weight_zip"))
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 1; }
        }
    }
}
