using System;
using System.IO;
using System.Linq;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Affiliates;
using Nop.Services.Authentication.External;
using Nop.Services.Blogs;
using Nop.Services.Caching;
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
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Installation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Polls;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using Nop.Services.Tax;
using Nop.Services.Themes;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Infrastructure.Installation;
using IAuthenticationService = Nop.Services.Authentication.IAuthenticationService;

namespace Nop.Tests
{
    public abstract class BaseNopTest
    {
        private static readonly ServiceProvider _serviceProvider;
       
        static BaseNopTest()
        {
            var services = new ServiceCollection();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var typeFinder = new AppDomainTypeFinder();

            Singleton<DataSettings>.Instance = new DataSettings
            {
                ConnectionString = "Data Source=nopCommerceTest.sqlite;Mode=Memory;Cache=Shared"
            };
            
            var mAssemblies = typeFinder.FindClassesOfType<AutoReversingMigration>()
                .Select(t => t.Assembly)
                .Distinct()
                .ToArray();

            //add NopConfig configuration parameters
            services.AddSingleton(new NopConfig());

            //add hosting configuration parameters
            services.AddSingleton(new HostingConfig());

            var hostApplicationLifetime = new Mock<IHostApplicationLifetime>();
            services.AddSingleton(hostApplicationLifetime.Object);

            var rootPath = new DirectoryInfo($@"{Directory.GetCurrentDirectory()}\..\..\..\..\..\Presentation\Nop.Web").FullName;
            
            //Presentation\Nop.Web\wwwroot
            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            webHostEnvironment.Setup(p => p.WebRootPath).Returns(Path.Combine(rootPath, "wwwroot"));
            webHostEnvironment.Setup(p => p.ContentRootPath).Returns(rootPath);
            webHostEnvironment.Setup(p => p.EnvironmentName).Returns("test");
            webHostEnvironment.Setup(p => p.ApplicationName).Returns("nopCommerce");
            services.AddSingleton(webHostEnvironment.Object);

            var httpContext = new DefaultHttpContext { Request = { Headers = { { HeaderNames.Host, NopTestsDefaults.HostIpAddress } } } };

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(p => p.HttpContext).Returns(httpContext);

            services.AddSingleton(httpContextAccessor.Object);

            var actionContextAccessor = new Mock<IActionContextAccessor>();
            actionContextAccessor.Setup(x => x.ActionContext).Returns(new ActionContext(httpContext, new RouteData(), new ActionDescriptor()));

            services.AddSingleton(actionContextAccessor.Object);

            var urlHelperFactory = new Mock<IUrlHelperFactory>();

            urlHelperFactory.Setup(x => x.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(new UrlHelper(actionContextAccessor.Object.ActionContext));

            services.AddSingleton(urlHelperFactory.Object);

            var tempDataDictionaryFactory = new Mock<ITempDataDictionaryFactory>();
            var dataDictionary = new TempDataDictionary(httpContextAccessor.Object.HttpContext, new Mock<ITempDataProvider>().Object);
            tempDataDictionaryFactory.Setup(f => f.GetTempData(It.IsAny<HttpContext>())).Returns(dataDictionary);
            services.AddSingleton(tempDataDictionaryFactory.Object);

            services.AddSingleton<ITypeFinder>(typeFinder);

            //file provider
            services.AddTransient<INopFileProvider, NopFileProvider>();

            //web helper
            services.AddTransient<IWebHelper, WebHelper>();

            //user agent helper
            services.AddTransient<IUserAgentHelper, UserAgentHelper>();

            //data layer
            services.AddTransient<IDataProviderManager, TestDataProviderManager>();
            services.AddSingleton<INopDataProvider, SqLiteNopDataProvider>();

            //repositories
            services.AddTransient(typeof(IRepository<>), typeof(EntityRepository<>));

            //plugins
            services.AddTransient<IPluginService, PluginService>();

            services.AddSingleton<IMemoryCache>(memoryCache);
            services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();
            services.AddSingleton<ILocker, MemoryCacheManager>();

            //services
            services.AddTransient<IBackInStockSubscriptionService, BackInStockSubscriptionService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<ICompareProductsService, CompareProductsService>();
            services.AddTransient<IRecentlyViewedProductsService, RecentlyViewedProductsService>();
            services.AddTransient<IManufacturerService, ManufacturerService>();
            services.AddTransient<IPriceFormatter, PriceFormatter>();
            services.AddTransient<IProductAttributeFormatter, ProductAttributeFormatter>();
            services.AddTransient<IProductAttributeParser, ProductAttributeParser>();
            services.AddTransient<IProductAttributeService, ProductAttributeService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICopyProductService, CopyProductService>();
            services.AddTransient<ISpecificationAttributeService, SpecificationAttributeService>();
            services.AddTransient<IProductTemplateService, ProductTemplateService>();
            services.AddTransient<ICategoryTemplateService, CategoryTemplateService>();
            services.AddTransient<IManufacturerTemplateService, ManufacturerTemplateService>();
            services.AddTransient<ITopicTemplateService, TopicTemplateService>();
            services.AddTransient<IProductTagService, ProductTagService>();
            services.AddTransient<IAddressAttributeFormatter, AddressAttributeFormatter>();
            services.AddTransient<IAddressAttributeParser, AddressAttributeParser>();
            services.AddTransient<IAddressAttributeService, AddressAttributeService>();
            services.AddTransient<IAddressService, AddressService>();
            services.AddTransient<IAffiliateService, AffiliateService>();
            services.AddTransient<IVendorService, VendorService>();
            services.AddTransient<IVendorAttributeFormatter, VendorAttributeFormatter>();
            services.AddTransient<IVendorAttributeParser, VendorAttributeParser>();
            services.AddTransient<IVendorAttributeService, VendorAttributeService>();
            services.AddTransient<ISearchTermService, SearchTermService>();
            services.AddTransient<IGenericAttributeService, GenericAttributeService>();
            services.AddTransient<IFulltextService, FulltextService>();
            services.AddTransient<IMaintenanceService, MaintenanceService>();
            services.AddTransient<ICustomerAttributeFormatter, CustomerAttributeFormatter>();
            services.AddTransient<ICustomerAttributeParser, CustomerAttributeParser>();
            services.AddTransient<ICustomerAttributeService, CustomerAttributeService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<ICustomerRegistrationService, CustomerRegistrationService>();
            services.AddTransient<ICustomerReportService, CustomerReportService>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IAclService, AclService>();
            services.AddTransient<IPriceCalculationService, PriceCalculationService>();
            services.AddTransient<IGeoLookupService, GeoLookupService>();
            services.AddTransient<ICountryService, CountryService>();
            services.AddTransient<ICurrencyService, CurrencyService>();
            services.AddTransient<IMeasureService, MeasureService>();
            services.AddTransient<IStateProvinceService, StateProvinceService>();
            services.AddTransient<IStoreService, StoreService>();
            services.AddTransient<IStoreMappingService, StoreMappingService>();
            services.AddTransient<IDiscountService, DiscountService>();
            services.AddTransient<ILocalizationService, LocalizationService>();
            services.AddTransient<ILocalizedEntityService, LocalizedEntityService>();
            services.AddTransient<IInstallationLocalizationService, InstallationLocalizationService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<IDownloadService, DownloadService>();
            services.AddTransient<IMessageTemplateService, MessageTemplateService>();
            services.AddTransient<IQueuedEmailService, QueuedEmailService>();
            services.AddTransient<INewsLetterSubscriptionService, NewsLetterSubscriptionService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<ICampaignService, CampaignService>();
            services.AddTransient<IEmailAccountService, EmailAccountService>();
            services.AddTransient<IWorkflowMessageService, WorkflowMessageService>();
            services.AddTransient<IMessageTokenProvider, MessageTokenProvider>();
            services.AddTransient<ITokenizer, Tokenizer>();
            services.AddTransient<ISmtpBuilder, SmtpBuilder>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ICheckoutAttributeFormatter, CheckoutAttributeFormatter>();
            services.AddTransient<ICheckoutAttributeParser, CheckoutAttributeParser>();
            services.AddTransient<ICheckoutAttributeService, CheckoutAttributeService>();
            services.AddTransient<IGiftCardService, GiftCardService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IOrderReportService, OrderReportService>();
            services.AddTransient<IOrderProcessingService, OrderProcessingService>();
            services.AddTransient<IOrderTotalCalculationService, OrderTotalCalculationService>();
            services.AddTransient<IReturnRequestService, ReturnRequestService>();
            services.AddTransient<IRewardPointService, RewardPointService>();
            services.AddTransient<IShoppingCartService, ShoppingCartService>();
            services.AddTransient<ICustomNumberFormatter, CustomNumberFormatter>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IEncryptionService, EncryptionService>();
            services.AddTransient<IAuthenticationService, TestAuthenticationService>();
            services.AddTransient<IUrlRecordService, UrlRecordService>();
            services.AddTransient<IShipmentService, ShipmentService>();
            services.AddTransient<IShippingService, ShippingService>();
            services.AddTransient<IDateRangeService, DateRangeService>();
            services.AddTransient<ITaxCategoryService, TaxCategoryService>();
            services.AddTransient<ITaxService, TaxService>();
            services.AddTransient<ILogger, DefaultLogger>();
            services.AddTransient<ICustomerActivityService, CustomerActivityService>();
            services.AddTransient<IForumService, ForumService>();
            services.AddTransient<IGdprService, GdprService>();
            services.AddTransient<IPollService, PollService>();
            services.AddTransient<IBlogService, BlogService>();
            services.AddTransient<ITopicService, TopicService>();
            services.AddTransient<INewsService, NewsService>();
            services.AddTransient<IDateTimeHelper, DateTimeHelper>();
            services.AddTransient<ISitemapGenerator, SitemapGenerator>();
            services.AddTransient<IScheduleTaskService, ScheduleTaskService>();
            services.AddTransient<IExportManager, ExportManager>();
            services.AddTransient<IImportManager, ImportManager>();
            services.AddTransient<IPdfService, PdfService>();
            services.AddTransient<IUploadService, UploadService>();
            services.AddTransient<IThemeProvider, ThemeProvider>();
            services.AddTransient<IExternalAuthenticationService, ExternalAuthenticationService>();
            services.AddTransient<ICacheKeyService, CacheKeyService>();

            //slug route transformer
            services.AddSingleton<IReviewTypeService, ReviewTypeService>();
            services.AddSingleton<IEventPublisher, EventPublisher>();
            services.AddTransient<ISettingService, SettingService>();

            //plugin managers
            services.AddTransient(typeof(IPluginManager<>), typeof(PluginManager<>));
            services.AddTransient<IAuthenticationPluginManager, AuthenticationPluginManager>();
            services.AddTransient<IWidgetPluginManager, WidgetPluginManager>();
            services.AddTransient<IExchangeRatePluginManager, ExchangeRatePluginManager>();
            services.AddTransient<IDiscountPluginManager, DiscountPluginManager>();
            services.AddTransient<IPaymentPluginManager, PaymentPluginManager>();
            services.AddTransient<IPickupPluginManager, PickupPluginManager>();
            services.AddTransient<IShippingPluginManager, ShippingPluginManager>();
            services.AddTransient<ITaxPluginManager, TaxPluginManager>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IPictureService, PictureService>();

            //register all settings
            var settings = typeFinder.FindClassesOfType(typeof(ISettings), false).ToList();
            foreach (var setting in settings)
                services.AddTransient(setting, context => context.GetRequiredService<ISettingService>().LoadSetting(setting));

            //event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
            foreach (var findInterface in consumer.FindInterfaces((type, criteria) =>
            {
                var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                return isMatch;
            }, typeof(IConsumer<>)))
                services.AddTransient(findInterface, consumer);

            services.AddSingleton<IInstallationService, CodeFirstInstallationService>();

            services
                // add common FluentMigrator services
                .AddFluentMigratorCore()
                .AddScoped<IProcessorAccessor, TestProcessorAccessor>()
                // set accessor for the connection string
                .AddScoped<IConnectionStringAccessor>(x => DataSettingsManager.LoadSettings())
                .AddScoped<IMigrationManager, MigrationManager>()
                .AddSingleton<IConventionSet, NopTestConventionSet>()
                .ConfigureRunner(rb =>
                    rb.WithVersionTable(new MigrationVersionInfo()).AddSQLite()
                        // define the assembly containing the migrations
                        .ScanIn(mAssemblies).For.Migrations());

            services.AddTransient<IStoreContext, WebStoreContext>();
            services.AddTransient<IWorkContext, WebWorkContext>();

            _serviceProvider = services.BuildServiceProvider();

            EngineContext.Replace(new NopTestEngine(_serviceProvider));

            _serviceProvider.GetService<INopDataProvider>().CreateDatabase(null);
            _serviceProvider.GetService<INopDataProvider>().InitializeDatabase();
            
            _serviceProvider.GetService<IInstallationService>().InstallRequiredData(NopTestsDefaults.AdminEmail, NopTestsDefaults.AdminPassword);
            _serviceProvider.GetService<IInstallationService>().InstallSampleData(NopTestsDefaults.AdminEmail);
        }

        public T GetService<T>()
        {
            try
            {
                return _serviceProvider.GetRequiredService<T>();
            }
            catch (InvalidOperationException)
            {
                return (T)EngineContext.Current.ResolveUnregistered(typeof(T));
            }
        }

        #region Nested classes
        
        protected class NopTestConventionSet : NopConventionSet
        {
            public NopTestConventionSet(INopDataProvider dataProvider) : base(dataProvider)
            {
            }
        }

        public partial class NopTestEngine : NopEngine
        {
            protected readonly IServiceProvider _internalServiceProvider;

            public NopTestEngine(IServiceProvider serviceProvider)
            {
                _internalServiceProvider = serviceProvider;
            }

            public override IServiceProvider ServiceProvider => _internalServiceProvider;
        }

        public class TestAuthenticationService : IAuthenticationService
        {
            public void SignIn(Customer customer, bool isPersistent)
            {
            }

            public void SignOut()
            {
            }

            public Customer GetAuthenticatedCustomer()
            {
                return _serviceProvider.GetService<ICustomerService>().GetCustomerByEmail(NopTestsDefaults.AdminEmail);
            }
        }

        #endregion
    }
}