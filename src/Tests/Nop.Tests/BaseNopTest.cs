﻿using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using FluentAssertions;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
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
using Nop.Data.Migrations;
using Nop.Services.Affiliates;
using Nop.Services.Attributes;
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
using Nop.Tests.Nop.Web.Tests.Public.Factories;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;
using Nop.Web.Framework.WebOptimizer;
using Nop.Web.Infrastructure.Installation;
using SkiaSharp;
using IAuthenticationService = Nop.Services.Authentication.IAuthenticationService;
using Task = System.Threading.Tasks.Task;

namespace Nop.Tests;

public partial class BaseNopTest
{
    private static readonly ServiceProvider _serviceProvider;
    private static readonly ResourceManager _resourceManager;

    protected BaseNopTest()
    {
        SetDataProviderType(DataProviderType.Unknown);
    }

    public ServiceProvider ServiceProvider => _serviceProvider;

    private static void Init()
    {
        var dataProvider = _serviceProvider.GetService<IDataProviderManager>().DataProvider;

        dataProvider.CreateDatabase(null);
        dataProvider.InitializeDatabase();
        
        var installationService = _serviceProvider.GetService<IInstallationService>();

        installationService.InstallAsync(
            new InstallationSettings
            {
                AdminEmail = NopTestsDefaults.AdminEmail,
                AdminPassword = NopTestsDefaults.AdminPassword,
                LanguagePackDownloadLink = string.Empty,
                LanguagePackProgress = 0,
                RegionInfo = new RegionInfo(NopCommonDefaults.DefaultLanguageCulture),
                CultureInfo = new CultureInfo(NopCommonDefaults.DefaultLanguageCulture),
                InstallSampleData = true
            }).Wait();
        
        var permissionService = EngineContext.Current.Resolve<IPermissionService>();
        permissionService.InsertPermissionsAsync().Wait();
    }

    protected static void PropertiesShouldEqual<T1, T2>(T1 obj1, T2 obj2, params string[] filter)
    {
        var object1Properties = typeof(T1).GetProperties();
        var object2Properties = typeof(T2).GetProperties();

        foreach (var object1Property in object1Properties)
        {
            var name = object1Property.Name;

            if (filter.Contains(name))
                continue;

            var object2Property = Array.Find(object2Properties, p => p.Name == name);

            if (object2Property == null)
                continue;

            var object1PropertyValue = object1Property.GetValue(obj1);
            var object2PropertyValue = object2Property.GetValue(obj2);

            object1PropertyValue.Should().Be(object2PropertyValue, $"The property \"{typeof(T1).Name}.{object1Property.Name}\" of these objects is not equal");
        }
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

        var rootPath =
            new DirectoryInfo(
                    $"{Directory.GetCurrentDirectory().Split("bin")[0]}{Path.Combine([.. @"\..\..\Presentation\Nop.Web".Split('\\', '/')])}")
                .FullName;

        //Presentation\Nop.Web\wwwroot
        var webHostEnvironment = new Mock<IWebHostEnvironment>();
        webHostEnvironment.Setup(p => p.WebRootPath).Returns(Path.Combine(rootPath, "wwwroot"));
        webHostEnvironment.Setup(p => p.ContentRootPath).Returns(rootPath);
        webHostEnvironment.Setup(p => p.EnvironmentName).Returns("test");
        webHostEnvironment.Setup(p => p.ApplicationName).Returns("nopCommerce");
        services.AddSingleton(webHostEnvironment.Object);

        var htmlHelper = new Mock<IHtmlHelper>();
        services.AddSingleton(htmlHelper.Object);

        //file provider
        services.AddTransient<INopFileProvider, NopFileProvider>();
        CommonHelper.DefaultFileProvider = new NopFileProvider(webHostEnvironment.Object);

        services.AddHttpClient();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var typeFinder = new WebAppTypeFinder();
        Singleton<ITypeFinder>.Instance = typeFinder;

        var mAssemblies = typeFinder.FindClassesOfType<ForwardOnlyMigration>()
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

        services.AddWebEncoders();

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append(HeaderNames.Host, NopTestsDefaults.HostIpAddress);
        httpContext.Session = new TestSeesion();

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

        //web helper
        services.AddTransient<IWebHelper, WebHelper>();

        //user agent helper
        services.AddTransient<IUserAgentHelper, UserAgentHelper>();

        //data layer
        services.AddSingleton<IDataProviderManager, TestDataProviderManager>();
        services.AddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<IDataProviderManager>().DataProvider);

        //repositories
        services.AddTransient(typeof(IRepository<>), typeof(EntityRepository<>));

        //plugins
        services.AddTransient<IPluginService, PluginService>();

        services.AddScoped<IShortTermCacheManager, PerRequestCacheManager>();

        services.AddSingleton<ICacheKeyManager, CacheKeyManager>();
        services.AddSingleton<IMemoryCache>(memoryCache);
        services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();
        services.AddSingleton<ILocker, MemoryCacheLocker>();
        services.AddSingleton<MemoryCacheLocker>();

        services.AddTransient(typeof(IConcurrentCollection<>), typeof(ConcurrentTrie<>));

