using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Common;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Security;

namespace Nop.Admin.Controllers
{
    public partial class CommonController : BaseAdminController
    {
        #region Fields

        private readonly IPaymentService _paymentService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICurrencyService _currencyService;
        private readonly IMeasureService _measureService;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly CurrencySettings _currencySettings;
        private readonly MeasureSettings _measureSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ISearchTermService _searchTermService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly CatalogSettings _catalogSettings;
        private readonly HttpContextBase _httpContext;
        private readonly IMaintenanceService _maintenanceService;

        #endregion

        #region Constructors

        public CommonController(IPaymentService paymentService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            ICurrencyService currencyService,
            IMeasureService measureService,
            ICustomerService customerService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            CurrencySettings currencySettings,
            MeasureSettings measureSettings,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ISearchTermService searchTermService,
            ISettingService settingService,
            IStoreService storeService,
            CatalogSettings catalogSettings,
            HttpContextBase httpContext,
            IMaintenanceService maintenanceService)
        {
            this._paymentService = paymentService;
            this._shippingService = shippingService;
            this._shoppingCartService = shoppingCartService;
            this._currencyService = currencyService;
            this._measureService = measureService;
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._webHelper = webHelper;
            this._currencySettings = currencySettings;
            this._measureSettings = measureSettings;
            this._dateTimeHelper = dateTimeHelper;
            this._languageService = languageService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
            this._searchTermService = searchTermService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._catalogSettings = catalogSettings;
            this._httpContext = httpContext;
            this._maintenanceService = maintenanceService;
        }

        #endregion

        #region Methods

