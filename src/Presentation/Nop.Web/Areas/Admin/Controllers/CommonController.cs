using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CommonController : BaseAdminController
    {
        #region Const

        private const string EXPORT_IMPORT_PATH = @"files\exportimport";

        #endregion

        #region Fields

        private readonly ICommonModelFactory _commonModelFactory;
        private readonly ICustomerService _customerService;
        private readonly INopDataProvider _dataProvider;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly INopFileProvider _fileProvider;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CommonController(ICommonModelFactory commonModelFactory,
            ICustomerService customerService,
            INopDataProvider dataProvider,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMaintenanceService maintenanceService,
            INopFileProvider fileProvider,
            INotificationService notificationService,
            IPermissionService permissionService,
            IQueuedEmailService queuedEmailService,
            IShoppingCartService shoppingCartService,
            IStaticCacheManager staticCacheManager,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            _commonModelFactory = commonModelFactory;
            _customerService = customerService;
            _dataProvider = dataProvider;
            _dateTimeHelper = dateTimeHelper;
            _languageService = languageService;
            _localizationService = localizationService;
            _maintenanceService = maintenanceService;
            _fileProvider = fileProvider;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _queuedEmailService = queuedEmailService;
            _shoppingCartService = shoppingCartService;
            _staticCacheManager = staticCacheManager;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> SystemInfo()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //prepare model
            var model = await _commonModelFactory.PrepareSystemInfoModelAsync(new SystemInfoModel());

            return View(model);
        }

        public virtual async Task<IActionResult> Warnings()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //prepare model
            var model = await _commonModelFactory.PrepareSystemWarningModelsAsync();

            return View(model);
        }

        public virtual async Task<IActionResult> Maintenance()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //prepare model
            var model = await _commonModelFactory.PrepareMaintenanceModelAsync(new MaintenanceModel());

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-guests")]
        public virtual async Task<IActionResult> MaintenanceDeleteGuests(MaintenanceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var startDateValue = model.DeleteGuests.StartDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteGuests.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

            var endDateValue = model.DeleteGuests.EndDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteGuests.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            model.DeleteGuests.NumberOfDeletedCustomers = await _customerService.DeleteGuestCustomersAsync(startDateValue, endDateValue, model.DeleteGuests.OnlyWithoutShoppingCart);

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-abondoned-carts")]
        public virtual async Task<IActionResult> MaintenanceDeleteAbandonedCarts(MaintenanceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var olderThanDateValue = _dateTimeHelper.ConvertToUtcTime(model.DeleteAbandonedCarts.OlderThan, await _dateTimeHelper.GetCurrentTimeZoneAsync());

            model.DeleteAbandonedCarts.NumberOfDeletedItems = await _shoppingCartService.DeleteExpiredShoppingCartItemsAsync(olderThanDateValue);
            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-exported-files")]
        public virtual async Task<IActionResult> MaintenanceDeleteFiles(MaintenanceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var startDateValue = model.DeleteExportedFiles.StartDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteExportedFiles.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

            var endDateValue = model.DeleteExportedFiles.EndDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteExportedFiles.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            model.DeleteExportedFiles.NumberOfDeletedFiles = 0;

            foreach (var fullPath in _fileProvider.GetFiles(_fileProvider.GetAbsolutePath(EXPORT_IMPORT_PATH)))
            {
                try
                {
                    var fileName = _fileProvider.GetFileName(fullPath);
                    if (fileName.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    var info = _fileProvider.GetFileInfo(fullPath);
                    var lastModifiedTimeUtc = info.LastModified.UtcDateTime;
                    if ((!startDateValue.HasValue || startDateValue.Value < lastModifiedTimeUtc) &&
                        (!endDateValue.HasValue || lastModifiedTimeUtc < endDateValue.Value))
                    {
                        _fileProvider.DeleteFile(fullPath);
                        model.DeleteExportedFiles.NumberOfDeletedFiles++;
                    }
                }
                catch (Exception exc)
                {
                    await _notificationService.ErrorNotificationAsync(exc);
                }
            }

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-minification-files")]
        public virtual async Task<IActionResult> MaintenanceDeleteMinificationFiles(MaintenanceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            model.DeleteMinificationFiles.NumberOfDeletedFiles = 0;

            foreach (var fullPath in _fileProvider.GetFiles(_fileProvider.GetAbsolutePath("bundles")))
            {
                try
                {
                    var info = _fileProvider.GetFileInfo(fullPath);

                    if (info.Name.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    _fileProvider.DeleteFile(info.PhysicalPath);
                    model.DeleteMinificationFiles.NumberOfDeletedFiles++;

                }
                catch (Exception exc)
                {
                    await _notificationService.ErrorNotificationAsync(exc);
                }
            }

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> BackupFiles(BackupFileSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _commonModelFactory.PrepareBackupFileListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("backup-database")]
        public virtual async Task<IActionResult> BackupDatabase(MaintenanceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            try
            {
                await _dataProvider.BackupDatabaseAsync(_maintenanceService.CreateNewBackupFilePath());
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.Maintenance.BackupDatabase.BackupCreated"));
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            //prepare model
            model = await _commonModelFactory.PrepareMaintenanceModelAsync(new MaintenanceModel());

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("re-index")]
        public virtual async Task<IActionResult> ReIndexTables(MaintenanceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            try
            {
                await _dataProvider.ReIndexTablesAsync();
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.Maintenance.ReIndexTables.Complete"));
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("backupFileName", "action")]
        public virtual async Task<IActionResult> BackupAction(MaintenanceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var action = Request.Form["action"];

            var fileName = Request.Form["backupFileName"];
            fileName = _fileProvider.GetFileName(_fileProvider.GetAbsolutePath(fileName));

            var backupPath = _maintenanceService.GetBackupPath(fileName);

            try
            {
                switch (action)
                {
                    case "delete-backup":
                        {
                            _fileProvider.DeleteFile(backupPath);
                            _notificationService.SuccessNotification(string.Format(await _localizationService.GetResourceAsync("Admin.System.Maintenance.BackupDatabase.BackupDeleted"), fileName));
                        }
                        break;
                    case "restore-backup":
                        {
                            await _dataProvider.RestoreDatabaseAsync(backupPath);
                            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.Maintenance.BackupDatabase.DatabaseRestored"));
                        }
                        break;
                }
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            //prepare model
            model = await _commonModelFactory.PrepareMaintenanceModelAsync(model);

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-already-sent-queued-emails")]
        public virtual async Task<IActionResult> MaintenanceDeleteAlreadySentQueuedEmails(MaintenanceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var startDateValue = model.DeleteAlreadySentQueuedEmails.StartDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteAlreadySentQueuedEmails.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

            var endDateValue = model.DeleteAlreadySentQueuedEmails.EndDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteAlreadySentQueuedEmails.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            model.DeleteAlreadySentQueuedEmails.NumberOfDeletedEmails = await _queuedEmailService.DeleteAlreadySentEmailsAsync(startDateValue, endDateValue);

            return View(model);
        }

        public virtual async Task<IActionResult> SetLanguage(int langid, string returnUrl = "")
        {
            var language = await _languageService.GetLanguageByIdAsync(langid);
            if (language != null)
                await _workContext.SetWorkingLanguageAsync(language);

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home", new { area = AreaNames.Admin });

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });

            return Redirect(returnUrl);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ClearCache(string returnUrl = "")
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            await _staticCacheManager.ClearAsync();

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });

            return Redirect(returnUrl);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RestartApplication(string returnUrl = "")
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home", new { area = AreaNames.Admin });

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.Action("Index", "Home", new { area = AreaNames.Admin });

            return View("RestartApplication", returnUrl);
        }

        public virtual async Task<IActionResult> RestartApplication()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance) &&
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins) &&
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            {
                return AccessDeniedView();
            }

            //restart application
            _webHelper.RestartAppDomain();

            return new EmptyResult();
        }

        public virtual async Task<IActionResult> SeNames()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //prepare model
            var model = await _commonModelFactory.PrepareUrlRecordSearchModelAsync(new UrlRecordSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SeNames(UrlRecordSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _commonModelFactory.PrepareUrlRecordListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelectedSeNames(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            await _urlRecordService.DeleteUrlRecordsAsync(await _urlRecordService.GetUrlRecordsByIdsAsync(selectedIds.ToArray()));

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> PopularSearchTermsReport(PopularSearchTermSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _commonModelFactory.PreparePopularSearchTermListModelAsync(searchModel);

            return Json(model);
        }

        //action displaying notification (warning) to a store owner that entered SE URL already exists
        public virtual async Task<IActionResult> UrlReservedWarning(string entityId, string entityName, string seName)
        {
            if (string.IsNullOrEmpty(seName))
                return Json(new { Result = string.Empty });

            _ = int.TryParse(entityId, out var parsedEntityId);
            var validatedSeName = await _urlRecordService.ValidateSeNameAsync(parsedEntityId, entityName, seName, null, false);

            if (seName.Equals(validatedSeName, StringComparison.InvariantCultureIgnoreCase))
                return Json(new { Result = string.Empty });

            return Json(new { Result = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.URL.Reserved"), validatedSeName) });
        }

        #endregion
    }
}