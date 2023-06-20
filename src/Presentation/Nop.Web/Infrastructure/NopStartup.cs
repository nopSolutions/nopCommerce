using Nop.Core.Infrastructure;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Helpers;
using Nop.Web.Factories;
using Nop.Web.Framework.Factories;
using Nop.Web.Infrastructure.Installation;
using AddressModelFactory = Nop.Web.Areas.Admin.Factories.AddressModelFactory;
using BlogModelFactory = Nop.Web.Areas.Admin.Factories.BlogModelFactory;
using CommonModelFactory = Nop.Web.Areas.Admin.Factories.CommonModelFactory;
using CountryModelFactory = Nop.Web.Areas.Admin.Factories.CountryModelFactory;
using CustomerModelFactory = Nop.Web.Areas.Admin.Factories.CustomerModelFactory;
using ForumModelFactory = Nop.Web.Areas.Admin.Factories.ForumModelFactory;
using IAddressModelFactory = Nop.Web.Areas.Admin.Factories.IAddressModelFactory;
using IBlogModelFactory = Nop.Web.Areas.Admin.Factories.IBlogModelFactory;
using ICommonModelFactory = Nop.Web.Areas.Admin.Factories.ICommonModelFactory;
using ICountryModelFactory = Nop.Web.Areas.Admin.Factories.ICountryModelFactory;
using ICustomerModelFactory = Nop.Web.Areas.Admin.Factories.ICustomerModelFactory;
using IForumModelFactory = Nop.Web.Areas.Admin.Factories.IForumModelFactory;
using INewsModelFactory = Nop.Web.Areas.Admin.Factories.INewsModelFactory;
using IOrderModelFactory = Nop.Web.Areas.Admin.Factories.IOrderModelFactory;
using IPollModelFactory = Nop.Web.Areas.Admin.Factories.IPollModelFactory;
using IProductModelFactory = Nop.Web.Areas.Admin.Factories.IProductModelFactory;
using IReturnRequestModelFactory = Nop.Web.Areas.Admin.Factories.IReturnRequestModelFactory;
using IShoppingCartModelFactory = Nop.Web.Areas.Admin.Factories.IShoppingCartModelFactory;
using ITopicModelFactory = Nop.Web.Areas.Admin.Factories.ITopicModelFactory;
using IVendorModelFactory = Nop.Web.Areas.Admin.Factories.IVendorModelFactory;
using IWidgetModelFactory = Nop.Web.Areas.Admin.Factories.IWidgetModelFactory;
using NewsModelFactory = Nop.Web.Areas.Admin.Factories.NewsModelFactory;
using OrderModelFactory = Nop.Web.Areas.Admin.Factories.OrderModelFactory;
using PollModelFactory = Nop.Web.Areas.Admin.Factories.PollModelFactory;
using ProductModelFactory = Nop.Web.Areas.Admin.Factories.ProductModelFactory;
using ReturnRequestModelFactory = Nop.Web.Areas.Admin.Factories.ReturnRequestModelFactory;
using ShoppingCartModelFactory = Nop.Web.Areas.Admin.Factories.ShoppingCartModelFactory;
using TopicModelFactory = Nop.Web.Areas.Admin.Factories.TopicModelFactory;
using VendorModelFactory = Nop.Web.Areas.Admin.Factories.VendorModelFactory;
using WidgetModelFactory = Nop.Web.Areas.Admin.Factories.WidgetModelFactory;

namespace Nop.Web.Infrastructure
{
    /// <summary>
    /// Represents the registering services on application startup
    /// </summary>
    public partial class NopStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //installation localization service
            services.AddScoped<IInstallationLocalizationService, InstallationLocalizationService>();

            //common factories
            services.AddScoped<IAclSupportedModelFactory, AclSupportedModelFactory>();
            services.AddScoped<IDiscountSupportedModelFactory, DiscountSupportedModelFactory>();
            services.AddScoped<ILocalizedModelFactory, LocalizedModelFactory>();
            services.AddScoped<IStoreMappingSupportedModelFactory, StoreMappingSupportedModelFactory>();

