using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office.CustomXsn;
using FluentAssertions;
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
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.ComponentModel;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Configuration;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Services.Affiliates;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
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
using Nop.Services.Html;
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
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Themes;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Tests.Nop.Services.Tests.ScheduleTasks;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;
using Nop.Web.Infrastructure.Installation;
using SkiaSharp;
using IAuthenticationService = Nop.Services.Authentication.IAuthenticationService;
using Task = System.Threading.Tasks.Task;

namespace Nop.Tests
{
    public partial class BaseNopTest
    {
        private static readonly ServiceProvider _serviceProvider;
        private static readonly ResourceManager _resourceManager;

        protected BaseNopTest()
        {
            SetDataProviderType(DataProviderType.Unknown);
        }

        private static void Init()
        {
            
            var dataProvider = _serviceProvider.GetService<IDataProviderManager>().DataProvider;
            
            dataProvider.CreateDatabase(null);
            dataProvider.InitializeDatabase();

            var languagePackInfo = (DownloadUrl: string.Empty, Progress: 0);
            
            _serviceProvider.GetService<IInstallationService>()
                .InstallRequiredDataAsync(NopTestsDefaults.AdminEmail, NopTestsDefaults.AdminPassword, languagePackInfo, null, null).Wait();
            _serviceProvider.GetService<IInstallationService>().InstallSampleDataAsync(NopTestsDefaults.AdminEmail).Wait();

            var provider = (IPermissionProvider)Activator.CreateInstance(typeof(StandardPermissionProvider));
            EngineContext.Current.Resolve<IPermissionService>().InstallPermissionsAsync(provider).Wait();
        }
        
        protected static T PropertiesShouldEqual<T, Tm>(T entity, Tm model, params string[] filter) where T : BaseEntity
        where Tm : BaseNopModel
        {
            var objectProperties = typeof(T).GetProperties();
            var modelProperties = typeof(Tm).GetProperties();

            foreach (var objectProperty in objectProperties)
            {
                var name = objectProperty.Name;

                if (filter.Contains(name))
                    continue;

                var modelProperty = Array.Find(modelProperties, p => p.Name == name);

                if (modelProperty == null)
                    continue;

                var objectPropertyValue = objectProperty.GetValue(entity);
                var modelPropertyValue = modelProperty.GetValue(model);

                objectPropertyValue.Should().Be(modelPropertyValue, $"The property \"{typeof(T).Name}.{objectProperty.Name}\" of these objects is not equal");
            }

            return entity;
        }

