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

        protected ICommonModelFactory CommonModelFactory { get; }
        protected ICustomerService CustomerService { get; }
        protected INopDataProvider DataProvider { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected ILanguageService LanguageService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IMaintenanceService MaintenanceService { get; }
        protected INopFileProvider FileProvider { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IQueuedEmailService QueuedEmailService { get; }
        protected IShoppingCartService ShoppingCartService { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }

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
            CommonModelFactory = commonModelFactory;
            CustomerService = customerService;
            DataProvider = dataProvider;
            DateTimeHelper = dateTimeHelper;
            LanguageService = languageService;
            LocalizationService = localizationService;
            MaintenanceService = maintenanceService;
            FileProvider = fileProvider;
            NotificationService = notificationService;
            PermissionService = permissionService;
            QueuedEmailService = queuedEmailService;
            ShoppingCartService = shoppingCartService;
            StaticCacheManager = staticCacheManager;
            UrlRecordService = urlRecordService;
            WebHelper = webHelper;
            WorkContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> SystemInfo()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //prepare model
            var model = await CommonModelFactory.PrepareSystemInfoModelAsync(new SystemInfoModel());

            return View(model);
        }

        public virtual async Task<IActionResult> Warnings()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //prepare model
            var model = await CommonModelFactory.PrepareSystemWarningModelsAsync();

            return View(model);
        }

        public virtual async Task<IActionResult> Maintenance()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //prepare model
            var model = await CommonModelFactory.PrepareMaintenanceModelAsync(new MaintenanceModel());

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-guests")]
        public virtual async Task<IActionResult> MaintenanceDeleteGuests(MaintenanceModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var startDateValue = model.DeleteGuests.StartDate == null ? null
                            : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.DeleteGuests.StartDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync());

            var endDateValue = model.DeleteGuests.EndDate == null ? null
                            : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.DeleteGuests.EndDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            model.DeleteGuests.NumberOfDeletedCustomers = await CustomerService.DeleteGuestCustomersAsync(startDateValue, endDateValue, model.DeleteGuests.OnlyWithoutShoppingCart);

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-abondoned-carts")]
        public virtual async Task<IActionResult> MaintenanceDeleteAbandonedCarts(MaintenanceModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var olderThanDateValue = DateTimeHelper.ConvertToUtcTime(model.DeleteAbandonedCarts.OlderThan, await DateTimeHelper.GetCurrentTimeZoneAsync());

            model.DeleteAbandonedCarts.NumberOfDeletedItems = await ShoppingCartService.DeleteExpiredShoppingCartItemsAsync(olderThanDateValue);
            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-exported-files")]
        public virtual async Task<IActionResult> MaintenanceDeleteFiles(MaintenanceModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var startDateValue = model.DeleteExportedFiles.StartDate == null ? null
                            : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.DeleteExportedFiles.StartDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync());

            var endDateValue = model.DeleteExportedFiles.EndDate == null ? null
                            : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.DeleteExportedFiles.EndDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            model.DeleteExportedFiles.NumberOfDeletedFiles = 0;

            foreach (var fullPath in FileProvider.GetFiles(FileProvider.GetAbsolutePath(EXPORT_IMPORT_PATH)))
            {
                try
                {
                    var fileName = FileProvider.GetFileName(fullPath);
                    if (fileName.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    var info = FileProvider.GetFileInfo(fullPath);
                    var lastModifiedTimeUtc = info.LastModified.UtcDateTime;
                    if ((!startDateValue.HasValue || startDateValue.Value < lastModifiedTimeUtc) &&
                        (!endDateValue.HasValue || lastModifiedTimeUtc < endDateValue.Value))
                    {
                        FileProvider.DeleteFile(fullPath);
                        model.DeleteExportedFiles.NumberOfDeletedFiles++;
                    }
                }
                catch (Exception exc)
                {
                    await NotificationService.ErrorNotificationAsync(exc);
                }
            }

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> BackupFiles(BackupFileSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await CommonModelFactory.PrepareBackupFileListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("backup-database")]
        public virtual async Task<IActionResult> BackupDatabase(MaintenanceModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            try
            {
                await DataProvider.BackupDatabaseAsync(MaintenanceService.CreateNewBackupFilePath());
                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.System.Maintenance.BackupDatabase.BackupCreated"));
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
            }

            //prepare model
            model = await CommonModelFactory.PrepareMaintenanceModelAsync(new MaintenanceModel());

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("re-index")]
        public virtual async Task<IActionResult> ReIndexTables(MaintenanceModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            try
            {
                await DataProvider.ReIndexTablesAsync();
                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.System.Maintenance.ReIndexTables.Complete"));
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
            }

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("backupFileName", "action")]
        public virtual async Task<IActionResult> BackupAction(MaintenanceModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var action = Request.Form["action"];

            var fileName = Request.Form["backupFileName"];
            var backupPath = MaintenanceService.GetBackupPath(fileName);

            try
            {
                switch (action)
                {
                    case "delete-backup":
                        {
                            FileProvider.DeleteFile(backupPath);
                            NotificationService.SuccessNotification(string.Format(await LocalizationService.GetResourceAsync("Admin.System.Maintenance.BackupDatabase.BackupDeleted"), fileName));
                        }
                        break;
                    case "restore-backup":
                        {
                            await DataProvider.RestoreDatabaseAsync(backupPath);
                            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.System.Maintenance.BackupDatabase.DatabaseRestored"));
                        }
                        break;
                }
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
            }

            //prepare model
            model = await CommonModelFactory.PrepareMaintenanceModelAsync(model);

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-already-sent-queued-emails")]
        public virtual async Task<IActionResult> MaintenanceDeleteAlreadySentQueuedEmails(MaintenanceModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var startDateValue = model.DeleteAlreadySentQueuedEmails.StartDate == null ? null
                            : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.DeleteAlreadySentQueuedEmails.StartDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync());

            var endDateValue = model.DeleteAlreadySentQueuedEmails.EndDate == null ? null
                            : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.DeleteAlreadySentQueuedEmails.EndDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            model.DeleteAlreadySentQueuedEmails.NumberOfDeletedEmails = await QueuedEmailService.DeleteAlreadySentEmailsAsync(startDateValue, endDateValue);

            return View(model);
        }

        public virtual async Task<IActionResult> SetLanguage(int langid, string returnUrl = "")
        {
            var language = await LanguageService.GetLanguageByIdAsync(langid);
            if (language != null)
                await WorkContext.SetWorkingLanguageAsync(language);

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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            await StaticCacheManager.ClearAsync();

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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance) &&
                !await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins) &&
                !await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
            {
                return AccessDeniedView();
            }

            //restart application
            WebHelper.RestartAppDomain();

            return new EmptyResult();
        }

        public virtual async Task<IActionResult> SeNames()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //prepare model
            var model = await CommonModelFactory.PrepareUrlRecordSearchModelAsync(new UrlRecordSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SeNames(UrlRecordSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await CommonModelFactory.PrepareUrlRecordListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelectedSeNames(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            await UrlRecordService.DeleteUrlRecordsAsync(await UrlRecordService.GetUrlRecordsByIdsAsync(selectedIds.ToArray()));

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> PopularSearchTermsReport(PopularSearchTermSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await CommonModelFactory.PreparePopularSearchTermListModelAsync(searchModel);

            return Json(model);
        }

        //action displaying notification (warning) to a store owner that entered SE URL already exists
        public virtual async Task<IActionResult> UrlReservedWarning(string entityId, string entityName, string seName)
        {
            if (string.IsNullOrEmpty(seName))
                return Json(new { Result = string.Empty });

            _ = int.TryParse(entityId, out var parsedEntityId);
            var validatedSeName = await UrlRecordService.ValidateSeNameAsync(parsedEntityId, entityName, seName, null, false);

            if (seName.Equals(validatedSeName, StringComparison.InvariantCultureIgnoreCase))
                return Json(new { Result = string.Empty });

            return Json(new { Result = string.Format(await LocalizationService.GetResourceAsync("Admin.System.Warnings.URL.Reserved"), validatedSeName) });
        }

        #endregion
    }
}