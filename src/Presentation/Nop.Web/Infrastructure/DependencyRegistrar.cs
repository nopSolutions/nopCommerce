using Autofac;
using Autofac.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Web.Factories;
using Nop.Web.Infrastructure.Installation;

namespace Nop.Web.Infrastructure
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
            //installation localization service
            builder.RegisterType<InstallationLocalizationService>().As<IInstallationLocalizationService>().InstancePerLifetimeScope();

            //factories
            builder.RegisterType<AddressModelFactory>().As<IAddressModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<BlogModelFactory>().As<IBlogModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CatalogModelFactory>().As<ICatalogModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutModelFactory>().As<ICheckoutModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CommonModelFactory>().As<ICommonModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CountryModelFactory>().As<ICountryModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerModelFactory>().As<ICustomerModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ForumModelFactory>().As<IForumModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ExternalAuthenticationModelFactory>().As<IExternalAuthenticationModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<NewsModelFactory>().As<INewsModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<NewsletterModelFactory>().As<INewsletterModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<OrderModelFactory>().As<IOrderModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<PollModelFactory>().As<IPollModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<PrivateMessagesModelFactory>().As<IPrivateMessagesModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ProductModelFactory>().As<IProductModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileModelFactory>().As<IProfileModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ReturnRequestModelFactory>().As<IReturnRequestModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartModelFactory>().As<IShoppingCartModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<TopicModelFactory>().As<ITopicModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<VendorModelFactory>().As<IVendorModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<WidgetModelFactory>().As<IWidgetModelFactory>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation (less is better)
        /// </summary>
        public int Order
        {
            get { return 2; }
        }
    }
}