        static BaseNopTest()
        {
            _resourceManager = Connections.ResourceManager;
            SetDataProviderType(DataProviderType.Unknown);

            TypeDescriptor.AddAttributes(typeof(List<int>),
                new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
            TypeDescriptor.AddAttributes(typeof(List<string>),
                new TypeConverterAttribute(typeof(GenericListTypeConverter<string>)));

            var services = new ServiceCollection();

            services.AddHttpClient();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var typeFinder = new AppDomainTypeFinder();
            Singleton<ITypeFinder>.Instance = typeFinder;

            var mAssemblies = typeFinder.FindClassesOfType<AutoReversingMigration>()
                .Select(t => t.Assembly)
                .Distinct()
                .ToArray();

            //create app settings
            var configurations = typeFinder
                .FindClassesOfType<IConfig>()
                .Select(configType => (IConfig)Activator.CreateInstance(configType))
                .ToList();
            
            var appSettings = new AppSettings(configurations);
            appSettings.Update(new List<IConfig> { Singleton<DataConfig>.Instance });
            Singleton<AppSettings>.Instance = appSettings;
            services.AddSingleton(appSettings);

            var hostApplicationLifetime = new Mock<IHostApplicationLifetime>();
            services.AddSingleton(hostApplicationLifetime.Object);

            var rootPath =
                new DirectoryInfo(
                        $"{Directory.GetCurrentDirectory().Split("bin")[0]}{Path.Combine(@"\..\..\Presentation\Nop.Web".Split('\\', '/').ToArray())}")
                    .FullName;

            //Presentation\Nop.Web\wwwroot
            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            webHostEnvironment.Setup(p => p.WebRootPath).Returns(Path.Combine(rootPath, "wwwroot"));
            webHostEnvironment.Setup(p => p.ContentRootPath).Returns(rootPath);
            webHostEnvironment.Setup(p => p.EnvironmentName).Returns("test");
            webHostEnvironment.Setup(p => p.ApplicationName).Returns("nopCommerce");
            services.AddSingleton(webHostEnvironment.Object);

            services.AddWebEncoders();

            var httpContext = new DefaultHttpContext
            {
                Request = { Headers = { { HeaderNames.Host, NopTestsDefaults.HostIpAddress } } }
            };

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(p => p.HttpContext).Returns(httpContext);

            services.AddSingleton(httpContextAccessor.Object);

            var actionContextAccessor = new Mock<IActionContextAccessor>();
            actionContextAccessor.Setup(x => x.ActionContext)
                .Returns(new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor()));

            services.AddSingleton(actionContextAccessor.Object);

            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            var urlHelper = new NopTestUrlHelper(actionContextAccessor.Object.ActionContext);

            urlHelperFactory.Setup(x => x.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(urlHelper);

            services.AddTransient(_ => actionContextAccessor.Object);

            services.AddSingleton(urlHelperFactory.Object);

            var tempDataDictionaryFactory = new Mock<ITempDataDictionaryFactory>();
            var dataDictionary = new TempDataDictionary(httpContextAccessor.Object.HttpContext,
                new Mock<ITempDataProvider>().Object);
            tempDataDictionaryFactory.Setup(f => f.GetTempData(It.IsAny<HttpContext>())).Returns(dataDictionary);
            services.AddSingleton(tempDataDictionaryFactory.Object);

            services.AddSingleton<ITypeFinder>(typeFinder);
            Singleton<ITypeFinder>.Instance = typeFinder;

            //file provider
            services.AddTransient<INopFileProvider, NopFileProvider>();
            CommonHelper.DefaultFileProvider = new NopFileProvider(webHostEnvironment.Object);

            //web helper
            services.AddTransient<IWebHelper, WebHelper>();

            //user agent helper
            services.AddTransient<IUserAgentHelper, UserAgentHelper>();

            //data layer
            services.AddTransient<IDataProviderManager, TestDataProviderManager>();
            services.AddTransient(serviceProvider =>
                serviceProvider.GetRequiredService<IDataProviderManager>().DataProvider);
            services.AddTransient<IMappingEntityAccessor>(x => x.GetRequiredService<INopDataProvider>());

            //repositories
            services.AddTransient(typeof(IRepository<>), typeof(EntityRepository<>));

            //plugins
            services.AddTransient<IPluginService, PluginService>();

            services.AddSingleton<IMemoryCache>(memoryCache);
            services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();
            services.AddSingleton<ILocker, MemoryCacheManager>();

            services.AddSingleton<IDistributedCache>(new MemoryDistributedCache(new TestMemoryDistributedCacheoptions()));
            services.AddTransient<MemoryDistributedCacheManager>();
            
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
            services.AddTransient<ISmtpBuilder, TestSmtpBuilder>();
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
            services.AddTransient<IScheduleTaskService, ScheduleTaskService>();
            services.AddTransient<IExportManager, ExportManager>();
            services.AddTransient<IImportManager, ImportManager>();
            services.AddTransient<IPdfService, PdfService>();
            services.AddTransient<IUploadService, UploadService>();
            services.AddTransient<IThemeProvider, ThemeProvider>();
            services.AddTransient<IExternalAuthenticationService, ExternalAuthenticationService>();
            services.AddScoped<IBBCodeHelper, BBCodeHelper>();
            services.AddScoped<IHtmlFormatter, HtmlFormatter>();

            //slug route transformer
            services.AddSingleton<IReviewTypeService, ReviewTypeService>();
            services.AddSingleton<IEventPublisher, EventPublisher>();
            services.AddTransient<ISettingService, SettingService>();

            //plugin managers
            services.AddTransient(typeof(IPluginManager<>), typeof(PluginManager<>));
            services.AddTransient<IAuthenticationPluginManager, AuthenticationPluginManager>();
            services.AddTransient<IMultiFactorAuthenticationPluginManager, MultiFactorAuthenticationPluginManager>();
            services.AddTransient<IWidgetPluginManager, WidgetPluginManager>();
            services.AddTransient<IExchangeRatePluginManager, ExchangeRatePluginManager>();
            services.AddTransient<IDiscountPluginManager, DiscountPluginManager>();
            services.AddTransient<IPaymentPluginManager, PaymentPluginManager>();
            services.AddTransient<IPickupPluginManager, PickupPluginManager>();
            services.AddTransient<IShippingPluginManager, ShippingPluginManager>();
            services.AddTransient<ITaxPluginManager, TaxPluginManager>();
            services.AddScoped<ISearchPluginManager, SearchPluginManager>();

            services.AddTransient<IPictureService, TestPictureService>();
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<INopUrlHelper, NopUrlHelper>();

            //register all settings
            var settings = typeFinder.FindClassesOfType(typeof(ISettings), false).ToList();
            foreach (var setting in settings)
            {
                services.AddTransient(setting,
                    context => context.GetRequiredService<ISettingService>().LoadSettingAsync(setting).Result);
            }

            //event consumers
            foreach (var consumer in typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList())
            {
                var interfaces = consumer.FindInterfaces((type, criteria) => type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition()), typeof(IConsumer<>));
                foreach (var findInterface in interfaces)
                {
                    services.AddTransient(findInterface, consumer);
                }
            }

            services.AddSingleton<IInstallationService, InstallationService>();
            services.AddTransient(p => new Lazy<IVersionLoader>(p.GetRequiredService<IVersionLoader>()));

            services
                // add common FluentMigrator services
                .AddFluentMigratorCore()
                .AddScoped<IProcessorAccessor, TestProcessorAccessor>()
                // set accessor for the connection string
                .AddScoped<IConnectionStringAccessor>(_ => DataSettingsManager.LoadSettings())
                .AddScoped<IMigrationManager, TestMigrationManager>()
                .AddSingleton<IConventionSet, NopTestConventionSet>()
                .ConfigureRunner(rb =>
                    rb.WithVersionTable(new MigrationVersionInfo()).AddSqlServer().AddMySql5().AddPostgres().AddSQLite()
                        // define the assembly containing the migrations
                        .ScanIn(mAssemblies).For.Migrations());

            services.AddTransient<IStoreContext, WebStoreContext>();
            services.AddTransient<Lazy<IStoreContext>>();
            services.AddTransient<IWorkContext, WebWorkContext>();
            services.AddTransient<IThemeContext, ThemeContext>();

            services.AddTransient<INopHtmlHelper, NopHtmlHelper>();

            //schedule tasks
            services.AddSingleton<ITaskScheduler, TestTaskScheduler>();
            services.AddTransient<IScheduleTaskRunner, ScheduleTaskRunner>();

            //WebOptimizer
            services.AddWebOptimizer();

            //common factories
            services.AddTransient<IAclSupportedModelFactory, AclSupportedModelFactory>();
            services.AddTransient<IDiscountSupportedModelFactory, DiscountSupportedModelFactory>();
            services.AddTransient<ILocalizedModelFactory, LocalizedModelFactory>();
            services.AddTransient<IStoreMappingSupportedModelFactory, StoreMappingSupportedModelFactory>();

            //admin factories
            services.AddTransient<IBaseAdminModelFactory, BaseAdminModelFactory>();
            services.AddTransient<IActivityLogModelFactory, ActivityLogModelFactory>();
            services.AddTransient<IAddressAttributeModelFactory, AddressAttributeModelFactory>();
            services.AddTransient<IAffiliateModelFactory, AffiliateModelFactory>();
            services.AddTransient<IBlogModelFactory, BlogModelFactory>();
            services.AddTransient<ICampaignModelFactory, CampaignModelFactory>();
            services.AddTransient<ICategoryModelFactory, CategoryModelFactory>();
            services.AddTransient<ICheckoutAttributeModelFactory, CheckoutAttributeModelFactory>();
            services.AddTransient<ICommonModelFactory, CommonModelFactory>();
            services.AddTransient<ICountryModelFactory, CountryModelFactory>();
            services.AddTransient<ICurrencyModelFactory, CurrencyModelFactory>();
            services.AddTransient<ICustomerAttributeModelFactory, CustomerAttributeModelFactory>();
            services.AddTransient<ICustomerModelFactory, CustomerModelFactory>();
            services.AddTransient<ICustomerRoleModelFactory, CustomerRoleModelFactory>();
            services.AddTransient<IDiscountModelFactory, DiscountModelFactory>();
            services.AddTransient<IEmailAccountModelFactory, EmailAccountModelFactory>();
            services
                .AddTransient<IExternalAuthenticationMethodModelFactory, ExternalAuthenticationMethodModelFactory>();
            services.AddTransient<IForumModelFactory, ForumModelFactory>();
            services.AddTransient<IGiftCardModelFactory, GiftCardModelFactory>();
            services.AddTransient<IHomeModelFactory, HomeModelFactory>();
            services.AddTransient<ILanguageModelFactory, LanguageModelFactory>();
            services.AddTransient<ILogModelFactory, LogModelFactory>();
            services.AddTransient<IManufacturerModelFactory, ManufacturerModelFactory>();
            services.AddTransient<IMeasureModelFactory, MeasureModelFactory>();
            services.AddTransient<IMessageTemplateModelFactory, MessageTemplateModelFactory>();
            services.AddTransient<INewsletterSubscriptionModelFactory, NewsletterSubscriptionModelFactory>();
            services.AddTransient<INewsModelFactory, NewsModelFactory>();
            services.AddTransient<IOrderModelFactory, OrderModelFactory>();
            services.AddTransient<IPaymentModelFactory, PaymentModelFactory>();
            services.AddTransient<IPluginModelFactory, PluginModelFactory>();
            services.AddTransient<IPollModelFactory, PollModelFactory>();
            services.AddTransient<IProductModelFactory, ProductModelFactory>();
            services.AddTransient<IProductAttributeModelFactory, ProductAttributeModelFactory>();
            services.AddTransient<IProductReviewModelFactory, ProductReviewModelFactory>();
            services.AddTransient<IReportModelFactory, ReportModelFactory>();
            services.AddTransient<IQueuedEmailModelFactory, QueuedEmailModelFactory>();
            services.AddTransient<IRecurringPaymentModelFactory, RecurringPaymentModelFactory>();
            services.AddTransient<IReturnRequestModelFactory, ReturnRequestModelFactory>();
            services.AddTransient<IReviewTypeModelFactory, ReviewTypeModelFactory>();
            services.AddTransient<IScheduleTaskModelFactory, ScheduleTaskModelFactory>();
            services.AddTransient<ISecurityModelFactory, SecurityModelFactory>();
            services.AddTransient<ISettingModelFactory, SettingModelFactory>();
            services.AddTransient<IShippingModelFactory, ShippingModelFactory>();
            services.AddTransient<IShoppingCartModelFactory, ShoppingCartModelFactory>();
            services.AddTransient<ISpecificationAttributeModelFactory, SpecificationAttributeModelFactory>();
            services.AddTransient<IStoreModelFactory, StoreModelFactory>();
            services.AddTransient<ITaxModelFactory, TaxModelFactory>();
            services.AddTransient<ITemplateModelFactory, TemplateModelFactory>();
            services.AddTransient<ITopicModelFactory, TopicModelFactory>();
            services.AddTransient<IVendorAttributeModelFactory, VendorAttributeModelFactory>();
            services.AddTransient<IVendorModelFactory, VendorModelFactory>();
            services.AddTransient<IWidgetModelFactory, WidgetModelFactory>();

            //factories
            services.AddTransient<Web.Factories.IAddressModelFactory, Web.Factories.AddressModelFactory>();
            services.AddTransient<Web.Factories.IBlogModelFactory, Web.Factories.BlogModelFactory>();
            services.AddTransient<Web.Factories.ICatalogModelFactory, Web.Factories.CatalogModelFactory>();
            services.AddTransient<Web.Factories.ICheckoutModelFactory, Web.Factories.CheckoutModelFactory>();
            services.AddTransient<Web.Factories.ICommonModelFactory, Web.Factories.CommonModelFactory>();
            services.AddTransient<Web.Factories.ICountryModelFactory, Web.Factories.CountryModelFactory>();
            services.AddTransient<Web.Factories.ICustomerModelFactory, Web.Factories.CustomerModelFactory>();
            services.AddTransient<Web.Factories.IForumModelFactory, Web.Factories.ForumModelFactory>();
            services
                .AddTransient<Web.Factories.IExternalAuthenticationModelFactory,
                    Web.Factories.ExternalAuthenticationModelFactory>();
            services.AddTransient<Web.Factories.INewsModelFactory, Web.Factories.NewsModelFactory>();
            services.AddTransient<Web.Factories.INewsletterModelFactory, Web.Factories.NewsletterModelFactory>();
            services.AddTransient<Web.Factories.IOrderModelFactory, Web.Factories.OrderModelFactory>();
            services.AddTransient<Web.Factories.IPollModelFactory, Web.Factories.PollModelFactory>();
            services
                .AddTransient<Web.Factories.IPrivateMessagesModelFactory, Web.Factories.PrivateMessagesModelFactory>();
            services.AddTransient<Web.Factories.IProductModelFactory, Web.Factories.ProductModelFactory>();
            services.AddTransient<Web.Factories.IProfileModelFactory, Web.Factories.ProfileModelFactory>();
            services.AddTransient<Web.Factories.IReturnRequestModelFactory, Web.Factories.ReturnRequestModelFactory>();
            services.AddTransient<Web.Factories.IShoppingCartModelFactory, Web.Factories.ShoppingCartModelFactory>();
            services.AddTransient<Web.Factories.ISitemapModelFactory, Web.Factories.SitemapModelFactory>();
            services.AddTransient<Web.Factories.ITopicModelFactory, Web.Factories.TopicModelFactory>();
            services.AddTransient<Web.Factories.IVendorModelFactory, Web.Factories.VendorModelFactory>();
            services.AddTransient<Web.Factories.IWidgetModelFactory, Web.Factories.WidgetModelFactory>();

            _serviceProvider = services.BuildServiceProvider();

            EngineContext.Replace(new NopTestEngine(_serviceProvider));

            Init();
        }