        var memoryDistributedCache = new MemoryDistributedCache(new TestMemoryDistributedCacheOptions());
        services.AddSingleton<IDistributedCache>(memoryDistributedCache);
        services.AddScoped<MemoryDistributedCacheManager>();
        services.AddSingleton(new DistributedCacheLocker(memoryDistributedCache));

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
        services.AddTransient<IAddressService, AddressService>();
        services.AddTransient<IAffiliateService, AffiliateService>();
        services.AddTransient<IVendorService, VendorService>();

        //attribute services
        services.AddScoped(typeof(IAttributeService<,>), typeof(AttributeService<,>));

        //attribute parsers
        services.AddScoped(typeof(IAttributeParser<,>), typeof(AttributeParser<,>));

        //attribute formatter
        services.AddScoped(typeof(IAttributeFormatter<,>), typeof(AttributeFormatter<,>));

        services.AddTransient<ISearchTermService, SearchTermService>();
        services.AddTransient<IGenericAttributeService, GenericAttributeService>();
        services.AddTransient<IMaintenanceService, MaintenanceService>();
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
        services.AddTransient(typeof(Lazy<ILocalizationService>));
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
        services.AddTransient<ICheckVatService, CheckVatService>();
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

        services.AddScoped<INopAssetHelper, NopAssetHelper>();

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
            .AddScoped<IMigrationManager, MigrationManager>()
            .AddScoped<Lazy<IMigrationManager>>()
            .AddSingleton<IConventionSet, NopTestConventionSet>()
            .ConfigureRunner(rb =>
                rb.WithVersionTable(new MigrationVersionInfo()).AddSqlServer().AddMySql5().AddPostgres().AddSQLite()
                    // define the assembly containing the migrations
                    .ScanIn(mAssemblies).For.Migrations());

        services.AddOptions<GeneratorOptions>().Configure(go => go.CompatibilityMode = CompatibilityMode.LOOSE);

        services.AddTransient<IStoreContext, WebStoreContext>();
        services.AddTransient<Lazy<IStoreContext>>();
        services.AddTransient<IWorkContext, WebWorkContext>();
        services.AddTransient<Lazy<IWorkContext>>();
        services.AddTransient<IThemeContext, ThemeContext>();
        services.AddTransient<Lazy<ILocalizationService>>();
        services.AddTransient<INopHtmlHelper, NopHtmlHelper>();

        //schedule tasks
        services.AddSingleton<ITaskScheduler, TestTaskScheduler>();
        services.AddTransient<IScheduleTaskRunner, ScheduleTaskRunner>();

        //WebOptimizer
        services.AddWebOptimizer();

        //common factories
        services.AddTransient<IDiscountSupportedModelFactory, DiscountSupportedModelFactory>();
        services.AddTransient<ILocalizedModelFactory, LocalizedModelFactory>();
        services.AddTransient<IStoreMappingSupportedModelFactory, StoreMappingSupportedModelFactory>();

        //admin factories
        services.AddTransient<IAclSupportedModelFactory, AclSupportedModelFactory>();
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
        services.AddTransient<ProductModelFactoryTests.ProductModelFactoryForTest>();
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
        services.AddTransient<Web.Framework.Factories.IWidgetModelFactory, Web.Framework.Factories.WidgetModelFactory>();

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
        services.AddTransient<Web.Factories.IJsonLdModelFactory, Web.Factories.JsonLdModelFactory>();
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

        _serviceProvider = services.BuildServiceProvider();

        EngineContext.Replace(new NopTestEngine(_serviceProvider));

        Init();
    }

    protected static T GetService<T>()
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    protected static T GetService<T>(IServiceScope scope)
    {
        return scope.ServiceProvider.GetService<T>();
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
            IProductAttributeParser productAttributeParser, IProductAttributeService productAttributeService,
            IRepository<Picture> pictureRepository, IRepository<PictureBinary> pictureBinaryRepository,
            IRepository<ProductPicture> productPictureRepository, ISettingService settingService,
            IUrlRecordService urlRecordService, IWebHelper webHelper, MediaSettings mediaSettings) : base(
            downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService,
            pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, urlRecordService,
            webHelper, mediaSettings)
        {
        }

        // Not all CI/CD support working with named semaphore, that's why we use implementation without it 
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

    private class TestMemoryDistributedCacheOptions : IOptions<MemoryDistributedCacheOptions>
    {
        public MemoryDistributedCacheOptions Value => new();
    }

    private class TestSeesion : ISession
    {
        private static ConcurrentDictionary<string, byte[]> _sessison = new();

        public Task LoadAsync(CancellationToken cancellationToken = new())
        {
            return Task.CompletedTask;
        }

        public Task CommitAsync(CancellationToken cancellationToken = new())
        {
            return Task.CompletedTask;
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            return _sessison.TryGetValue(key, out value);
        }

        public void Set(string key, byte[] value)
        {
            if (!_sessison.ContainsKey(key))
                _sessison.TryAdd(key, value);
            else
                _sessison[key] = value;
        }

        public void Remove(string key)
        {
            _sessison.Remove(key, out _);
        }

        public void Clear()
        {
            _sessison.Clear();
        }

        public bool IsAvailable => true;
        public string Id => "nop_test_session";
        public IEnumerable<string> Keys => _sessison.Keys;
    }

    #endregion
}