            //admin factories
            services.AddScoped<IBaseAdminModelFactory, BaseAdminModelFactory>();
            services.AddScoped<IActivityLogModelFactory, ActivityLogModelFactory>();
            services.AddScoped<IAddressModelFactory, AddressModelFactory>();
            services.AddScoped<IAddressAttributeModelFactory, AddressAttributeModelFactory>();
            services.AddScoped<IAffiliateModelFactory, AffiliateModelFactory>();
            services.AddScoped<IBlogModelFactory, BlogModelFactory>();
            services.AddScoped<ICampaignModelFactory, CampaignModelFactory>();
            services.AddScoped<ICategoryModelFactory, CategoryModelFactory>();
            services.AddScoped<ICheckoutAttributeModelFactory, CheckoutAttributeModelFactory>();
            services.AddScoped<ICommonModelFactory, CommonModelFactory>();
            services.AddScoped<ICountryModelFactory, CountryModelFactory>();
            services.AddScoped<ICurrencyModelFactory, CurrencyModelFactory>();
            services.AddScoped<ICustomerAttributeModelFactory, CustomerAttributeModelFactory>();
            services.AddScoped<ICustomerModelFactory, CustomerModelFactory>();
            services.AddScoped<ICustomerRoleModelFactory, CustomerRoleModelFactory>();
            services.AddScoped<IDiscountModelFactory, DiscountModelFactory>();
            services.AddScoped<IEmailAccountModelFactory, EmailAccountModelFactory>();
            services.AddScoped<IExternalAuthenticationMethodModelFactory, ExternalAuthenticationMethodModelFactory>();
            services.AddScoped<IForumModelFactory, ForumModelFactory>();
            services.AddScoped<IGiftCardModelFactory, GiftCardModelFactory>();
            services.AddScoped<IHomeModelFactory, HomeModelFactory>();
            services.AddScoped<ILanguageModelFactory, LanguageModelFactory>();
            services.AddScoped<ILogModelFactory, LogModelFactory>();
            services.AddScoped<IManufacturerModelFactory, ManufacturerModelFactory>();
            services.AddScoped<IMeasureModelFactory, MeasureModelFactory>();
            services.AddScoped<IMessageTemplateModelFactory, MessageTemplateModelFactory>();
            services.AddScoped<IMultiFactorAuthenticationMethodModelFactory, MultiFactorAuthenticationMethodModelFactory>();
            services.AddScoped<INewsletterSubscriptionModelFactory, NewsletterSubscriptionModelFactory>();
            services.AddScoped<INewsModelFactory, NewsModelFactory>();
            services.AddScoped<IOrderModelFactory, OrderModelFactory>();
            services.AddScoped<IPaymentModelFactory, PaymentModelFactory>();
            services.AddScoped<IPluginModelFactory, PluginModelFactory>();
            services.AddScoped<IPollModelFactory, PollModelFactory>();
            services.AddScoped<IProductModelFactory, ProductModelFactory>();
            services.AddScoped<IProductAttributeModelFactory, ProductAttributeModelFactory>();
            services.AddScoped<IProductReviewModelFactory, ProductReviewModelFactory>();
            services.AddScoped<IReportModelFactory, ReportModelFactory>();
            services.AddScoped<IQueuedEmailModelFactory, QueuedEmailModelFactory>();
            services.AddScoped<IRecurringPaymentModelFactory, RecurringPaymentModelFactory>();
            services.AddScoped<IReturnRequestModelFactory, ReturnRequestModelFactory>();
            services.AddScoped<IReviewTypeModelFactory, ReviewTypeModelFactory>();
            services.AddScoped<IScheduleTaskModelFactory, ScheduleTaskModelFactory>();
            services.AddScoped<ISecurityModelFactory, SecurityModelFactory>();
            services.AddScoped<ISettingModelFactory, SettingModelFactory>();
            services.AddScoped<IShippingModelFactory, ShippingModelFactory>();
            services.AddScoped<IShoppingCartModelFactory, ShoppingCartModelFactory>();
            services.AddScoped<ISpecificationAttributeModelFactory, SpecificationAttributeModelFactory>();
            services.AddScoped<IStoreModelFactory, StoreModelFactory>();
            services.AddScoped<ITaxModelFactory, TaxModelFactory>();
            services.AddScoped<ITemplateModelFactory, TemplateModelFactory>();
            services.AddScoped<ITopicModelFactory, TopicModelFactory>();
            services.AddScoped<IVendorAttributeModelFactory, VendorAttributeModelFactory>();
            services.AddScoped<IVendorModelFactory, VendorModelFactory>();
            services.AddScoped<IWidgetModelFactory, WidgetModelFactory>();
            services.AddScoped<ITireDealModelFactory, TireDealModelFactory>();

            //factories
            services.AddScoped<Factories.IAddressModelFactory, Factories.AddressModelFactory>();
            services.AddScoped<Factories.IBlogModelFactory, Factories.BlogModelFactory>();
            services.AddScoped<Factories.ICatalogModelFactory, Factories.CatalogModelFactory>();
            services.AddScoped<Factories.ICheckoutModelFactory, Factories.CheckoutModelFactory>();
            services.AddScoped<Factories.ICommonModelFactory, Factories.CommonModelFactory>();
            services.AddScoped<Factories.ICountryModelFactory, Factories.CountryModelFactory>();
            services.AddScoped<Factories.ICustomerModelFactory, Factories.CustomerModelFactory>();
            services.AddScoped<Factories.IForumModelFactory, Factories.ForumModelFactory>();
            services.AddScoped<Factories.IExternalAuthenticationModelFactory, Factories.ExternalAuthenticationModelFactory>();
            services.AddScoped<Factories.IJsonLdModelFactory, Factories.JsonLdModelFactory>();
            services.AddScoped<Factories.INewsModelFactory, Factories.NewsModelFactory>();
            services.AddScoped<Factories.INewsletterModelFactory, Factories.NewsletterModelFactory>();
            services.AddScoped<Factories.IOrderModelFactory, Factories.OrderModelFactory>();
            services.AddScoped<Factories.IPollModelFactory, Factories.PollModelFactory>();
            services.AddScoped<Factories.IPrivateMessagesModelFactory, Factories.PrivateMessagesModelFactory>();
            services.AddScoped<Factories.IProductModelFactory, Factories.ProductModelFactory>();
            services.AddScoped<Factories.IProfileModelFactory, Factories.ProfileModelFactory>();
            services.AddScoped<Factories.IReturnRequestModelFactory, Factories.ReturnRequestModelFactory>();
            services.AddScoped<Factories.IShoppingCartModelFactory, Factories.ShoppingCartModelFactory>();
            services.AddScoped<Factories.ISitemapModelFactory, Factories.SitemapModelFactory>();
            services.AddScoped<Factories.ITopicModelFactory, Factories.TopicModelFactory>();
            services.AddScoped<Factories.IVendorModelFactory, Factories.VendorModelFactory>();
            services.AddScoped<Factories.IWidgetModelFactory, Factories.WidgetModelFactory>();

            //helpers classes
            services.AddScoped<ITinyMceHelper, TinyMceHelper>();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 2002;
    }
}