        public static T GetService<T>()
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

        public static T GetService<T>(IServiceScope scope)
        {
            try
            {
                return scope.ServiceProvider.GetService<T>();
            }
            catch (InvalidOperationException)
            {
                return (T)EngineContext.Current.ResolveUnregistered(typeof(T));
            }
        }

        public async Task TestCrud<TEntity>(TEntity baseEntity, Func<TEntity, Task> insert, TEntity updateEntity, Func<TEntity, Task> update, Func<int, Task<TEntity>> getById, Func<TEntity, TEntity, bool> equals, Func<TEntity, Task> delete) where TEntity : BaseEntity
        {
            baseEntity.Id = 0;

            await insert(baseEntity);
            baseEntity.Id.Should().BeGreaterThan(0);
            
            updateEntity.Id = baseEntity.Id;
            await update(updateEntity);

            var item = await getById(baseEntity.Id);
            item.Should().NotBeNull();
            equals(updateEntity, item).Should().BeTrue();

            await delete(baseEntity);
            item = await getById(baseEntity.Id);
            item.Should().BeNull();
        }

        public static bool SetDataProviderType(DataProviderType type)
        {
            var dataConfig = Singleton<DataConfig>.Instance ?? new DataConfig();

            dataConfig.DataProvider = type;
            dataConfig.ConnectionString = string.Empty;

            try
            {
                switch (type)
                {
                    case DataProviderType.SqlServer:
                        dataConfig.ConnectionString = _resourceManager.GetString("sql server connection string");
                        break;
                    case DataProviderType.MySql:
                        dataConfig.ConnectionString = _resourceManager.GetString("MySql server connection string");
                        break;
                    case DataProviderType.PostgreSQL:
                        dataConfig.ConnectionString = _resourceManager.GetString("PostgreSql server connection string");
                        break;
                    case DataProviderType.Unknown:
                        dataConfig.ConnectionString = "Data Source=nopCommerceTest.sqlite;Mode=Memory;Cache=Shared";
                        break;
                }
            }
            catch (MissingManifestResourceException)
            {
                //ignore
            }

            Singleton<DataConfig>.Instance = dataConfig;
            var flag = !string.IsNullOrEmpty(dataConfig.ConnectionString);

            if (Singleton<AppSettings>.Instance == null)
                return flag;

            Singleton<AppSettings>.Instance.Update(new List<IConfig> { Singleton<DataConfig>.Instance });

            return flag;
        }

