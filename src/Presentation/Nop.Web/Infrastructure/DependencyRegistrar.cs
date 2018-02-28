using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Web.Areas.Admin.Factories;
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

            //admin factories
            builder.RegisterType<ActivityLogModelFactory>().As<IActivityLogModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<BlogModelFactory>().As<IBlogModelFactory>().InstancePerLifetimeScope();

            //factories
            builder.RegisterType<Factories.AddressModelFactory>().As<Factories.IAddressModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.BlogModelFactory>().As<Factories.IBlogModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.CatalogModelFactory>().As<Factories.ICatalogModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.CheckoutModelFactory>().As<Factories.ICheckoutModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.CommonModelFactory>().As<Factories.ICommonModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.CountryModelFactory>().As<Factories.ICountryModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.CustomerModelFactory>().As<Factories.ICustomerModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.ForumModelFactory>().As<Factories.IForumModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.ExternalAuthenticationModelFactory>().As<Factories.IExternalAuthenticationModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.NewsModelFactory>().As<Factories.INewsModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.NewsletterModelFactory>().As<Factories.INewsletterModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.OrderModelFactory>().As<Factories.IOrderModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.PollModelFactory>().As<Factories.IPollModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.PrivateMessagesModelFactory>().As<Factories.IPrivateMessagesModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.ProductModelFactory>().As<Factories.IProductModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.ProfileModelFactory>().As<Factories.IProfileModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.ReturnRequestModelFactory>().As<Factories.IReturnRequestModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.ShoppingCartModelFactory>().As<Factories.IShoppingCartModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.TopicModelFactory>().As<Factories.ITopicModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.VendorModelFactory>().As<Factories.IVendorModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.WidgetModelFactory>().As<Factories.IWidgetModelFactory>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 2; }
        }
    }
}
