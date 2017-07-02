using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
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
using Nop.Services.Infrastructure;
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
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using Nop.Services.Tax;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;

namespace Nop.Web.Framework.Infrastructure
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
            //web helper
            services.AddScoped<IWebHelper, WebHelper>();

            //user agent helper
            services.AddScoped<IUserAgentHelper, UserAgentHelper>();

            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();

            //TODO test data services injection
            services.AddTransient<DataSettings>(c => dataSettingsManager.LoadSettings());
            services.AddTransient<BaseDataProviderManager>(x => new EfDataProviderManager(x.GetRequiredService<DataSettings>()));
            services.AddTransient<IDataProvider>(x => x.GetRequiredService<BaseDataProviderManager>().LoadDataProvider());

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                //application is already installed
                var efDataProviderManager = new EfDataProviderManager(dataSettingsManager.LoadSettings());
                var dataProvider = efDataProviderManager.LoadDataProvider();
                dataProvider.InitConnectionFactory();

                services.AddScoped<IDbContext>(c => new NopObjectContext(dataProviderSettings.DataConnectionString));
            }
            else
            {
                //application is not installed yet
                services.AddScoped<IDbContext>(c => new NopObjectContext(dataSettingsManager.LoadSettings().DataConnectionString));
            }

            //repositories
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            //plugins
            services.AddScoped<IPluginFinder, PluginFinder>();
            services.AddScoped<IOfficialFeedManager, OfficialFeedManager>();

            //cache manager
            services.AddScoped<ICacheManager, PerRequestCacheManager>();

            //static cache manager
            if (config.RedisCachingEnabled)
                services.AddScoped<IStaticCacheManager, RedisCacheManager>();
            else
                services.AddScoped<IStaticCacheManager, MemoryCacheManager>();

            //machine name provider
            if (config.RunOnAzureWebApps)
                services.AddSingleton<IMachineNameProvider, AzureWebAppsMachineNameProvider>();
            else
                services.AddSingleton<IMachineNameProvider, DefaultMachineNameProvider>();

            //work context
            services.AddScoped<IWorkContext, WebWorkContext>();

            //store context
            services.AddScoped<IStoreContext, WebStoreContext>();

            //services
            services.AddScoped<IBackInStockSubscriptionService, BackInStockSubscriptionService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICompareProductsService, CompareProductsService>();
            services.AddScoped<IRecentlyViewedProductsService, RecentlyViewedProductsService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IPriceFormatter, PriceFormatter>();
            services.AddScoped<IProductAttributeFormatter, ProductAttributeFormatter>();
            services.AddScoped<IProductAttributeParser, ProductAttributeParser>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICopyProductService, CopyProductService>();
            services.AddScoped<ISpecificationAttributeService, SpecificationAttributeService>();
            services.AddScoped<IProductTemplateService, ProductTemplateService>();
            services.AddScoped<ICategoryTemplateService, CategoryTemplateService>();
            services.AddScoped<IManufacturerTemplateService, ManufacturerTemplateService>();
            services.AddScoped<ITopicTemplateService, TopicTemplateService>();
            services.AddScoped<IProductTagService, ProductTagService>();
            services.AddScoped<IAddressAttributeFormatter, AddressAttributeFormatter>();
            services.AddScoped<IAddressAttributeParser, AddressAttributeParser>();
            services.AddScoped<IAddressAttributeService, AddressAttributeService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAffiliateService, AffiliateService>();
            services.AddScoped<IVendorService, VendorService>();
            services.AddScoped<ISearchTermService, SearchTermService>();
            services.AddScoped<IGenericAttributeService, GenericAttributeService>();
            services.AddScoped<IFulltextService, FulltextService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<ICustomerAttributeFormatter, CustomerAttributeFormatter>();
            services.AddScoped<ICustomerAttributeParser, CustomerAttributeParser>();
            services.AddScoped<ICustomerAttributeService, CustomerAttributeService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerRegistrationService, CustomerRegistrationService>();
            services.AddScoped<ICustomerReportService, CustomerReportService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IAclService, AclService>();
            services.AddScoped<IPriceCalculationService, PriceCalculationService>();
            services.AddScoped<IGeoLookupService, GeoLookupService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IMeasureService, MeasureService>();
            services.AddScoped<IStateProvinceService, StateProvinceService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IStoreMappingService, StoreMappingService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ILocalizedEntityService, LocalizedEntityService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IDownloadService, DownloadService>();
            services.AddScoped<IMessageTemplateService, MessageTemplateService>();
            services.AddScoped<IQueuedEmailService, QueuedEmailService>();
            services.AddScoped<INewsLetterSubscriptionService, NewsLetterSubscriptionService>();
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<IEmailAccountService, EmailAccountService>();
            services.AddScoped<IWorkflowMessageService, WorkflowMessageService>();
            services.AddScoped<IMessageTokenProvider, MessageTokenProvider>();
            services.AddScoped<ITokenizer, Tokenizer>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ICheckoutAttributeFormatter, CheckoutAttributeFormatter>();
            services.AddScoped<ICheckoutAttributeParser, CheckoutAttributeParser>();
            services.AddScoped<ICheckoutAttributeService, CheckoutAttributeService>();
            services.AddScoped<IGiftCardService, GiftCardService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderReportService, OrderReportService>();
            services.AddScoped<IOrderProcessingService, OrderProcessingService>();
            services.AddScoped<IOrderTotalCalculationService, OrderTotalCalculationService>();
            services.AddScoped<IReturnRequestService, ReturnRequestService>();
            services.AddScoped<IRewardPointService, RewardPointService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<ICustomNumberFormatter, CustomNumberFormatter>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IAuthenticationService, CookieAuthenticationService>();
            services.AddScoped<IUrlRecordService, UrlRecordService>();
            services.AddScoped<IShipmentService, ShipmentService>();
            services.AddScoped<IShippingService, ShippingService>();
            services.AddScoped<IDateRangeService, DateRangeService>();
            services.AddScoped<ITaxCategoryService, TaxCategoryService>();
            services.AddScoped<ITaxService, TaxService>();
            services.AddScoped<ILogger, DefaultLogger>();
            services.AddScoped<ICustomerActivityService, CustomerActivityService>();
            services.AddScoped<IForumService, ForumService>();
            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IWidgetService, WidgetService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IDateTimeHelper, DateTimeHelper>();
            services.AddScoped<ISitemapGenerator, SitemapGenerator>();
            services.AddScoped<IPageHeadBuilder, PageHeadBuilder>();
            services.AddScoped<IScheduleTaskService, ScheduleTaskService>();
            services.AddScoped<IExportManager, ExportManager>();
            services.AddScoped<IImportManager, ImportManager>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<IThemeProvider, ThemeProvider>();
            services.AddScoped<IThemeContext, ThemeContext>();
            services.AddScoped<IExternalAuthorizer, ExternalAuthorizer>();
            services.AddScoped<IOpenAuthenticationService, OpenAuthenticationService>();
            services.AddScoped<IRoutePublisher, RoutePublisher>();
            services.AddScoped<IEventPublisher, EventPublisher>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IActionContextAccessor, ActionContextAccessor>();

            //register settings
            var settings = typeFinder.FindClassesOfType<ISettings>();
            foreach (var setting in settings)
            {
                services.AddScoped(setting, c =>
                {
                    var currentStoreId = c.GetRequiredService<IStoreContext>().CurrentStore.Id;
                    //uncomment the code below if you want load settings per store only when you have two stores installed.
                    //var currentStoreId = c.Resolve<IStoreService>().GetAllStores().Count > 1
                    //    c.Resolve<IStoreContext>().CurrentStore.Id : 0;

                    //although it's better to connect to your database and execute the following SQL:
                    //DELETE FROM [Setting] WHERE [StoreId] > 0
                    return c.GetRequiredService<ISettingService>().LoadSetting(setting, currentStoreId);
                });
            }

            //picture service
            if (!string.IsNullOrEmpty(config.AzureBlobStorageConnectionString))
                services.AddScoped<IPictureService, AzurePictureService>();
            else
                services.AddScoped<IPictureService, PictureService>();

            //installation service
            if (!DataSettingsHelper.DatabaseIsInstalled())
            {
                if (config.UseFastInstallationService)
                    services.AddScoped<IInstallationService, SqlFileInstallationService>();
                else
                    services.AddScoped<IInstallationService, CodeFirstInstallationService>();
            }

            //event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
            {
                services.AddScoped(consumer, c =>
                {
                    return consumer.FindInterfaces((type, criteria) =>
                    {
                        var isMatch = type.IsGenericType && ((Type) criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof(IConsumer<>));
                });
            }
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 0; }
        }
    }
}