        #region Nested classes

        protected class NopTestUrlHelper : UrlHelperBase
        {
            public NopTestUrlHelper(ActionContext actionContext) : base(actionContext)
            {
            }

            public override string Action(UrlActionContext actionContext)
            {
                return string.Empty;
            }

            public override string RouteUrl(UrlRouteContext routeContext)
            {
                return string.Empty;
            }
        }

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
            public Task SignInAsync(Customer customer, bool isPersistent)
            {
                return Task.CompletedTask;
            }

            public Task SignOutAsync()
            {
                return Task.CompletedTask;
            }

            public async Task<Customer> GetAuthenticatedCustomerAsync()
            {
                return await _serviceProvider.GetService<ICustomerService>().GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
            }
        }

        protected class TestPictureService : PictureService
        {
            public TestPictureService(IDownloadService downloadService,
                IHttpContextAccessor httpContextAccessor, ILogger logger, INopFileProvider fileProvider,
                IProductAttributeParser productAttributeParser, IRepository<Picture> pictureRepository,
                IRepository<PictureBinary> pictureBinaryRepository,
                IRepository<ProductPicture> productPictureRepository, ISettingService settingService,
                IUrlRecordService urlRecordService, IWebHelper webHelper, MediaSettings mediaSettings) : base(
                downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser,
                pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, urlRecordService,
                webHelper, mediaSettings)
            {
            }

