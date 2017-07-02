using System;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Localization;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Events;
using Nop.Web.Factories;
using Nop.Web.Infrastructure.Cache;
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
        /// <param name="services">Services</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(IServiceCollection services, ITypeFinder typeFinder, NopConfig config)
        {
            //installation localization service
            services.AddSingleton<IInstallationLocalizationService, InstallationLocalizationService>();

            //factories
            services.AddSingleton<IAddressModelFactory, AddressModelFactory>();
            services.AddSingleton<IBlogModelFactory, BlogModelFactory>();
            services.AddSingleton<ICatalogModelFactory, CatalogModelFactory>();
            services.AddSingleton<ICheckoutModelFactory, CheckoutModelFactory>();
            services.AddSingleton<ICommonModelFactory, CommonModelFactory>();
            services.AddSingleton<ICountryModelFactory, CountryModelFactory>();
            services.AddSingleton<ICustomerModelFactory, CustomerModelFactory>();
            services.AddSingleton<IForumModelFactory, ForumModelFactory>();
            services.AddSingleton<IExternalAuthenticationModelFactory, ExternalAuthenticationModelFactory>();
            services.AddSingleton<INewsModelFactory, NewsModelFactory>();
            services.AddSingleton<INewsletterModelFactory, NewsletterModelFactory>();
            services.AddSingleton<IOrderModelFactory, OrderModelFactory>();
            services.AddSingleton<IPollModelFactory, PollModelFactory>();
            services.AddSingleton<IPrivateMessagesModelFactory, PrivateMessagesModelFactory>();
            services.AddSingleton<IProductModelFactory, ProductModelFactory>();
            services.AddSingleton<IProfileModelFactory, ProfileModelFactory>();
            services.AddSingleton<IReturnRequestModelFactory, ReturnRequestModelFactory>();
            services.AddSingleton<IShoppingCartModelFactory, ShoppingCartModelFactory>();
            services.AddSingleton<ITopicModelFactory, TopicModelFactory>();
            services.AddSingleton<IVendorModelFactory, VendorModelFactory>();
            services.AddSingleton<IWidgetModelFactory, WidgetModelFactory>();
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
