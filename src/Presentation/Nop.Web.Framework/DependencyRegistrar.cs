using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Fakes;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Plugins;
using Nop.Data;
using Nop.Services.Affiliates;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.ExportImport;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Installation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Polls;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using Nop.Services.Tax;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;

namespace Nop.Web.Framework
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context and other related stuff
            builder.Register(c => 
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerRequest();

            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerRequest();
            //user agent helper
            builder.RegisterType<UserAgentHelper>().As<IUserAgentHelper>().InstancePerRequest();

            
            //controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();
            builder.Register(c => dataSettingsManager.LoadSettings()).As<DataSettings>();
            builder.Register(x => new EfDataProviderManager(x.Resolve<DataSettings>())).As<BaseDataProviderManager>().InstancePerDependency();


            builder.Register(x => x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IDataProvider>().InstancePerDependency();

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                var efDataProviderManager = new EfDataProviderManager(dataSettingsManager.LoadSettings());
                var dataProvider = efDataProviderManager.LoadDataProvider();
                dataProvider.InitConnectionFactory();

                builder.Register<IDbContext>(c => new NopObjectContext(dataProviderSettings.DataConnectionString)).InstancePerRequest();
            }
            else
            {
                builder.Register<IDbContext>(c => new NopObjectContext(dataSettingsManager.LoadSettings().DataConnectionString)).InstancePerRequest();
            }


            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerRequest();
            
            //plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerRequest();

            //cache manager
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_static").SingleInstance();
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_per_request").InstancePerRequest();


            //work context
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerRequest();
            //store context
            builder.RegisterType<WebStoreContext>().As<IStoreContext>().InstancePerRequest();

            //services
            builder.RegisterType<BackInStockSubscriptionService>().As<IBackInStockSubscriptionService>().InstancePerRequest();
            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerRequest();
            builder.RegisterType<CompareProductsService>().As<ICompareProductsService>().InstancePerRequest();
            builder.RegisterType<RecentlyViewedProductsService>().As<IRecentlyViewedProductsService>().InstancePerRequest();
            builder.RegisterType<ManufacturerService>().As<IManufacturerService>().InstancePerRequest();
            builder.RegisterType<PriceFormatter>().As<IPriceFormatter>().InstancePerRequest();
            builder.RegisterType<ProductAttributeFormatter>().As<IProductAttributeFormatter>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAttributeParser>().As<IProductAttributeParser>().InstancePerRequest();
            builder.RegisterType<ProductAttributeService>().As<IProductAttributeService>().InstancePerRequest();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerRequest();
            builder.RegisterType<CopyProductService>().As<ICopyProductService>().InstancePerRequest();
            builder.RegisterType<SpecificationAttributeService>().As<ISpecificationAttributeService>().InstancePerRequest();
            builder.RegisterType<ProductTemplateService>().As<IProductTemplateService>().InstancePerRequest();
            builder.RegisterType<CategoryTemplateService>().As<ICategoryTemplateService>().InstancePerRequest();
            builder.RegisterType<ManufacturerTemplateService>().As<IManufacturerTemplateService>().InstancePerRequest();
            //pass MemoryCacheManager as cacheManager (cache settings between requests)
            builder.RegisterType<ProductTagService>().As<IProductTagService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerRequest();

            builder.RegisterType<AffiliateService>().As<IAffiliateService>().InstancePerRequest();
            builder.RegisterType<VendorService>().As<IVendorService>().InstancePerRequest();
            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerRequest();
            builder.RegisterType<SearchTermService>().As<ISearchTermService>().InstancePerRequest();
            builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().InstancePerRequest();
            builder.RegisterType<FulltextService>().As<IFulltextService>().InstancePerRequest();
            builder.RegisterType<MaintenanceService>().As<IMaintenanceService>().InstancePerRequest();


            builder.RegisterType<CustomerAttributeParser>().As<ICustomerAttributeParser>().InstancePerRequest();
            builder.RegisterType<CustomerAttributeService>().As<ICustomerAttributeService>().InstancePerRequest();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerRequest();
            builder.RegisterType<CustomerRegistrationService>().As<ICustomerRegistrationService>().InstancePerRequest();
            builder.RegisterType<CustomerReportService>().As<ICustomerReportService>().InstancePerRequest();

            //pass MemoryCacheManager as cacheManager (cache settings between requests)
            builder.RegisterType<PermissionService>().As<IPermissionService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerRequest();
            //pass MemoryCacheManager as cacheManager (cache settings between requests)
            builder.RegisterType<AclService>().As<IAclService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerRequest();
            //pass MemoryCacheManager as cacheManager (cache settings between requests)
            builder.RegisterType<PriceCalculationService>().As<IPriceCalculationService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerRequest();

            builder.RegisterType<GeoLookupService>().As<IGeoLookupService>().InstancePerRequest();
            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerRequest();
            builder.RegisterType<CurrencyService>().As<ICurrencyService>().InstancePerRequest();
            builder.RegisterType<MeasureService>().As<IMeasureService>().InstancePerRequest();
            builder.RegisterType<StateProvinceService>().As<IStateProvinceService>().InstancePerRequest();

            builder.RegisterType<StoreService>().As<IStoreService>().InstancePerRequest();
            //pass MemoryCacheManager as cacheManager (cache settings between requests)
            builder.RegisterType<StoreMappingService>().As<IStoreMappingService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerRequest();

            builder.RegisterType<DiscountService>().As<IDiscountService>().InstancePerRequest();


            //pass MemoryCacheManager as cacheManager (cache settings between requests)
            builder.RegisterType<SettingService>().As<ISettingService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerRequest();
            builder.RegisterSource(new SettingsSource());

            //pass MemoryCacheManager as cacheManager (cache locales between requests)
            builder.RegisterType<LocalizationService>().As<ILocalizationService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerRequest();

            //pass MemoryCacheManager as cacheManager (cache locales between requests)
            builder.RegisterType<LocalizedEntityService>().As<ILocalizedEntityService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerRequest();
            builder.RegisterType<LanguageService>().As<ILanguageService>().InstancePerRequest();

            builder.RegisterType<DownloadService>().As<IDownloadService>().InstancePerRequest();
            builder.RegisterType<PictureService>().As<IPictureService>().InstancePerRequest();

            builder.RegisterType<MessageTemplateService>().As<IMessageTemplateService>().InstancePerRequest();
            builder.RegisterType<QueuedEmailService>().As<IQueuedEmailService>().InstancePerRequest();
            builder.RegisterType<NewsLetterSubscriptionService>().As<INewsLetterSubscriptionService>().InstancePerRequest();
            builder.RegisterType<CampaignService>().As<ICampaignService>().InstancePerRequest();
            builder.RegisterType<EmailAccountService>().As<IEmailAccountService>().InstancePerRequest();
            builder.RegisterType<WorkflowMessageService>().As<IWorkflowMessageService>().InstancePerRequest();
            builder.RegisterType<MessageTokenProvider>().As<IMessageTokenProvider>().InstancePerRequest();
            builder.RegisterType<Tokenizer>().As<ITokenizer>().InstancePerRequest();
            builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerRequest();

            builder.RegisterType<CheckoutAttributeFormatter>().As<ICheckoutAttributeFormatter>().InstancePerRequest();
            builder.RegisterType<CheckoutAttributeParser>().As<ICheckoutAttributeParser>().InstancePerRequest();
            builder.RegisterType<CheckoutAttributeService>().As<ICheckoutAttributeService>().InstancePerRequest();
            builder.RegisterType<GiftCardService>().As<IGiftCardService>().InstancePerRequest();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerRequest();
            builder.RegisterType<OrderReportService>().As<IOrderReportService>().InstancePerRequest();
            builder.RegisterType<OrderProcessingService>().As<IOrderProcessingService>().InstancePerRequest();
            builder.RegisterType<OrderTotalCalculationService>().As<IOrderTotalCalculationService>().InstancePerRequest();
            builder.RegisterType<ShoppingCartService>().As<IShoppingCartService>().InstancePerRequest();

            builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerRequest();

            builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerRequest();
            builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().InstancePerRequest();


            //pass MemoryCacheManager as cacheManager (cache settings between requests)
            builder.RegisterType<UrlRecordService>().As<IUrlRecordService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerRequest();

            builder.RegisterType<ShipmentService>().As<IShipmentService>().InstancePerRequest();
            builder.RegisterType<ShippingService>().As<IShippingService>().InstancePerRequest();

            builder.RegisterType<TaxCategoryService>().As<ITaxCategoryService>().InstancePerRequest();
            builder.RegisterType<TaxService>().As<ITaxService>().InstancePerRequest();
            builder.RegisterType<TaxCategoryService>().As<ITaxCategoryService>().InstancePerRequest();

            builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerRequest();

            //pass MemoryCacheManager as cacheManager (cache settings between requests)
            builder.RegisterType<CustomerActivityService>().As<ICustomerActivityService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerRequest();

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["UseFastInstallationService"]) &&
                Convert.ToBoolean(ConfigurationManager.AppSettings["UseFastInstallationService"]))
            {
                builder.RegisterType<SqlFileInstallationService>().As<IInstallationService>().InstancePerRequest();
            }
            else
            {
                builder.RegisterType<CodeFirstInstallationService>().As<IInstallationService>().InstancePerRequest();
            }

            builder.RegisterType<ForumService>().As<IForumService>().InstancePerRequest();

            builder.RegisterType<PollService>().As<IPollService>().InstancePerRequest();
            builder.RegisterType<BlogService>().As<IBlogService>().InstancePerRequest();
            builder.RegisterType<WidgetService>().As<IWidgetService>().InstancePerRequest();
            builder.RegisterType<TopicService>().As<ITopicService>().InstancePerRequest();
            builder.RegisterType<NewsService>().As<INewsService>().InstancePerRequest();

            builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerRequest();
            builder.RegisterType<SitemapGenerator>().As<ISitemapGenerator>().InstancePerRequest();
            builder.RegisterType<PageHeadBuilder>().As<IPageHeadBuilder>().InstancePerRequest();

            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerRequest();

            builder.RegisterType<ExportManager>().As<IExportManager>().InstancePerRequest();
            builder.RegisterType<ImportManager>().As<IImportManager>().InstancePerRequest();
            builder.RegisterType<PdfService>().As<IPdfService>().InstancePerRequest();
            builder.RegisterType<ThemeProvider>().As<IThemeProvider>().InstancePerRequest();
            builder.RegisterType<ThemeContext>().As<IThemeContext>().InstancePerRequest();


            builder.RegisterType<ExternalAuthorizer>().As<IExternalAuthorizer>().InstancePerRequest();
            builder.RegisterType<OpenAuthenticationService>().As<IOpenAuthenticationService>().InstancePerRequest();
           
                
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();

            //Register event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
            {
                builder.RegisterType(consumer)
                    .As(consumer.FindInterfaces((type, criteria) =>
                    {
                        var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof(IConsumer<>)))
                    .InstancePerRequest();
            }
            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
            builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().SingleInstance();

        }

        public int Order
        {
            get { return 0; }
        }
    }


    public class SettingsSource : IRegistrationSource
    {
        static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
            "BuildRegistration",
            BindingFlags.Static | BindingFlags.NonPublic);

        public IEnumerable<IComponentRegistration> RegistrationsFor(
                Service service,
                Func<Service, IEnumerable<IComponentRegistration>> registrations)
        {
            var ts = service as TypedService;
            if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
            {
                var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration)buildMethod.Invoke(null, null);
            }
        }

        static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
        {
            return RegistrationBuilder
                .ForDelegate((c, p) =>
                {
                    var currentStoreId = c.Resolve<IStoreContext>().CurrentStore.Id;
                    //uncomment the code below if you want load settings per store only when you have two stores installed.
                    //var currentStoreId = c.Resolve<IStoreService>().GetAllStores().Count > 1
                    //    c.Resolve<IStoreContext>().CurrentStore.Id : 0;

                    //although it's better to connect to your database and execute the following SQL:
                    //DELETE FROM [Setting] WHERE [StoreId] > 0
                    return c.Resolve<ISettingService>().LoadSetting<TSettings>(currentStoreId);
                })
                .InstancePerRequest()
                .CreateRegistration();
        }

        public bool IsAdapterForIndividualComponents { get { return false; } }
    }

}