            // Travis doesn't support named semaphore, that's why we use implementation without it 
            public override async Task<(string Url, Picture Picture)> GetPictureUrlAsync(Picture picture,
                int targetSize = 0,
                bool showDefaultPicture = true,
                string storeLocation = null,
                PictureType defaultPictureType = PictureType.Entity)
            {
                if (picture == null)
                {
                    return showDefaultPicture
                        ? (await GetDefaultPictureUrlAsync(targetSize, defaultPictureType, storeLocation), null)
                        : (string.Empty, (Picture)null);
                }

                byte[] pictureBinary = null;
                if (picture.IsNew)
                {
                    await DeletePictureThumbsAsync(picture);
                    pictureBinary = await LoadPictureBinaryAsync(picture);

                    if ((pictureBinary?.Length ?? 0) == 0)
                    {
                        return showDefaultPicture
                            ? (await GetDefaultPictureUrlAsync(targetSize, defaultPictureType, storeLocation), picture)
                            : (string.Empty, picture);
                    }

                    //we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown
                    picture = await UpdatePictureAsync(picture.Id,
                        pictureBinary,
                        picture.MimeType,
                        picture.SeoFilename,
                        picture.AltAttribute,
                        picture.TitleAttribute,
                        false,
                        false);
                }

                var seoFileName = picture.SeoFilename; // = GetPictureSeName(picture.SeoFilename); //just for sure

                var lastPart = await GetFileExtensionFromMimeTypeAsync(picture.MimeType);
                string thumbFileName;
                if (targetSize == 0)
                {
                    thumbFileName = !string.IsNullOrEmpty(seoFileName)
                        ? $"{picture.Id:0000000}_{seoFileName}.{lastPart}"
                        : $"{picture.Id:0000000}.{lastPart}";

                    var thumbFilePath = await GetThumbLocalPathAsync(thumbFileName);
                    if (await GeneratedThumbExistsAsync(thumbFilePath, thumbFileName))
                        return (await GetThumbUrlAsync(thumbFileName, storeLocation), picture);

                    pictureBinary ??= await LoadPictureBinaryAsync(picture);

                    //the named mutex helps to avoid creating the same files in different threads,
                    //and does not decrease performance significantly, because the code is blocked only for the specific file.
                    //you should be very careful, mutexes cannot be used in with the await operation
                    //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
                    using var mutex = new Mutex(false, thumbFileName);
                    mutex.WaitOne();
                    try
                    {
                        SaveThumbAsync(thumbFilePath, thumbFileName, string.Empty, pictureBinary).Wait();
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }
                }
                else
                {
                    thumbFileName = !string.IsNullOrEmpty(seoFileName)
                        ? $"{picture.Id:0000000}_{seoFileName}_{targetSize}.{lastPart}"
                        : $"{picture.Id:0000000}_{targetSize}.{lastPart}";

                    var thumbFilePath = await GetThumbLocalPathAsync(thumbFileName);
                    if (await GeneratedThumbExistsAsync(thumbFilePath, thumbFileName))
                        return (await GetThumbUrlAsync(thumbFileName, storeLocation), picture);

                    pictureBinary ??= await LoadPictureBinaryAsync(picture);

                    //the named mutex helps to avoid creating the same files in different threads,
                    //and does not decrease performance significantly, because the code is blocked only for the specific file.
                    //you should be very careful, mutexes cannot be used in with the await operation
                    //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
                    using var mutex = new Mutex(false, thumbFileName);
                    mutex.WaitOne();
                    try
                    {
                        if (pictureBinary != null)
                        {
                            try
                            {
                                using var image = SKBitmap.Decode(pictureBinary);
                                var format = GetImageFormatByMimeType(picture.MimeType);
                                pictureBinary = ImageResize(image, format, targetSize);
                            }
                            catch
                            {
                            }
                        }

                        SaveThumbAsync(thumbFilePath, thumbFileName, string.Empty, pictureBinary).Wait();
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }
                }

                return (await GetThumbUrlAsync(thumbFileName, storeLocation), picture);
            }
        }

        private class TestMemoryDistributedCache
        {
            public TestMemoryDistributedCache()
            {
            }
        }

        private class TestMemoryDistributedCacheoptions : IOptions<MemoryDistributedCacheOptions>
        {
            public MemoryDistributedCacheOptions Value => new();
        }

        #endregion
    }
}