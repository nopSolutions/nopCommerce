using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Helpers;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CommonController : BaseAdminController
    {
        #region Fields

        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ISearchTermService _searchTermService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public CommonController(IShoppingCartService shoppingCartService,
            ICustomerService customerService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            IWorkContext workContext,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ISearchTermService searchTermService,
            IMaintenanceService maintenanceService,
            IHostingEnvironment hostingEnvironment,
            IStaticCacheManager cacheManager)
        {
            this._shoppingCartService = shoppingCartService;
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._webHelper = webHelper;
            this._dateTimeHelper = dateTimeHelper;
            this._languageService = languageService;
            this._workContext = workContext;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
            this._searchTermService = searchTermService;
            this._maintenanceService = maintenanceService;
            this._hostingEnvironment = hostingEnvironment;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        protected virtual bool IsDebugAssembly(Assembly assembly)
        {
            var attribs = assembly.GetCustomAttributes(typeof(System.Diagnostics.DebuggableAttribute), false);

            if (attribs.Length > 0)
            {
                var attr = attribs[0] as System.Diagnostics.DebuggableAttribute;
                if (attr != null)
                {
                    return attr.IsJITOptimizerDisabled;
                }
            }

            return false;
        }

        #endregion

        #region Methods

        public virtual IActionResult SystemInfo()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new SystemInfoModel
            {
                NopVersion = NopVersion.CurrentVersion
            };
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
            model.HttpHost = HttpContext.Request.Headers[HeaderNames.Host];

            foreach (var header in HttpContext.Request.Headers)
            {
                model.Headers.Add(new SystemInfoModel.HeaderModel
                {
                    Name = header.Key,
                    Value = header.Value
                });
            }
            
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var loadedAssembly = new SystemInfoModel.LoadedAssembly
                {
                    FullName = assembly.FullName,
                   
                };
                //ensure no exception is thrown
                try
                {
                    loadedAssembly.Location = assembly.IsDynamic ? null : assembly.Location;
                    loadedAssembly.IsDebug = IsDebugAssembly(assembly);

                    //https://stackoverflow.com/questions/2050396/getting-the-date-of-a-net-assembly
                    //we use a simple method because the more Jeff Atwood's solution doesn't work anymore (more info at https://blog.codinghorror.com/determining-build-date-the-hard-way/)
                    loadedAssembly.BuildDate = assembly.IsDynamic ? null : (DateTime?)TimeZoneInfo.ConvertTimeFromUtc(System.IO.File.GetLastWriteTimeUtc(assembly.Location), TimeZoneInfo.Local);
                }
                catch (Exception) { }
                model.LoadedAssemblies.Add(loadedAssembly);
            }

            return View(model);
        }

        public virtual IActionResult Warnings()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();
            var model = WarningsHelper.GetWarnings();
            return View(model);
        }
        
        public virtual IActionResult Maintenance()
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
        public virtual IActionResult MaintenanceDeleteGuests(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var startDateValue = (model.DeleteGuests.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteGuests.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.DeleteGuests.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteGuests.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            model.DeleteGuests.NumberOfDeletedCustomers = _customerService.DeleteGuestCustomers(startDateValue, endDateValue, model.DeleteGuests.OnlyWithoutShoppingCart);

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-abondoned-carts")]
        public virtual IActionResult MaintenanceDeleteAbandonedCarts(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var olderThanDateValue = _dateTimeHelper.ConvertToUtcTime(model.DeleteAbandonedCarts.OlderThan, _dateTimeHelper.CurrentTimeZone);

            model.DeleteAbandonedCarts.NumberOfDeletedItems = _shoppingCartService.DeleteExpiredShoppingCartItems(olderThanDateValue);
            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-exported-files")]
        public virtual IActionResult MaintenanceDeleteFiles(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var startDateValue = (model.DeleteExportedFiles.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteExportedFiles.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.DeleteExportedFiles.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteExportedFiles.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);


            model.DeleteExportedFiles.NumberOfDeletedFiles = 0;
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "files\\exportimport");
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
        public virtual IActionResult BackupFiles(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedKendoGridJson();

            var backupFiles = _maintenanceService.GetAllBackupFiles().ToList();
            
            var gridModel = new DataSourceResult
            {
                Data = backupFiles.Select(p => new {p.Name,
                    Length = $"{p.Length / 1024f / 1024f:F2} Mb",
                    Link = _webHelper.GetStoreLocation(false) + "db_backups/" + p.Name
                }),
                Total = backupFiles.Count
            };
            return Json(gridModel);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("backup-database")]
        public virtual IActionResult BackupDatabase(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            try
            {
                _maintenanceService.BackupDatabase();
                SuccessNotification(_localizationService.GetResource("Admin.System.Maintenance.BackupDatabase.BackupCreated"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("backupFileName", "action")]
        public virtual IActionResult BackupAction(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var action = Request.Form["action"];

            var fileName = Request.Form["backupFileName"];
            var backupPath = _maintenanceService.GetBackupPath(fileName);

            try
            {
                switch (action)
                {
                    case "delete-backup":
                    {
                        System.IO.File.Delete(backupPath);
                        SuccessNotification(string.Format(_localizationService.GetResource("Admin.System.Maintenance.BackupDatabase.BackupDeleted"), fileName));
                    }
                        break;
                    case "restore-backup":
                    {
                        _maintenanceService.RestoreDatabase(backupPath);
                        SuccessNotification(_localizationService.GetResource("Admin.System.Maintenance.BackupDatabase.DatabaseRestored"));
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
        
        public virtual IActionResult SetLanguage(int langid, string returnUrl = "")
        {
            var language = _languageService.GetLanguageById(langid);
            if (language != null)
            {
                _workContext.WorkingLanguage = language;
            }

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home", new { area = AreaNames.Admin });
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });
            return Redirect(returnUrl);
        }

        [HttpPost]
        public virtual IActionResult ClearCache(string returnUrl = "")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();
            
            _cacheManager.Clear();

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });
            return Redirect(returnUrl);
        }

        [HttpPost]
        public virtual IActionResult RestartApplication(string returnUrl = "")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //restart application
            _webHelper.RestartAppDomain();

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });
            return Redirect(returnUrl);
        }

        public virtual IActionResult SeNames()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new UrlRecordListModel();
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult SeNames(DataSourceRequest command, UrlRecordListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedKendoGridJson();

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
                    var detailsUrl = "";
                    var entityName = x.EntityName?.ToLowerInvariant() ?? "";
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
        public virtual IActionResult DeleteSelectedSeNames(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                _urlRecordService.DeleteUrlRecords(_urlRecordService.GetUrlRecordsByIds(selectedIds.ToArray()));
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult PopularSearchTermsReport(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

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
        
        //action displaying notification (warning) to a store owner that entered SE URL already exists
        public virtual IActionResult UrlReservedWarning(string entityId, string entityName, string seName)
        {
            if (string.IsNullOrEmpty(seName))
                return Json(new { Result = string.Empty });

            int.TryParse(entityId, out int parsedEntityId);
            var validatedSeName = SeoExtensions.ValidateSeName(parsedEntityId, entityName, seName, null, false);

            if (seName.Equals(validatedSeName, StringComparison.InvariantCultureIgnoreCase))
                return Json(new { Result = string.Empty });

            return Json(new { Result = string.Format(_localizationService.GetResource("Admin.System.Warnings.URL.Reserved"), validatedSeName) });
        }
        
        #endregion
    }
}