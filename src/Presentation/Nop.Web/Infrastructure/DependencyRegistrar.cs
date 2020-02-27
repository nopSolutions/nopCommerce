using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Factories;
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

            //common factories
            builder.RegisterType<AclSupportedModelFactory>().As<IAclSupportedModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<DiscountSupportedModelFactory>().As<IDiscountSupportedModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<LocalizedModelFactory>().As<ILocalizedModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<StoreMappingSupportedModelFactory>().As<IStoreMappingSupportedModelFactory>().InstancePerLifetimeScope();

            //admin factories
            builder.RegisterType<BaseAdminModelFactory>().As<IBaseAdminModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ActivityLogModelFactory>().As<IActivityLogModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<AddressAttributeModelFactory>().As<IAddressAttributeModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<AffiliateModelFactory>().As<IAffiliateModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<BlogModelFactory>().As<IBlogModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CampaignModelFactory>().As<ICampaignModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryModelFactory>().As<ICategoryModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutAttributeModelFactory>().As<ICheckoutAttributeModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CommonModelFactory>().As<ICommonModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CountryModelFactory>().As<ICountryModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CurrencyModelFactory>().As<ICurrencyModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerAttributeModelFactory>().As<ICustomerAttributeModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerModelFactory>().As<ICustomerModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerRoleModelFactory>().As<ICustomerRoleModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<DiscountModelFactory>().As<IDiscountModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<EmailAccountModelFactory>().As<IEmailAccountModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ExternalAuthenticationMethodModelFactory>().As<IExternalAuthenticationMethodModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ForumModelFactory>().As<IForumModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<GiftCardModelFactory>().As<IGiftCardModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<HomeModelFactory>().As<IHomeModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<LanguageModelFactory>().As<ILanguageModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<LogModelFactory>().As<ILogModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ManufacturerModelFactory>().As<IManufacturerModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<MeasureModelFactory>().As<IMeasureModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<MessageTemplateModelFactory>().As<IMessageTemplateModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<NewsletterSubscriptionModelFactory>().As<INewsletterSubscriptionModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<NewsModelFactory>().As<INewsModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<OrderModelFactory>().As<IOrderModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentModelFactory>().As<IPaymentModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<PluginModelFactory>().As<IPluginModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<PollModelFactory>().As<IPollModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ProductModelFactory>().As<IProductModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAttributeModelFactory>().As<IProductAttributeModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ProductReviewModelFactory>().As<IProductReviewModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ReportModelFactory>().As<IReportModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<QueuedEmailModelFactory>().As<IQueuedEmailModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<RecurringPaymentModelFactory>().As<IRecurringPaymentModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ReturnRequestModelFactory>().As<IReturnRequestModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ReviewTypeModelFactory>().As<IReviewTypeModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ScheduleTaskModelFactory>().As<IScheduleTaskModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SecurityModelFactory>().As<ISecurityModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SettingModelFactory>().As<ISettingModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ShippingModelFactory>().As<IShippingModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartModelFactory>().As<IShoppingCartModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SpecificationAttributeModelFactory>().As<ISpecificationAttributeModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<StoreModelFactory>().As<IStoreModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<TaxModelFactory>().As<ITaxModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<TemplateModelFactory>().As<ITemplateModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<TopicModelFactory>().As<ITopicModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<VendorAttributeModelFactory>().As<IVendorAttributeModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<VendorModelFactory>().As<IVendorModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<WidgetModelFactory>().As<IWidgetModelFactory>().InstancePerLifetimeScope();

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
        public int Order => 2;
    }
}