        public ActionResult SystemInfo()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new SystemInfoModel();
            model.NopVersion = NopVersion.CurrentVersion;
            try
            {
                model.OperatingSystem = Environment.OSVersion.VersionString;
            }
            catch (Exception) { }
            try
            {
                model.AspNetInfo = RuntimeEnvironment.GetSystemVersion();
            }
            catch (Exception) { }
            try
            {
                model.IsFullTrust = AppDomain.CurrentDomain.IsFullyTrusted.ToString();
            }
            catch (Exception) { }
            model.ServerTimeZone = TimeZone.CurrentTimeZone.StandardName;
            model.ServerLocalTime = DateTime.Now;
            model.UtcTime = DateTime.UtcNow;
            model.CurrentUserTime = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            model.HttpHost = _webHelper.ServerVariables("HTTP_HOST");
            foreach (var key in _httpContext.Request.ServerVariables.AllKeys)
            {
                model.ServerVariables.Add(new SystemInfoModel.ServerVariableModel
                {
                    Name = key,
                    Value = _httpContext.Request.ServerVariables[key]
                });
            }
            //Environment.GetEnvironmentVariable("USERNAME");
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                model.LoadedAssemblies.Add(new SystemInfoModel.LoadedAssembly
                {
                    FullName = assembly.FullName,
                    //we cannot use Location property in medium trust
                    //Location = assembly.Location
                });
            }
            return View(model);
        }

        public ActionResult Warnings()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new List<SystemWarningModel>();

            //store URL
            var currentStoreUrl = _storeContext.CurrentStore.Url;
            if (!String.IsNullOrEmpty(currentStoreUrl) &&
                (currentStoreUrl.Equals(_webHelper.GetStoreLocation(false), StringComparison.InvariantCultureIgnoreCase)
                ||
                currentStoreUrl.Equals(_webHelper.GetStoreLocation(true), StringComparison.InvariantCultureIgnoreCase)
                ))
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.URL.Match")
                });
            else
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = string.Format(_localizationService.GetResource("Admin.System.Warnings.URL.NoMatch"), currentStoreUrl, _webHelper.GetStoreLocation(false))
                });


            //primary exchange rate currency
            var perCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryExchangeRateCurrencyId);
            if (perCurrency != null)
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.ExchangeCurrency.Set"),
                });
                if (perCurrency.Rate != 1)
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Fail,
                        Text = _localizationService.GetResource("Admin.System.Warnings.ExchangeCurrency.Rate1")
                    });
                }
            }
            else
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.ExchangeCurrency.NotSet")
                });
            }

            //primary store currency
            var pscCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (pscCurrency != null)
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.PrimaryCurrency.Set"),
                });
            }
            else
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.PrimaryCurrency.NotSet")
                });
            }


            //base measure weight
            var bWeight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId);
            if (bWeight != null)
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.DefaultWeight.Set"),
                });

                if (bWeight.Ratio != 1)
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Fail,
                        Text = _localizationService.GetResource("Admin.System.Warnings.DefaultWeight.Ratio1")
                    });
                }
            }
            else
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.DefaultWeight.NotSet")
                });
            }


            //base dimension weight
            var bDimension = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId);
            if (bDimension != null)
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.DefaultDimension.Set"),
                });

                if (bDimension.Ratio != 1)
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Fail,
                        Text = _localizationService.GetResource("Admin.System.Warnings.DefaultDimension.Ratio1")
                    });
                }
            }
            else
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.DefaultDimension.NotSet")
                });
            }

            //shipping rate coputation methods
            var srcMethods = _shippingService.LoadActiveShippingRateComputationMethods();
            if (!srcMethods.Any())
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.Shipping.NoComputationMethods")
                });
            if (srcMethods.Count(x => x.ShippingRateComputationMethodType == ShippingRateComputationMethodType.Offline) > 1)
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = _localizationService.GetResource("Admin.System.Warnings.Shipping.OnlyOneOffline")
                });

            //payment methods
            if (_paymentService.LoadActivePaymentMethods().Any())
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.PaymentMethods.OK")
                });
            else
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.PaymentMethods.NoActive")
                });

            //incompatible plugins
            if (PluginManager.IncompatiblePlugins != null)
                foreach (var pluginName in PluginManager.IncompatiblePlugins)
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Warning,
                        Text = string.Format(_localizationService.GetResource("Admin.System.Warnings.IncompatiblePlugin"), pluginName)
                    });

            //performance settings
            if (!_catalogSettings.IgnoreStoreLimitations && _storeService.GetAllStores().Count == 1)
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = _localizationService.GetResource("Admin.System.Warnings.Performance.IgnoreStoreLimitations")
                });
            }
            if (!_catalogSettings.IgnoreAcl)
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = _localizationService.GetResource("Admin.System.Warnings.Performance.IgnoreAcl")
                });
            }

            //validate write permissions (the same procedure like during installation)
            var dirPermissionsOk = true;
            var dirsToCheck = FilePermissionHelper.GetDirectoriesWrite();
            foreach (string dir in dirsToCheck)
                if (!FilePermissionHelper.CheckPermissions(dir, false, true, true, false))
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Warning,
                        Text = string.Format(_localizationService.GetResource("Admin.System.Warnings.DirectoryPermission.Wrong"), WindowsIdentity.GetCurrent().Name, dir)
                    });
                    dirPermissionsOk = false;
                }
            if (dirPermissionsOk)
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.DirectoryPermission.OK")
                });

            var filePermissionsOk = true;
            var filesToCheck = FilePermissionHelper.GetFilesWrite();
            foreach (string file in filesToCheck)
                if (!FilePermissionHelper.CheckPermissions(file, false, true, true, true))
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Warning,
                        Text = string.Format(_localizationService.GetResource("Admin.System.Warnings.FilePermission.Wrong"), WindowsIdentity.GetCurrent().Name, file)
                    });
                    filePermissionsOk = false;
                }
            if (filePermissionsOk)
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.FilePermission.OK")
                });

            //machine key
            try
            {
                var machineKeySection = ConfigurationManager.GetSection("system.web/machineKey") as MachineKeySection;
                var machineKeySpecified = machineKeySection != null &&
                    !String.IsNullOrEmpty(machineKeySection.DecryptionKey) &&
                    !machineKeySection.DecryptionKey.StartsWith("AutoGenerate", StringComparison.InvariantCultureIgnoreCase);

                if (!machineKeySpecified)
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Warning,
                        Text = _localizationService.GetResource("Admin.System.Warnings.MachineKey.NotSpecified")
                    });
                }
                else
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Pass,
                        Text = _localizationService.GetResource("Admin.System.Warnings.MachineKey.Specified")
                    });
                }
            }
            catch (Exception exc)
            {
                LogException(exc);
            }

            return View(model);
        }
        
        public ActionResult Maintenance()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new MaintenanceModel();
            model.DeleteGuests.EndDate = DateTime.UtcNow.AddDays(-7);
            model.DeleteGuests.OnlyWithoutShoppingCart = true;
            model.DeleteAbandonedCarts.OlderThan = DateTime.UtcNow.AddDays(-182);
            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-guests")]
        public ActionResult MaintenanceDeleteGuests(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            DateTime? startDateValue = (model.DeleteGuests.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteGuests.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.DeleteGuests.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteGuests.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            model.DeleteGuests.NumberOfDeletedCustomers = _customerService.DeleteGuestCustomers(startDateValue, endDateValue, model.DeleteGuests.OnlyWithoutShoppingCart);

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-abondoned-carts")]
        public ActionResult MaintenanceDeleteAbandonedCarts(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var olderThanDateValue = _dateTimeHelper.ConvertToUtcTime(model.DeleteAbandonedCarts.OlderThan, _dateTimeHelper.CurrentTimeZone);

            model.DeleteAbandonedCarts.NumberOfDeletedItems = _shoppingCartService.DeleteExpiredShoppingCartItems(olderThanDateValue);
            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-exported-files")]
        public ActionResult MaintenanceDeleteFiles(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            DateTime? startDateValue = (model.DeleteExportedFiles.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteExportedFiles.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.DeleteExportedFiles.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteExportedFiles.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);


            model.DeleteExportedFiles.NumberOfDeletedFiles = 0;
            string path = Path.Combine(this.Request.PhysicalApplicationPath, "content\\files\\exportimport");
            foreach (var fullPath in Directory.GetFiles(path))
            {
                try
                {
                    var fileName = Path.GetFileName(fullPath);
                    if (fileName.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    var info = new FileInfo(fullPath);
                    if ((!startDateValue.HasValue || startDateValue.Value < info.CreationTimeUtc) &&
                        (!endDateValue.HasValue || info.CreationTimeUtc < endDateValue.Value))
                    {
                        System.IO.File.Delete(fullPath);
                        model.DeleteExportedFiles.NumberOfDeletedFiles++;
                    }
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc, false);
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult BackupFiles(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var backupFiles = _maintenanceService.GetAllBackupFiles().ToList();
            
            var gridModel = new DataSourceResult
            {
                Data = backupFiles.Select(p=>new {p.Name,
                    Length = string.Format("{0:F2} Mb", p.Length / 1024f / 1024f),
                    Link = _webHelper.GetStoreLocation(false) + "Administration/db_backups/" + p.Name
                }),
                Total = backupFiles.Count
            };
            return Json(gridModel);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("backup-database")]
        public ActionResult BackupDatabase(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            try
            {
                _maintenanceService.BackupDatabase();
                this.SuccessNotification(_localizationService.GetResource("Admin.System.Maintenance.BackupDatabase.BackupCreated"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("backupFileName", "action")]
        public ActionResult BackupAction(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var action = this.Request.Form["action"];

            var fileName = this.Request.Form["backupFileName"];
            var backupPath = _maintenanceService.GetBackupPath(fileName);

            try
            {
                switch (action)
                {
                    case "delete-backup":
                    {
                        System.IO.File.Delete(backupPath);
                        this.SuccessNotification(string.Format(_localizationService.GetResource("Admin.System.Maintenance.BackupDatabase.BackupDeleted"), fileName));
                    }
                        break;
                    case "restore-backup":
                    {
                        _maintenanceService.RestoreDatabase(backupPath);
                        this.SuccessNotification(_localizationService.GetResource("Admin.System.Maintenance.BackupDatabase.DatabaseRestored"));
                    }
                        break;
                }
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }
            
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            var model = new LanguageSelectorModel();
            model.CurrentLanguage = _workContext.WorkingLanguage.ToModel();
            model.AvailableLanguages = _languageService
                .GetAllLanguages(storeId: _storeContext.CurrentStore.Id)
                .Select(x => x.ToModel())
                .ToList();
            return PartialView(model);
        }
        public ActionResult SetLanguage(int langid, string returnUrl = "")
        {
            var language = _languageService.GetLanguageById(langid);
            if (language != null)
            {
                _workContext.WorkingLanguage = language;
            }

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home", new { area = "Admin" });
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            return Redirect(returnUrl);
        }

        [HttpPost]
        public ActionResult ClearCache(string returnUrl = "")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
            cacheManager.Clear();

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            return Redirect(returnUrl);
        }

        [HttpPost]
        public ActionResult RestartApplication(string returnUrl = "")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //restart application
            _webHelper.RestartAppDomain();

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            return Redirect(returnUrl);
        }


        public ActionResult SeNames()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new UrlRecordListModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult SeNames(DataSourceRequest command, UrlRecordListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var urlRecords = _urlRecordService.GetAllUrlRecords(model.SeName, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = urlRecords.Select(x =>
                {
                    //language
                    string languageName;
                    if (x.LanguageId == 0)
                    {
                        languageName = _localizationService.GetResource("Admin.System.SeNames.Language.Standard");
                    }
                    else
                    {
                        var language = _languageService.GetLanguageById(x.LanguageId);
                        languageName = language != null ? language.Name : "Unknown";
                    }

                    //details URL
                    string detailsUrl = "";
                    var entityName = x.EntityName != null ? x.EntityName.ToLowerInvariant() : "";
                    switch (entityName)
                    {
                        case "blogpost":
                            detailsUrl = Url.Action("Edit", "Blog", new { id = x.EntityId });
                            break;
                        case "category":
                            detailsUrl = Url.Action("Edit", "Category", new { id = x.EntityId });
                            break;
                        case "manufacturer":
                            detailsUrl = Url.Action("Edit", "Manufacturer", new { id = x.EntityId });
                            break;
                        case "product":
                            detailsUrl = Url.Action("Edit", "Product", new { id = x.EntityId });
                            break;
                        case "newsitem":
                            detailsUrl = Url.Action("Edit", "News", new { id = x.EntityId });
                            break;
                        case "topic":
                            detailsUrl = Url.Action("Edit", "Topic", new { id = x.EntityId });
                            break;
                        case "vendor":
                            detailsUrl = Url.Action("Edit", "Vendor", new { id = x.EntityId });
                            break;
                        default:
                            break;
                    }

                    return new UrlRecordModel
                    {
                        Id = x.Id,
                        Name = x.Slug,
                        EntityId = x.EntityId,
                        EntityName = x.EntityName,
                        IsActive = x.IsActive,
                        Language = languageName,
                        DetailsUrl = detailsUrl
                    };
                }),
                Total = urlRecords.TotalCount
            };
            return Json(gridModel);
        }
        [HttpPost]
        public ActionResult DeleteSelectedSeNames(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                _urlRecordService.DeleteUrlRecords(_urlRecordService.GetUrlRecordsByIds(selectedIds.ToArray()));
            }

            return Json(new { Result = true });
        }


        [ChildActionOnly]
        public ActionResult PopularSearchTermsReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return Content("");

            return PartialView();
        }
        [HttpPost]
        public ActionResult PopularSearchTermsReport(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var searchTermRecordLines = _searchTermService.GetStats(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = searchTermRecordLines.Select(x => new SearchTermReportLineModel
                {
                    Keyword = x.Keyword,
                    Count = x.Count,
                }),
                Total = searchTermRecordLines.TotalCount
            };
            return Json(gridModel);
        }


        //action displaying notification (warning) to a store owner that "limit per store" feature is ignored
        [ChildActionOnly]
        public ActionResult MultistoreDisabledWarning()
        {
            //default setting
            bool enabled = _catalogSettings.IgnoreStoreLimitations;
            if (!enabled)
            {
                //overridden settings
                var stores = _storeService.GetAllStores();
                foreach (var store in stores)
                {
                    if (!enabled)
                    {
                        var catalogSettings = _settingService.LoadSetting<CatalogSettings>(store.Id);
                        enabled = catalogSettings.IgnoreStoreLimitations;
                    }
                }
            }

            //This setting is disabled. No warnings.
            if (!enabled)
                return Content("");

            return PartialView();
        }
        //action displaying notification (warning) to a store owner that "ACL rules" feature is ignored
        [ChildActionOnly]
        public ActionResult AclDisabledWarning()
        {
            //default setting
            bool enabled = _catalogSettings.IgnoreAcl;
            if (!enabled)
            {
                //overridden settings
                var stores = _storeService.GetAllStores();
                foreach (var store in stores)
                {
                    if (!enabled)
                    {
                        var catalogSettings = _settingService.LoadSetting<CatalogSettings>(store.Id);
                        enabled = catalogSettings.IgnoreAcl;
                    }
                }
            }

            //This setting is disabled. No warnings.
            if (!enabled)
                return Content("");

            return PartialView();
        }

        #endregion
    }
}
