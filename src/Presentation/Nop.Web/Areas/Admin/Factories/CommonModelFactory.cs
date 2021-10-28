using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Security;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents common models factory implementation
    /// </summary>
    public partial class CommonModelFactory : ICommonModelFactory
    {
        #region Fields

        protected AdminAreaSettings AdminAreaSettings { get; }
        protected AppSettings AppSettings { get; }
        protected CatalogSettings CatalogSettings { get; }
        protected CurrencySettings CurrencySettings { get; }
        protected IActionContextAccessor ActionContextAccessor { get; }
        protected IAuthenticationPluginManager AuthenticationPluginManager { get; }
        protected IBaseAdminModelFactory BaseAdminModelFactory { get; }
        protected ICurrencyService CurrencyService { get; }
        protected ICustomerService CustomerService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected INopDataProvider DataProvider { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IExchangeRatePluginManager ExchangeRatePluginManager { get; }
        protected IHttpContextAccessor HttpContextAccessor { get; }
        protected ILanguageService LanguageService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IMaintenanceService MaintenanceService { get; }
        protected IMeasureService MeasureService { get; }
        protected IMultiFactorAuthenticationPluginManager MultiFactorAuthenticationPluginManager { get; }
        protected INopFileProvider FileProvider { get; }
        protected IOrderService OrderService { get; }
        protected IPaymentPluginManager PaymentPluginManager { get; }
        protected IPickupPluginManager PickupPluginManager { get; }
        protected IPluginService PluginService { get; }
        protected IProductService ProductService { get; }
        protected IReturnRequestService ReturnRequestService { get; }
        protected ISearchTermService SearchTermService { get; }
        protected IServiceCollection ServiceCollection { get; }
        protected IShippingPluginManager ShippingPluginManager { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreService StoreService { get; }
        protected ITaxPluginManager TaxPluginManager { get; }
        protected IUrlHelperFactory UrlHelperFactory { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWebHelper WebHelper { get; }
        protected IWidgetPluginManager WidgetPluginManager { get; }
        protected IWorkContext WorkContext { get; }
        protected MeasureSettings MeasureSettings { get; }
        protected NopHttpClient NopHttpClient { get; }
        protected ProxySettings ProxySettings { get; }

        #endregion

        #region Ctor

        public CommonModelFactory(AdminAreaSettings adminAreaSettings,
            AppSettings appSettings,
            CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            IActionContextAccessor actionContextAccessor,
            IAuthenticationPluginManager authenticationPluginManager,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            INopDataProvider dataProvider,
            IDateTimeHelper dateTimeHelper,
            INopFileProvider fileProvider,
            IExchangeRatePluginManager exchangeRatePluginManager,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMaintenanceService maintenanceService,
            IMeasureService measureService,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPickupPluginManager pickupPluginManager,
            IPluginService pluginService,
            IProductService productService,
            IReturnRequestService returnRequestService,
            ISearchTermService searchTermService,
            IServiceCollection serviceCollection,
            IShippingPluginManager shippingPluginManager,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreService storeService,
            ITaxPluginManager taxPluginManager,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            NopHttpClient nopHttpClient,
            ProxySettings proxySettings)
        {
            AdminAreaSettings = adminAreaSettings;
            AppSettings = appSettings;
            CatalogSettings = catalogSettings;
            CurrencySettings = currencySettings;
            ActionContextAccessor = actionContextAccessor;
            AuthenticationPluginManager = authenticationPluginManager;
            BaseAdminModelFactory = baseAdminModelFactory;
            CurrencyService = currencyService;
            CustomerService = customerService;
            EventPublisher = eventPublisher;
            DataProvider = dataProvider;
            DateTimeHelper = dateTimeHelper;
            ExchangeRatePluginManager = exchangeRatePluginManager;
            HttpContextAccessor = httpContextAccessor;
            LanguageService = languageService;
            LocalizationService = localizationService;
            MaintenanceService = maintenanceService;
            MeasureService = measureService;
            MultiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
            FileProvider = fileProvider;
            OrderService = orderService;
            PaymentPluginManager = paymentPluginManager;
            PickupPluginManager = pickupPluginManager;
            PluginService = pluginService;
            ProductService = productService;
            ReturnRequestService = returnRequestService;
            SearchTermService = searchTermService;
            ServiceCollection = serviceCollection;
            ShippingPluginManager = shippingPluginManager;
            StaticCacheManager = staticCacheManager;
            StoreContext = storeContext;
            StoreService = storeService;
            TaxPluginManager = taxPluginManager;
            UrlHelperFactory = urlHelperFactory;
            UrlRecordService = urlRecordService;
            WebHelper = webHelper;
            WidgetPluginManager = widgetPluginManager;
            WorkContext = workContext;
            MeasureSettings = measureSettings;
            NopHttpClient = nopHttpClient;
            ProxySettings = proxySettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare store URL warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareStoreUrlWarningModelAsync(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether current store URL matches the store configured URL
            var store = await StoreContext.GetCurrentStoreAsync();
            var currentStoreUrl = store.Url;
            if (!string.IsNullOrEmpty(currentStoreUrl) &&
                (currentStoreUrl.Equals(WebHelper.GetStoreLocation(false), StringComparison.InvariantCultureIgnoreCase) ||
                currentStoreUrl.Equals(WebHelper.GetStoreLocation(true), StringComparison.InvariantCultureIgnoreCase)))
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.URL.Match")
                });
                return;
            }

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = string.Format(await LocalizationService.GetResourceAsync("Admin.System.Warnings.URL.NoMatch"),
                    currentStoreUrl, WebHelper.GetStoreLocation(false))
            });
        }

        /// <summary>
        /// Prepare copyright removal key warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareRemovalKeyWarningModelAsync(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (!AdminAreaSettings.CheckCopyrightRemovalKey)
                return;

            //try to get a warning
            var warning = string.Empty;
            try
            {
                warning = await NopHttpClient.GetCopyrightWarningAsync();
            }
            catch
            {
                // ignored
            }

            if (string.IsNullOrEmpty(warning))
                return;

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.CopyrightRemovalKey,
                Text = warning,
                DontEncode = true //this text could contain links, so don't encode it
            });
        }

        /// <summary>
        /// Prepare primary exchange rate currency warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareExchangeRateCurrencyWarningModelAsync(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether primary exchange rate currency set
            var primaryExchangeRateCurrency = await CurrencyService.GetCurrencyByIdAsync(CurrencySettings.PrimaryExchangeRateCurrencyId);
            if (primaryExchangeRateCurrency == null)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.ExchangeCurrency.NotSet")
                });
                return;
            }

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.ExchangeCurrency.Set")
            });

            //check whether primary exchange rate currency rate configured
            if (primaryExchangeRateCurrency.Rate != 1)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.ExchangeCurrency.Rate1")
                });
            }
        }

        /// <summary>
        /// Prepare primary store currency warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PreparePrimaryStoreCurrencyWarningModelAsync(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether primary store currency set
            var primaryStoreCurrency = await CurrencyService.GetCurrencyByIdAsync(CurrencySettings.PrimaryStoreCurrencyId);
            if (primaryStoreCurrency == null)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.PrimaryCurrency.NotSet")
                });
                return;
            }

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.PrimaryCurrency.Set")
            });
        }

        /// <summary>
        /// Prepare base weight warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareBaseWeightWarningModelAsync(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether base measure weight set
            var baseWeight = await MeasureService.GetMeasureWeightByIdAsync(MeasureSettings.BaseWeightId);
            if (baseWeight == null)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.DefaultWeight.NotSet")
                });
                return;
            }

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.DefaultWeight.Set")
            });

            //check whether base measure weight ratio configured
            if (baseWeight.Ratio != 1)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.DefaultWeight.Ratio1")
                });
            }
        }

        /// <summary>
        /// Prepare base dimension warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareBaseDimensionWarningModelAsync(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether base measure dimension set
            var baseDimension = await MeasureService.GetMeasureDimensionByIdAsync(MeasureSettings.BaseDimensionId);
            if (baseDimension == null)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.DefaultDimension.NotSet")
                });
                return;
            }

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.DefaultDimension.Set")
            });

            //check whether base measure dimension ratio configured
            if (baseDimension.Ratio != 1)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.DefaultDimension.Ratio1")
                });
            }
        }

        /// <summary>
        /// Prepare payment methods warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PreparePaymentMethodsWarningModelAsync(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether payment methods activated
            if ((await PaymentPluginManager.LoadAllPluginsAsync()).Any())
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.PaymentMethods.OK")
                });
                return;
            }

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.PaymentMethods.NoActive")
            });
        }

        /// <summary>
        /// Prepare plugins warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PreparePluginsWarningModelAsync(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether there are incompatible plugins
            foreach (var pluginName in PluginService.GetIncompatiblePlugins())
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = string.Format(await LocalizationService.GetResourceAsync("Admin.System.Warnings.PluginNotLoaded"), pluginName)
                });
            }

            //check whether there are any collision of loaded assembly
            foreach (var assembly in PluginService.GetAssemblyCollisions())
            {
                //get plugin references message
                var message = (await assembly.Collisions
                    .SelectAwait(async item => string.Format(await LocalizationService
                        .GetResourceAsync("Admin.System.Warnings.PluginRequiredAssembly"), item.PluginName, item.AssemblyName))
                    .AggregateAsync("", (curent, all) => all + ", " + curent)).TrimEnd(',', ' ');

                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = string.Format(await LocalizationService.GetResourceAsync("Admin.System.Warnings.AssemblyHasCollision"),
                        assembly.ShortName, assembly.AssemblyFullNameInMemory, message)
                });
            }
            
            //check whether there are different plugins which try to override the same interface
            var baseLibraries = new[] { "Nop.Core", "Nop.Data", "Nop.Services", "Nop.Web", "Nop.Web.Framework" };
            var overridenServices = ServiceCollection.Where(p =>
                    p.ServiceType.FullName != null &&
                    p.ServiceType.FullName.StartsWith("Nop.", StringComparison.InvariantCulture) &&
                    !p.ServiceType.FullName.StartsWith(
                        typeof(IConsumer<>).FullName?.Replace("~1", string.Empty) ?? string.Empty,
                        StringComparison.InvariantCulture)).Select(p =>
                    KeyValuePair.Create(p.ServiceType.FullName, p.ImplementationType?.Assembly.GetName().Name))
                .Where(p => baseLibraries.All(library =>
                    !p.Value?.StartsWith(library, StringComparison.InvariantCultureIgnoreCase) ?? false))
                .GroupBy(p => p.Key, p => p.Value)
                .Where(p => p.Count() > 1)
                .ToDictionary(p => p.Key, p => p.ToList());

            foreach (var overridenService in overridenServices)
            {
                var assemblies = overridenService.Value
                    .Aggregate("", (current, all) => all + ", " + current).TrimEnd(',', ' ');

                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = string.Format(await LocalizationService.GetResourceAsync("Admin.System.Warnings.PluginsOverrideSameService"), overridenService.Key, assemblies)
                });
            }
        }

        /// <summary>
        /// Prepare performance settings warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PreparePerformanceSettingsWarningModelAsync(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether "IgnoreStoreLimitations" setting disabled
            if (!CatalogSettings.IgnoreStoreLimitations && (await StoreService.GetAllStoresAsync()).Count == 1)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Recommendation,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.Performance.IgnoreStoreLimitations")
                });
            }

            //check whether "IgnoreAcl" setting disabled
            if (!CatalogSettings.IgnoreAcl)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Recommendation,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.Performance.IgnoreAcl")
                });
            }
        }

        /// <summary>
        /// Prepare file permissions warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareFilePermissionsWarningModelAsync(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            var dirPermissionsOk = true;
            var dirsToCheck = FilePermissionHelper.GetDirectoriesWrite();
            foreach (var dir in dirsToCheck)
            {
                if (FilePermissionHelper.CheckPermissions(dir, false, true, true, false))
                    continue;

                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = string.Format(await LocalizationService.GetResourceAsync("Admin.System.Warnings.DirectoryPermission.Wrong"),
                        CurrentOSUser.FullName, dir)
                });
                dirPermissionsOk = false;
            }

            if (dirPermissionsOk)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.DirectoryPermission.OK")
                });
            }

            var filePermissionsOk = true;
            var filesToCheck = FilePermissionHelper.GetFilesWrite();
            foreach (var file in filesToCheck)
            {
                if (FilePermissionHelper.CheckPermissions(file, false, true, true, true))
                    continue;

                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = string.Format(await LocalizationService.GetResourceAsync("Admin.System.Warnings.FilePermission.Wrong"),
                        CurrentOSUser.FullName, file)
                });
                filePermissionsOk = false;
            }

            if (filePermissionsOk)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.FilePermission.OK")
                });
            }
        }

        /// <summary>
        /// Prepare backup file search model
        /// </summary>
        /// <param name="searchModel">Backup file search model</param>
        /// <returns>Backup file search model</returns>
        protected virtual BackupFileSearchModel PrepareBackupFileSearchModel(BackupFileSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare plugins installed warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PreparePluginsInstalledWarningModelAsync(List<SystemWarningModel> models)
        {
            var plugins = await PluginService.GetPluginDescriptorsAsync<IPlugin>(LoadPluginsMode.NotInstalledOnly);

            var notInstalled = plugins.Select(p => p.FriendlyName).ToList();

            if (!notInstalled.Any())
                return;

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Warning,
                DontEncode = true,
                Text = $"{await LocalizationService.GetResourceAsync("Admin.System.Warnings.PluginNotInstalled")}: {string.Join(", ", notInstalled)}. {await LocalizationService.GetResourceAsync("Admin.System.Warnings.PluginNotInstalled.HelpText")}"
            });
        }

        /// <summary>
        /// Prepare plugins enabled warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PreparePluginsEnabledWarningModelAsync(List<SystemWarningModel> models)
        {
            var plugins = await PluginService.GetPluginsAsync<IPlugin>();

            var notEnabled = new List<string>();
            var notEnabledSystemNames = new List<string>();

            foreach (var plugin in plugins)
            {
                var isEnabled = true;

                switch (plugin)
                {
                    case IPaymentMethod paymentMethod:
                        isEnabled = PaymentPluginManager.IsPluginActive(paymentMethod);
                        break;

                    case IShippingRateComputationMethod shippingRateComputationMethod:
                        isEnabled = ShippingPluginManager.IsPluginActive(shippingRateComputationMethod);
                        break;

                    case IPickupPointProvider pickupPointProvider:
                        isEnabled = PickupPluginManager.IsPluginActive(pickupPointProvider);
                        break;

                    case ITaxProvider taxProvider:
                        isEnabled = TaxPluginManager.IsPluginActive(taxProvider);
                        break;

                    case IExternalAuthenticationMethod externalAuthenticationMethod:
                        isEnabled = AuthenticationPluginManager.IsPluginActive(externalAuthenticationMethod);
                        break;

                    case IMultiFactorAuthenticationMethod multiFactorAuthenticationMethod:
                        isEnabled = MultiFactorAuthenticationPluginManager.IsPluginActive(multiFactorAuthenticationMethod);
                        break;

                    case IWidgetPlugin widgetPlugin:
                        isEnabled = WidgetPluginManager.IsPluginActive(widgetPlugin);
                        break;

                    case IExchangeRateProvider exchangeRateProvider:
                        isEnabled = ExchangeRatePluginManager.IsPluginActive(exchangeRateProvider);
                        break;
                }

                if (isEnabled)
                    continue;

                notEnabled.Add(plugin.PluginDescriptor.FriendlyName);
                notEnabledSystemNames.Add(plugin.PluginDescriptor.SystemName);
            }

            if (notEnabled.Any())
            {
                //get URL helper
                var urlHelper = UrlHelperFactory.GetUrlHelper(ActionContextAccessor.ActionContext);

                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    DontEncode = true,

                    Text = $"{await LocalizationService.GetResourceAsync("Admin.System.Warnings.PluginNotEnabled")}: {string.Join(", ", notEnabled)} (<a href=\"{urlHelper.Action("UninstallAndDeleteUnusedPlugins", "Plugin", new { names = notEnabledSystemNames.ToArray() })}\">{await LocalizationService.GetResourceAsync("Admin.System.Warnings.PluginNotEnabled.AutoFixAndRestart")}</a>)"
                });
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare system info model
        /// </summary>
        /// <param name="model">System info model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the system info model
        /// </returns>
        public virtual async Task<SystemInfoModel> PrepareSystemInfoModelAsync(SystemInfoModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.NopVersion = NopVersion.FULL_VERSION;
            model.ServerTimeZone = TimeZoneInfo.Local.StandardName;
            model.ServerLocalTime = DateTime.Now;
            model.UtcTime = DateTime.UtcNow;
            model.CurrentUserTime = await DateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
            model.HttpHost = HttpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];

            //ensure no exception is thrown
            try
            {
                model.OperatingSystem = Environment.OSVersion.VersionString;
                model.AspNetInfo = RuntimeInformation.FrameworkDescription;
                model.IsFullTrust = AppDomain.CurrentDomain.IsFullyTrusted;
            }
            catch
            {
                // ignored
            }

            foreach (var header in HttpContextAccessor.HttpContext.Request.Headers)
            {
                model.Headers.Add(new SystemInfoModel.HeaderModel
                {
                    Name = header.Key,
                    Value = header.Value
                });
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var loadedAssemblyModel = new SystemInfoModel.LoadedAssembly
                {
                    FullName = assembly.FullName
                };

                //ensure no exception is thrown
                try
                {
                    loadedAssemblyModel.Location = assembly.IsDynamic ? null : assembly.Location;
                    loadedAssemblyModel.IsDebug = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false)
                        .FirstOrDefault() is DebuggableAttribute attribute && attribute.IsJITOptimizerDisabled;

                    //https://stackoverflow.com/questions/2050396/getting-the-date-of-a-net-assembly
                    //we use a simple method because the more Jeff Atwood's solution doesn't work anymore 
                    //more info at https://blog.codinghorror.com/determining-build-date-the-hard-way/
                    loadedAssemblyModel.BuildDate = assembly.IsDynamic ? null : (DateTime?)TimeZoneInfo.ConvertTimeFromUtc(FileProvider.GetLastWriteTimeUtc(assembly.Location), TimeZoneInfo.Local);

                }
                catch
                {
                    // ignored
                }

                model.LoadedAssemblies.Add(loadedAssemblyModel);
            }


            var currentStaticCacheManagerName = StaticCacheManager.GetType().Name;

            if (AppSettings.Get<DistributedCacheConfig>().Enabled)
                currentStaticCacheManagerName +=
                    $"({await LocalizationService.GetLocalizedEnumAsync(AppSettings.Get<DistributedCacheConfig>().DistributedCacheType)})";

            model.CurrentStaticCacheManager = currentStaticCacheManagerName;

            model.AzureBlobStorageEnabled = AppSettings.Get<AzureBlobConfig>().Enabled;

            return model;
        }

        /// <summary>
        /// Prepare proxy connection warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareProxyConnectionWarningModelAsync(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //whether proxy is enabled
            if (!ProxySettings.Enabled)
                return;

            try
            {
                await NopHttpClient.PingAsync();

                //connection is OK
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.ProxyConnection.OK")
                });
            }
            catch
            {
                //connection failed
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = await LocalizationService.GetResourceAsync("Admin.System.Warnings.ProxyConnection.Failed")
                });
            }
        }

        /// <summary>
        /// Prepare system warning models
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of system warning models
        /// </returns>
        public virtual async Task<IList<SystemWarningModel>> PrepareSystemWarningModelsAsync()
        {
            var models = new List<SystemWarningModel>();

            //store URL
            await PrepareStoreUrlWarningModelAsync(models);

            //removal key
            await PrepareRemovalKeyWarningModelAsync(models);

            //primary exchange rate currency
            await PrepareExchangeRateCurrencyWarningModelAsync(models);

            //primary store currency
            await PreparePrimaryStoreCurrencyWarningModelAsync(models);

            //base measure weight
            await PrepareBaseWeightWarningModelAsync(models);

            //base dimension weight
            await PrepareBaseDimensionWarningModelAsync(models);

            //payment methods
            await PreparePaymentMethodsWarningModelAsync(models);

            //plugins
            await PreparePluginsWarningModelAsync(models);

            //performance settings
            await PreparePerformanceSettingsWarningModelAsync(models);

            //validate write permissions (the same procedure like during installation)
            await PrepareFilePermissionsWarningModelAsync(models);

            //not active plugins
            await PreparePluginsEnabledWarningModelAsync(models);

            //not install plugins
            await PreparePluginsInstalledWarningModelAsync(models);

            //proxy connection
            await PrepareProxyConnectionWarningModelAsync(models);

            //publish event
            var warningEvent = new SystemWarningCreatedEvent();
            await EventPublisher.PublishAsync(warningEvent);
            //add another warnings (for example from plugins) 
            models.AddRange(warningEvent.SystemWarnings);

            return models;
        }

        /// <summary>
        /// Prepare maintenance model
        /// </summary>
        /// <param name="model">Maintenance model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the maintenance model
        /// </returns>
        public virtual Task<MaintenanceModel> PrepareMaintenanceModelAsync(MaintenanceModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.DeleteGuests.EndDate = DateTime.UtcNow.AddDays(-7);
            model.DeleteGuests.OnlyWithoutShoppingCart = true;
            model.DeleteAbandonedCarts.OlderThan = DateTime.UtcNow.AddDays(-182);

            model.DeleteAlreadySentQueuedEmails.EndDate = DateTime.UtcNow.AddDays(-7);

            model.BackupSupported = DataProvider.BackupSupported;

            //prepare nested search model
            PrepareBackupFileSearchModel(model.BackupFileSearchModel);

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare paged backup file list model
        /// </summary>
        /// <param name="searchModel">Backup file search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the backup file list model
        /// </returns>
        public virtual Task<BackupFileListModel> PrepareBackupFileListModelAsync(BackupFileSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get backup files
            var backupFiles = MaintenanceService.GetAllBackupFiles().ToPagedList(searchModel);

            //prepare list model
            var model = new BackupFileListModel().PrepareToGrid(searchModel, backupFiles, () =>
            {
                return backupFiles.Select(file => new BackupFileModel
                {
                    Name = FileProvider.GetFileName(file),

                    //fill in additional values (not existing in the entity)
                    Length = $"{FileProvider.FileLength(file) / 1024f / 1024f:F2} Mb",

                    Link = $"{WebHelper.GetStoreLocation()}db_backups/{FileProvider.GetFileName(file)}"
                });
            });

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare URL record search model
        /// </summary>
        /// <param name="searchModel">URL record search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the uRL record search model
        /// </returns>
        public virtual async Task<UrlRecordSearchModel> PrepareUrlRecordSearchModelAsync(UrlRecordSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available languages
            //we insert 0 as 'Standard' language.
            //let's insert -1 for 'All' language selection.
            await BaseAdminModelFactory.PrepareLanguagesAsync(searchModel.AvailableLanguages,
                defaultItemText: await LocalizationService.GetResourceAsync("Admin.System.SeNames.List.Language.Standard"));
            searchModel.AvailableLanguages.Insert(0,
                new SelectListItem { Text = await LocalizationService.GetResourceAsync("Admin.Common.All"), Value = "-1" });
            searchModel.LanguageId = -1;

            //prepare "is active" filter (0 - all; 1 - active only; 2 - inactive only)
            searchModel.AvailableActiveOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = await LocalizationService.GetResourceAsync("Admin.System.SeNames.List.IsActive.All")
            });
            searchModel.AvailableActiveOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = await LocalizationService.GetResourceAsync("Admin.System.SeNames.List.IsActive.ActiveOnly")
            });
            searchModel.AvailableActiveOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = await LocalizationService.GetResourceAsync("Admin.System.SeNames.List.IsActive.InactiveOnly")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged URL record list model
        /// </summary>
        /// <param name="searchModel">URL record search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the uRL record list model
        /// </returns>
        public virtual async Task<UrlRecordListModel> PrepareUrlRecordListModelAsync(UrlRecordSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var isActive = searchModel.IsActiveId == 0 ? null : (bool?)(searchModel.IsActiveId == 1);
            var languageId = searchModel.LanguageId < 0 ? null : (int?)(searchModel.LanguageId);

            //get URL records
            var urlRecords = await UrlRecordService.GetAllUrlRecordsAsync(slug: searchModel.SeName,
                languageId: languageId, isActive: isActive,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //get URL helper
            var urlHelper = UrlHelperFactory.GetUrlHelper(ActionContextAccessor.ActionContext);

            //prepare list model
            var model = await new UrlRecordListModel().PrepareToGridAsync(searchModel, urlRecords, () =>
            {
                return urlRecords.SelectAwait(async urlRecord =>
                {
                    //fill in model values from the entity
                    var urlRecordModel = urlRecord.ToModel<UrlRecordModel>();

                    //fill in additional values (not existing in the entity)
                    urlRecordModel.Name = urlRecord.Slug;
                    urlRecordModel.Language = urlRecord.LanguageId == 0
                        ? await LocalizationService.GetResourceAsync("Admin.System.SeNames.Language.Standard")
                        : (await LanguageService.GetLanguageByIdAsync(urlRecord.LanguageId))?.Name ?? "Unknown";

                    //details URL
                    var detailsUrl = string.Empty;
                    var entityName = urlRecord.EntityName?.ToLowerInvariant() ?? string.Empty;
                    switch (entityName)
                    {
                        case "blogpost":
                            detailsUrl = urlHelper.Action("BlogPostEdit", "Blog", new { id = urlRecord.EntityId });
                            break;
                        case "category":
                            detailsUrl = urlHelper.Action("Edit", "Category", new { id = urlRecord.EntityId });
                            break;
                        case "manufacturer":
                            detailsUrl = urlHelper.Action("Edit", "Manufacturer", new { id = urlRecord.EntityId });
                            break;
                        case "product":
                            detailsUrl = urlHelper.Action("Edit", "Product", new { id = urlRecord.EntityId });
                            break;
                        case "newsitem":
                            detailsUrl = urlHelper.Action("NewsItemEdit", "News", new { id = urlRecord.EntityId });
                            break;
                        case "topic":
                            detailsUrl = urlHelper.Action("Edit", "Topic", new { id = urlRecord.EntityId });
                            break;
                        case "vendor":
                            detailsUrl = urlHelper.Action("Edit", "Vendor", new { id = urlRecord.EntityId });
                            break;
                    }

                    urlRecordModel.DetailsUrl = detailsUrl;

                    return urlRecordModel;
                });
            });
            return model;
        }

        /// <summary>
        /// Prepare language selector model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the language selector model
        /// </returns>
        public virtual async Task<LanguageSelectorModel> PrepareLanguageSelectorModelAsync()
        {
            var store = await StoreContext.GetCurrentStoreAsync();
            var model = new LanguageSelectorModel
            {
                CurrentLanguage = (await WorkContext.GetWorkingLanguageAsync()).ToModel<LanguageModel>(),
                AvailableLanguages = (await LanguageService
                    .GetAllLanguagesAsync(storeId: store.Id))
                    .Select(language => language.ToModel<LanguageModel>()).ToList()
            };

            return model;
        }

        /// <summary>
        /// Prepare popular search term search model
        /// </summary>
        /// <param name="searchModel">Popular search term search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the popular search term search model
        /// </returns>
        public virtual Task<PopularSearchTermSearchModel> PreparePopularSearchTermSearchModelAsync(PopularSearchTermSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize(5);

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged popular search term list model
        /// </summary>
        /// <param name="searchModel">Popular search term search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the popular search term list model
        /// </returns>
        public virtual async Task<PopularSearchTermListModel> PreparePopularSearchTermListModelAsync(PopularSearchTermSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get popular search terms
            var searchTermRecordLines = await SearchTermService.GetStatsAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new PopularSearchTermListModel().PrepareToGrid(searchModel, searchTermRecordLines, () =>
            {
                return searchTermRecordLines.Select(searchTerm => new PopularSearchTermModel
                {
                    Keyword = searchTerm.Keyword,
                    Count = searchTerm.Count
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare common statistics model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the common statistics model
        /// </returns>
        public virtual async Task<CommonStatisticsModel> PrepareCommonStatisticsModelAsync()
        {
            var model = new CommonStatisticsModel
            {
                NumberOfOrders = (await OrderService.SearchOrdersAsync(pageIndex: 0, pageSize: 1, getOnlyTotalCount: true)).TotalCount
            };

            var customerRoleIds = new[] { (await CustomerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName)).Id };
            model.NumberOfCustomers = (await CustomerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds,
                pageIndex: 0, pageSize: 1, getOnlyTotalCount: true)).TotalCount;

            var returnRequestStatus = ReturnRequestStatus.Pending;
            model.NumberOfPendingReturnRequests = (await ReturnRequestService.SearchReturnRequestsAsync(rs: returnRequestStatus,
                pageIndex: 0, pageSize: 1, getOnlyTotalCount: true)).TotalCount;

            model.NumberOfLowStockProducts =
                (await ProductService.GetLowStockProductsAsync(getOnlyTotalCount: true)).TotalCount +
                (await ProductService.GetLowStockProductCombinationsAsync(getOnlyTotalCount: true)).TotalCount;

            return model;
        }

        #endregion
    }
}