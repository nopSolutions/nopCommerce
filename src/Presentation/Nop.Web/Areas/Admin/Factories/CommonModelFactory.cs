using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
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

        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly INopDataProvider _dataProvider;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExchangeRatePluginManager _exchangeRatePluginManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IMeasureService _measureService;
        private readonly INopFileProvider _fileProvider;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPickupPluginManager _pickupPluginManager;
        private readonly IPluginService _pluginService;
        private readonly IProductService _productService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly ISearchTermService _searchTermService;
        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ITaxPluginManager _taxPluginManager;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IWorkContext _workContext;
        private readonly MeasureSettings _measureSettings;
        private readonly NopConfig _nopConfig;
        private readonly NopHttpClient _nopHttpClient;
        private readonly ProxySettings _proxySettings;

        #endregion

        #region Ctor

        public CommonModelFactory(AdminAreaSettings adminAreaSettings,
            CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            IActionContextAccessor actionContextAccessor,
            IAuthenticationPluginManager authenticationPluginManager,
            ICurrencyService currencyService,
            ICustomerService customerService,
            INopDataProvider dataProvider,
            IDateTimeHelper dateTimeHelper,
            INopFileProvider fileProvider,
            IExchangeRatePluginManager exchangeRatePluginManager,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMaintenanceService maintenanceService,
            IMeasureService measureService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPickupPluginManager pickupPluginManager,
            IPluginService pluginService,
            IProductService productService,
            IReturnRequestService returnRequestService,
            ISearchTermService searchTermService,
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
            NopConfig nopConfig,
            NopHttpClient nopHttpClient,
            ProxySettings proxySettings)
        {
            _adminAreaSettings = adminAreaSettings;
            _catalogSettings = catalogSettings;
            _currencySettings = currencySettings;
            _actionContextAccessor = actionContextAccessor;
            _authenticationPluginManager = authenticationPluginManager;
            _currencyService = currencyService;
            _customerService = customerService;
            _dataProvider = dataProvider;
            _dateTimeHelper = dateTimeHelper;
            _exchangeRatePluginManager = exchangeRatePluginManager;
            _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
            _localizationService = localizationService;
            _maintenanceService = maintenanceService;
            _measureService = measureService;
            _fileProvider = fileProvider;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _pickupPluginManager = pickupPluginManager;
            _pluginService = pluginService;
            _productService = productService;
            _returnRequestService = returnRequestService;
            _searchTermService = searchTermService;
            _shippingPluginManager = shippingPluginManager;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeService = storeService;
            _taxPluginManager = taxPluginManager;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _widgetPluginManager = widgetPluginManager;
            _workContext = workContext;
            _measureSettings = measureSettings;
            _nopConfig = nopConfig;
            _nopHttpClient = nopHttpClient;
            _proxySettings = proxySettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare store URL warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        protected virtual void PrepareStoreUrlWarningModel(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether current store URL matches the store configured URL
            var currentStoreUrl = _storeContext.CurrentStore.Url;
            if (!string.IsNullOrEmpty(currentStoreUrl) &&
                (currentStoreUrl.Equals(_webHelper.GetStoreLocation(false), StringComparison.InvariantCultureIgnoreCase) ||
                currentStoreUrl.Equals(_webHelper.GetStoreLocation(true), StringComparison.InvariantCultureIgnoreCase)))
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.URL.Match")
                });
                return;
            }

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = string.Format(_localizationService.GetResource("Admin.System.Warnings.URL.NoMatch"),
                    currentStoreUrl, _webHelper.GetStoreLocation(false))
            });
        }

        /// <summary>
        /// Prepare copyright removal key warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        protected virtual void PrepareRemovalKeyWarningModel(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (!_adminAreaSettings.CheckCopyrightRemovalKey)
                return;

            //try to get a warning
            var warning = string.Empty;
            try
            {
                warning = _nopHttpClient.GetCopyrightWarningAsync().Result;
            }
            catch { }
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
        protected virtual void PrepareExchangeRateCurrencyWarningModel(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether primary exchange rate currency set
            var primaryExchangeRateCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryExchangeRateCurrencyId);
            if (primaryExchangeRateCurrency == null)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.ExchangeCurrency.NotSet")
                });
                return;
            }

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = _localizationService.GetResource("Admin.System.Warnings.ExchangeCurrency.Set")
            });

            //check whether primary exchange rate currency rate configured
            if (primaryExchangeRateCurrency.Rate != 1)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.ExchangeCurrency.Rate1")
                });
            }
        }

        /// <summary>
        /// Prepare primary store currency warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        protected virtual void PreparePrimaryStoreCurrencyWarningModel(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether primary store currency set
            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (primaryStoreCurrency == null)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.PrimaryCurrency.NotSet")
                });
                return;
            }

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = _localizationService.GetResource("Admin.System.Warnings.PrimaryCurrency.Set")
            });
        }

        /// <summary>
        /// Prepare base weight warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        protected virtual void PrepareBaseWeightWarningModel(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether base measure weight set
            var baseWeight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId);
            if (baseWeight == null)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.DefaultWeight.NotSet")
                });
                return;
            }

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = _localizationService.GetResource("Admin.System.Warnings.DefaultWeight.Set")
            });

            //check whether base measure weight ratio configured
            if (baseWeight.Ratio != 1)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.DefaultWeight.Ratio1")
                });
            }
        }

        /// <summary>
        /// Prepare base dimension warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        protected virtual void PrepareBaseDimensionWarningModel(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether base measure dimension set
            var baseDimension = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId);
            if (baseDimension == null)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.DefaultDimension.NotSet")
                });
                return;
            }

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = _localizationService.GetResource("Admin.System.Warnings.DefaultDimension.Set")
            });

            //check whether base measure dimension ratio configured
            if (baseDimension.Ratio != 1)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.DefaultDimension.Ratio1")
                });
            }
        }

        /// <summary>
        /// Prepare payment methods warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        protected virtual void PreparePaymentMethodsWarningModel(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether payment methods activated
            if (_paymentPluginManager.LoadAllPlugins().Any())
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.PaymentMethods.OK")
                });
                return;
            }

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = _localizationService.GetResource("Admin.System.Warnings.PaymentMethods.NoActive")
            });
        }

        /// <summary>
        /// Prepare plugins warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        protected virtual void PreparePluginsWarningModel(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether there are incompatible plugins
            foreach (var pluginName in _pluginService.GetIncompatiblePlugins())
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = string.Format(_localizationService.GetResource("Admin.System.Warnings.PluginNotLoaded"), pluginName)
                });
            }

            //check whether there are any collision of loaded assembly
            foreach (var assembly in _pluginService.GetAssemblyCollisions())
            {
                //get plugin references message
                var message = assembly.Collisions
                    .Select(item => string.Format(_localizationService
                        .GetResource("Admin.System.Warnings.PluginRequiredAssembly"), item.PluginName, item.AssemblyName))
                    .Aggregate("", (curent, all) => all + ", " + curent).TrimEnd(',', ' ');

                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = string.Format(_localizationService.GetResource("Admin.System.Warnings.AssemblyHasCollision"),
                        assembly.ShortName, assembly.AssemblyFullNameInMemory, message)
                });
            }
        }

        /// <summary>
        /// Prepare performance settings warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        protected virtual void PreparePerformanceSettingsWarningModel(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether "IgnoreStoreLimitations" setting disabled
            if (!_catalogSettings.IgnoreStoreLimitations && _storeService.GetAllStores().Count == 1)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Recommendation,
                    Text = _localizationService.GetResource("Admin.System.Warnings.Performance.IgnoreStoreLimitations")
                });
            }

            //check whether "IgnoreAcl" setting disabled
            if (!_catalogSettings.IgnoreAcl)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Recommendation,
                    Text = _localizationService.GetResource("Admin.System.Warnings.Performance.IgnoreAcl")
                });
            }
        }

        /// <summary>
        /// Prepare file permissions warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        protected virtual void PrepareFilePermissionsWarningModel(IList<SystemWarningModel> models)
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
                    Text = string.Format(_localizationService.GetResource("Admin.System.Warnings.DirectoryPermission.Wrong"),
                        CurrentOSUser.FullName, dir)
                });
                dirPermissionsOk = false;
            }

            if (dirPermissionsOk)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.DirectoryPermission.OK")
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
                    Text = string.Format(_localizationService.GetResource("Admin.System.Warnings.FilePermission.Wrong"),
                        CurrentOSUser.FullName, file)
                });
                filePermissionsOk = false;
            }

            if (filePermissionsOk)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.FilePermission.OK")
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
        /// Prepare plugins enabled warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        protected virtual void PreparePluginsEnabledWarningModel(List<SystemWarningModel> models)
        {
            var plugins = _pluginService.GetPlugins<IPlugin>();

            var notEnabled = new List<string>();

            foreach (var plugin in plugins)
            {
                var isEnabled = true;

                switch (plugin)
                {
                    case IPaymentMethod paymentMethod:
                        isEnabled = _paymentPluginManager.IsPluginActive(paymentMethod);
                        break;

                    case IShippingRateComputationMethod shippingRateComputationMethod:
                        isEnabled = _shippingPluginManager.IsPluginActive(shippingRateComputationMethod);
                        break;

                    case IPickupPointProvider pickupPointProvider:
                        isEnabled = _pickupPluginManager.IsPluginActive(pickupPointProvider);
                        break;

                    case ITaxProvider taxProvider:
                        isEnabled = _taxPluginManager.IsPluginActive(taxProvider);
                        break;

                    case IExternalAuthenticationMethod externalAuthenticationMethod:
                        isEnabled = _authenticationPluginManager.IsPluginActive(externalAuthenticationMethod);
                        break;

                    case IWidgetPlugin widgetPlugin:
                        isEnabled = _widgetPluginManager.IsPluginActive(widgetPlugin);
                        break;

                    case IExchangeRateProvider exchangeRateProvider:
                        isEnabled = _exchangeRatePluginManager.IsPluginActive(exchangeRateProvider);
                        break;
                }

                if (isEnabled)
                    continue;

                notEnabled.Add(plugin.PluginDescriptor.FriendlyName);
            }

            if (notEnabled.Any())
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = $"{_localizationService.GetResource("Admin.System.Warnings.PluginNotEnabled")}: {string.Join(", ", notEnabled)}"
                });
            }
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Prepare system info model
        /// </summary>
        /// <param name="model">System info model</param>
        /// <returns>System info model</returns>
        public virtual SystemInfoModel PrepareSystemInfoModel(SystemInfoModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.NopVersion = NopVersion.CurrentVersion;
            model.ServerTimeZone = TimeZoneInfo.Local.StandardName;
            model.ServerLocalTime = DateTime.Now;
            model.UtcTime = DateTime.UtcNow;
            model.CurrentUserTime = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            model.HttpHost = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];

            //ensure no exception is thrown
            try
            {
                model.OperatingSystem = Environment.OSVersion.VersionString;
                model.AspNetInfo = RuntimeEnvironment.GetSystemVersion();
                model.IsFullTrust = AppDomain.CurrentDomain.IsFullyTrusted.ToString();
            }
            catch { }

            foreach (var header in _httpContextAccessor.HttpContext.Request.Headers)
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
                    loadedAssemblyModel.BuildDate = assembly.IsDynamic ? null : (DateTime?)TimeZoneInfo.ConvertTimeFromUtc(_fileProvider.GetLastWriteTimeUtc(assembly.Location), TimeZoneInfo.Local);

                }
                catch { }
                model.LoadedAssemblies.Add(loadedAssemblyModel);
            }
            
            model.CurrentStaticCacheManager = _staticCacheManager.GetType().Name;

            model.RedisEnabled = _nopConfig.RedisEnabled;
            model.UseRedisToStoreDataProtectionKeys = _nopConfig.UseRedisToStoreDataProtectionKeys;
            model.UseRedisForCaching = _nopConfig.UseRedisForCaching;
            model.UseRedisToStorePluginsInfo = _nopConfig.UseRedisToStorePluginsInfo;

            model.AzureBlobStorageEnabled = _nopConfig.AzureBlobStorageEnabled;

            return model;
        }

        /// <summary>
        /// Prepare proxy connection warning model
        /// </summary>
        /// <param name="models">List of system warning models</param>
        protected virtual void PrepareProxyConnectionWarningModel(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //whether proxy is enabled
            if (!_proxySettings.Enabled)
                return;

            try
            {
                _nopHttpClient.PingAsync().Wait();

                //connection is OK
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.ProxyConnection.OK")
                });
            }
            catch
            {
                //connection failed
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.ProxyConnection.Failed")
                });
            }
        }

        /// <summary>
        /// Prepare system warning models
        /// </summary>
        /// <returns>List of system warning models</returns>
        public virtual IList<SystemWarningModel> PrepareSystemWarningModels()
        {
            var models = new List<SystemWarningModel>();

            //store URL
            PrepareStoreUrlWarningModel(models);

            //removal key
            PrepareRemovalKeyWarningModel(models);

            //primary exchange rate currency
            PrepareExchangeRateCurrencyWarningModel(models);

            //primary store currency
            PreparePrimaryStoreCurrencyWarningModel(models);

            //base measure weight
            PrepareBaseWeightWarningModel(models);

            //base dimension weight
            PrepareBaseDimensionWarningModel(models);

            //payment methods
            PreparePaymentMethodsWarningModel(models);

            //incompatible plugins
            PreparePluginsWarningModel(models);

            //performance settings
            PreparePerformanceSettingsWarningModel(models);

            //validate write permissions (the same procedure like during installation)
            PrepareFilePermissionsWarningModel(models);

            //not active plugins
            PreparePluginsEnabledWarningModel(models);

            //proxy connection
            PrepareProxyConnectionWarningModel(models);

            return models;
        }

        /// <summary>
        /// Prepare maintenance model
        /// </summary>
        /// <param name="model">Maintenance model</param>
        /// <returns>Maintenance model</returns>
        public virtual MaintenanceModel PrepareMaintenanceModel(MaintenanceModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.DeleteGuests.EndDate = DateTime.UtcNow.AddDays(-7);
            model.DeleteGuests.OnlyWithoutShoppingCart = true;
            model.DeleteAbandonedCarts.OlderThan = DateTime.UtcNow.AddDays(-182);

            model.DeleteAlreadySentQueuedEmails.EndDate = DateTime.UtcNow.AddDays(-7);

            model.BackupSupported = _dataProvider.BackupSupported;

            //prepare nested search model
            PrepareBackupFileSearchModel(model.BackupFileSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare paged backup file list model
        /// </summary>
        /// <param name="searchModel">Backup file search model</param>
        /// <returns>Backup file list model</returns>
        public virtual BackupFileListModel PrepareBackupFileListModel(BackupFileSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get backup files
            var backupFiles = _maintenanceService.GetAllBackupFiles().ToPagedList(searchModel);

            //prepare list model
            var model = new BackupFileListModel().PrepareToGrid(searchModel, backupFiles, ()=>
            {
                return backupFiles.Select(file => new BackupFileModel
                {
                    Name = _fileProvider.GetFileName(file),
                    //fill in additional values (not existing in the entity)
                    Length = $"{_fileProvider.FileLength(file) / 1024f / 1024f:F2} Mb",
                    Link = $"{_webHelper.GetStoreLocation(false)}db_backups/{_fileProvider.GetFileName(file)}"
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare URL record search model
        /// </summary>
        /// <param name="searchModel">URL record search model</param>
        /// <returns>URL record search model</returns>
        public virtual UrlRecordSearchModel PrepareUrlRecordSearchModel(UrlRecordSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged URL record list model
        /// </summary>
        /// <param name="searchModel">URL record search model</param>
        /// <returns>URL record list model</returns>
        public virtual UrlRecordListModel PrepareUrlRecordListModel(UrlRecordSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get URL records
            var urlRecords = _urlRecordService.GetAllUrlRecords(slug: searchModel.SeName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //get URL helper
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            //prepare list model
            var model = new UrlRecordListModel().PrepareToGrid(searchModel, urlRecords, () =>
            {
                return urlRecords.Select(urlRecord =>
                {
                    //fill in model values from the entity
                    var urlRecordModel = urlRecord.ToModel<UrlRecordModel>();

                    //fill in additional values (not existing in the entity)
                    urlRecordModel.Name = urlRecord.Slug;
                    urlRecordModel.Language = urlRecord.LanguageId == 0
                        ? _localizationService.GetResource("Admin.System.SeNames.Language.Standard")
                        : _languageService.GetLanguageById(urlRecord.LanguageId)?.Name ?? "Unknown";

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
        /// <returns>Language selector model</returns>
        public virtual LanguageSelectorModel PrepareLanguageSelectorModel()
        {
            var model = new LanguageSelectorModel
            {
                CurrentLanguage = _workContext.WorkingLanguage.ToModel<LanguageModel>(),
                AvailableLanguages = _languageService
                    .GetAllLanguages(storeId: _storeContext.CurrentStore.Id)
                    .Select(language => language.ToModel<LanguageModel>()).ToList()
            };

            return model;
        }

        /// <summary>
        /// Prepare popular search term search model
        /// </summary>
        /// <param name="searchModel">Popular search term search model</param>
        /// <returns>Popular search term search model</returns>
        public virtual PopularSearchTermSearchModel PreparePopularSearchTermSearchModel(PopularSearchTermSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize(5);

            return searchModel;
        }

        /// <summary>
        /// Prepare paged popular search term list model
        /// </summary>
        /// <param name="searchModel">Popular search term search model</param>
        /// <returns>Popular search term list model</returns>
        public virtual PopularSearchTermListModel PreparePopularSearchTermListModel(PopularSearchTermSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get popular search terms
            var searchTermRecordLines = _searchTermService.GetStats(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

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
        /// <returns>Common statistics model</returns>
        public virtual CommonStatisticsModel PrepareCommonStatisticsModel()
        {
            var model = new CommonStatisticsModel
            {
                NumberOfOrders = _orderService.SearchOrders(pageIndex: 0, pageSize: 1, getOnlyTotalCount: true).TotalCount
            };

            var customerRoleIds = new[] { _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.RegisteredRoleName).Id };
            model.NumberOfCustomers = _customerService.GetAllCustomers(customerRoleIds: customerRoleIds,
                pageIndex: 0, pageSize: 1, getOnlyTotalCount: true).TotalCount;

            var returnRequestStatus = ReturnRequestStatus.Pending;
            model.NumberOfPendingReturnRequests = _returnRequestService.SearchReturnRequests(rs: returnRequestStatus,
                pageIndex: 0, pageSize: 1, getOnlyTotalCount: true).TotalCount;

            model.NumberOfLowStockProducts =
                _productService.GetLowStockProducts(getOnlyTotalCount: true).TotalCount +
                _productService.GetLowStockProductCombinations(getOnlyTotalCount: true).TotalCount;

            return model;
        }

        #endregion
    }
}