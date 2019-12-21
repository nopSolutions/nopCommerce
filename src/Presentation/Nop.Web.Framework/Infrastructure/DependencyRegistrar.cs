using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Redis;
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
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Installation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Media.RoxyFileman;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Plugins.Marketplace;
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
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Seo;
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
        /// <param name="services">Service Collection</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(IServiceCollection services, ITypeFinder typeFinder, NopConfig config)
        {
            //file provider
            services.AddScoped<INopFileProvider, NopFileProvider>();

            //web helper
            services.AddScoped<IWebHelper, WebHelper>();

            //user agent helper
            services.AddScoped<IUserAgentHelper, UserAgentHelper>();

            //data layer
            services.AddTransient<IDataProviderManager, EfDataProviderManager>();
            services.AddTransient(provider => provider.GetService<IDataProviderManager>().DataProvider);
            services.AddScoped<IDbContext>(provider => new NopObjectContext(provider.GetService<DbContextOptions<NopObjectContext>>()));

            //repositories
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            //plugins
            services.AddScoped<IPluginService, PluginService>();
            services.AddScoped<OfficialFeedManager>();

            //cache manager
            services.AddScoped<ICacheManager, PerRequestCacheManager>();

            //redis connection wrapper
            if (config.RedisEnabled)
            {
                services.AddSingleton<RedisConnectionWrapper>();
                services.AddSingleton<ILocker, RedisConnectionWrapper>();
                services.AddSingleton<IRedisConnectionWrapper>(provider => provider.GetService<RedisConnectionWrapper>());
            }

            //static cache manager
            if (config.RedisEnabled && config.UseRedisForCaching)
            {
                services.AddScoped<RedisCacheManager>();
                services.AddScoped<IStaticCacheManager, RedisCacheManager>();
            }
            else
            {
                services.AddSingleton<MemoryCacheManager>();
                services.AddSingleton<ILocker, MemoryCacheManager>();
                services.AddSingleton<IStaticCacheManager>(provider => provider.GetService<MemoryCacheManager>());
            }

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
            services.AddScoped<IVendorAttributeFormatter, VendorAttributeFormatter>();
            services.AddScoped<IVendorAttributeParser, VendorAttributeParser>();
            services.AddScoped<IVendorAttributeService, VendorAttributeService>();
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
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<IEmailAccountService, EmailAccountService>();
            services.AddScoped<IWorkflowMessageService, WorkflowMessageService>();
            services.AddScoped<IMessageTokenProvider, MessageTokenProvider>();
            services.AddScoped<ITokenizer, Tokenizer>();
            services.AddScoped<ISmtpBuilder, SmtpBuilder>();
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
            services.AddScoped<IGdprService, GdprService>();
            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IDateTimeHelper, DateTimeHelper>();
            services.AddScoped<ISitemapGenerator, SitemapGenerator>();
            services.AddScoped<IPageHeadBuilder, PageHeadBuilder>();
            services.AddScoped<IScheduleTaskService, ScheduleTaskService>();
            services.AddScoped<IExportManager, ExportManager>();
            services.AddScoped<IImportManager, ImportManager>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IThemeProvider, ThemeProvider>();
            services.AddScoped<IThemeContext, ThemeContext>();
            services.AddScoped<IExternalAuthenticationService, ExternalAuthenticationService>();
            services.AddSingleton<IRoutePublisher, RoutePublisher>();
            services.AddSingleton<IReviewTypeService, ReviewTypeService>();
            services.AddSingleton<IEventPublisher, EventPublisher>();
            services.AddScoped<ISettingService, SettingService>();

            //plugin managers
            services.AddScoped(typeof(IPluginManager<>), typeof(PluginManager<>));
            services.AddScoped<IAuthenticationPluginManager, AuthenticationPluginManager>();
            services.AddScoped<IWidgetPluginManager, WidgetPluginManager>();
            services.AddScoped<IExchangeRatePluginManager, ExchangeRatePluginManager>();
            services.AddScoped<IDiscountPluginManager, DiscountPluginManager>();
            services.AddScoped<IPaymentPluginManager, PaymentPluginManager>();
            services.AddScoped<IPickupPluginManager, PickupPluginManager>();
            services.AddScoped<IShippingPluginManager, ShippingPluginManager>();
            services.AddScoped<ITaxPluginManager, TaxPluginManager>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddSingleton<GenericRouteTransformer>();

            //picture service
            if (config.AzureBlobStorageEnabled)
                services.AddScoped<IPictureService, AzurePictureService>();
            else
                services.AddScoped<IPictureService, PictureService>();

            //roxy file manager service
            services.AddScoped(provider =>
            {
                var pictureService = provider.GetService<IPictureService>();

                return (IRoxyFilemanService)EngineContext.Current.ResolveUnregistered(pictureService.StoreInDb
                    ? typeof(DatabaseRoxyFilemanService)
                    : typeof(FileRoxyFilemanService));
            });

            //installation service
            if (!DataSettingsManager.DatabaseIsInstalled)
            {
                if (config.UseFastInstallationService)
                    services.AddScoped<IInstallationService, SqlFileInstallationService>();
                else
                    services.AddScoped<IInstallationService, CodeFirstInstallationService>();
            }

            //register all event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
            {
                var implInterfaces = consumer.FindInterfaces((type, criteria) =>
                 {
                     var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                     return isMatch;
                 }, typeof(IConsumer<>));
                foreach (var type in implInterfaces)
                {
                    services.AddScoped(type, consumer);
                }
            }

            //register all settings
            var settings = typeFinder.FindClassesOfType(typeof(ISettings)).ToList();
            foreach (var setting in settings)
            {
                services.AddSingleton(setting, provider =>
                 {
                     var instance = provider.GetService<ISettingService>().LoadSetting(setting);
                     return instance;
                 });
            }
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order => 0;
    }
}