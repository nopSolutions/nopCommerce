using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Http.Extensions;
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
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CommonController : BaseAdminController
{
    #region Const

    protected const string EXPORT_IMPORT_PATH = @"files\exportimport";

    #endregion

    #region Fields

    protected readonly ICommonModelFactory _commonModelFactory;
    protected readonly ICustomerService _customerService;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMaintenanceService _maintenanceService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IQueuedEmailService _queuedEmailService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;

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

    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> SystemInfo()
    {
        //prepare model
        var model = await _commonModelFactory.PrepareSystemInfoModelAsync(new SystemInfoModel());

        return View(model);
    }

    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> Warnings()
    {
        //prepare model
        var model = await _commonModelFactory.PrepareSystemWarningModelsAsync();

        return View(model);
    }

    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> Maintenance()
    {
        //prepare model
        var model = await _commonModelFactory.PrepareMaintenanceModelAsync(new MaintenanceModel());

        return View(model);
    }

    [HttpPost, ActionName("Maintenance")]
    [FormValueRequired("delete-guests")]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> MaintenanceDeleteGuests(MaintenanceModel model)
    {
        var startDateValue = model.DeleteGuests.StartDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteGuests.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

        var endDateValue = model.DeleteGuests.EndDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteGuests.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        model.DeleteGuests.NumberOfDeletedCustomers = await _customerService.DeleteGuestCustomersAsync(startDateValue, endDateValue, model.DeleteGuests.OnlyWithoutShoppingCart);

        return View(model);
    }

    [HttpPost, ActionName("Maintenance")]
    [FormValueRequired("delete-abondoned-carts")]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> MaintenanceDeleteAbandonedCarts(MaintenanceModel model)
    {
        var olderThanDateValue = _dateTimeHelper.ConvertToUtcTime(model.DeleteAbandonedCarts.OlderThan, await _dateTimeHelper.GetCurrentTimeZoneAsync());

        model.DeleteAbandonedCarts.NumberOfDeletedItems = await _shoppingCartService.DeleteExpiredShoppingCartItemsAsync(olderThanDateValue);
        return View(model);
    }

    [HttpPost, ActionName("Maintenance")]
    [FormValueRequired("delete-exported-files")]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> MaintenanceDeleteFiles(MaintenanceModel model)
    {
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
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> MaintenanceDeleteMinificationFiles(MaintenanceModel model)
    {
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
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> BackupFiles(BackupFileSearchModel searchModel)
    {
        //prepare model
        var model = await _commonModelFactory.PrepareBackupFileListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost, ActionName("Maintenance")]
    [FormValueRequired("backup-database")]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> BackupDatabase(MaintenanceModel model)
    {
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
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ReIndexTables(MaintenanceModel model)
    {
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
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> BackupAction(MaintenanceModel model)
    {
        var action = await Request.GetFormValueAsync("action");

        var fileName = await Request.GetFormValueAsync("backupFileName");
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
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> MaintenanceDeleteAlreadySentQueuedEmails(MaintenanceModel model)
    {
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
            returnUrl = Url.Action("Index", "Home", new { area = AreaNames.ADMIN });

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            return RedirectToAction("Index", "Home", new { area = AreaNames.ADMIN });

        return Redirect(returnUrl);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ClearCache(string returnUrl = "")
    {
        await _staticCacheManager.ClearAsync();

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            return RedirectToAction("Index", "Home", new { area = AreaNames.ADMIN });

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            return RedirectToAction("Index", "Home", new { area = AreaNames.ADMIN });

        return Redirect(returnUrl);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual IActionResult RestartApplication(string returnUrl = "")
    {
        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.Action("Index", "Home", new { area = AreaNames.ADMIN });

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = Url.Action("Index", "Home", new { area = AreaNames.ADMIN });

        return View("RestartApplication", returnUrl);
    }

    [CheckPermission(new[]
    {
        StandardPermission.Configuration.MANAGE_SETTINGS, 
        StandardPermission.Configuration.MANAGE_PLUGINS,
        StandardPermission.System.MANAGE_MAINTENANCE
    })]
    public virtual IActionResult RestartApplication()
    {
        //restart application
        _webHelper.RestartAppDomain();

        return new EmptyResult();
    }

    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> SeNames()
    {
        //prepare model
        var model = await _commonModelFactory.PrepareUrlRecordSearchModelAsync(new UrlRecordSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> SeNames(UrlRecordSearchModel searchModel)
    {
        //prepare model
        var model = await _commonModelFactory.PrepareUrlRecordListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> DeleteSelectedSeNames(ICollection<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        await _urlRecordService.DeleteUrlRecordsAsync(await _urlRecordService.GetUrlRecordsByIdsAsync(selectedIds.ToArray()));

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> PopularSearchTermsReport(PopularSearchTermSearchModel searchModel)
    {
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