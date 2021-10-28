using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class EmailAccountController : BaseAdminController
    {
        #region Fields

        protected EmailAccountSettings EmailAccountSettings { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected IEmailAccountModelFactory EmailAccountModelFactory { get; }
        protected IEmailAccountService EmailAccountService { get; }
        protected IEmailSender EmailSender { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected ISettingService SettingService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IWorkContext WorkContext { get; }
        protected IStoreContext StoreContext { get; }

        #endregion

        #region Ctor

        public EmailAccountController(EmailAccountSettings emailAccountSettings,
            ICustomerActivityService customerActivityService,
            IEmailAccountModelFactory emailAccountModelFactory,
            IEmailAccountService emailAccountService,
            IEmailSender emailSender,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            IStoreContext storeContext)
        {
            EmailAccountSettings = emailAccountSettings;
            CustomerActivityService = customerActivityService;
            EmailAccountModelFactory = emailAccountModelFactory;
            EmailAccountService = emailAccountService;
            EmailSender = emailSender;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            SettingService = settingService;
            GenericAttributeService = genericAttributeService;
            WorkContext = workContext;
            StoreContext = storeContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List(bool showtour = false)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            //prepare model
            var model = await EmailAccountModelFactory.PrepareEmailAccountSearchModelAsync(new EmailAccountSearchModel());

            //show configuration tour
            if (showtour)
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                var hideCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
                var closeCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(EmailAccountSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await EmailAccountModelFactory.PrepareEmailAccountListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> MarkAsDefaultEmail(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var defaultEmailAccount = await EmailAccountService.GetEmailAccountByIdAsync(id);
            if (defaultEmailAccount == null)
                return RedirectToAction("List");

            EmailAccountSettings.DefaultEmailAccountId = defaultEmailAccount.Id;
            await SettingService.SaveSettingAsync(EmailAccountSettings);

            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            //prepare model
            var model = await EmailAccountModelFactory.PrepareEmailAccountModelAsync(new EmailAccountModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(EmailAccountModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var emailAccount = model.ToEntity<EmailAccount>();

                //set password manually
                emailAccount.Password = model.Password;
                await EmailAccountService.InsertEmailAccountAsync(emailAccount);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewEmailAccount",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewEmailAccount"), emailAccount.Id), emailAccount);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = emailAccount.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await EmailAccountModelFactory.PrepareEmailAccountModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id, bool showtour = false)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            //try to get an email account with the specified id
            var emailAccount = await EmailAccountService.GetEmailAccountByIdAsync(id);
            if (emailAccount == null)
                return RedirectToAction("List");

            //prepare model
            var model = await EmailAccountModelFactory.PrepareEmailAccountModelAsync(null, emailAccount);

            //show configuration tour
            if (showtour)
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                var hideCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
                var closeCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(EmailAccountModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            //try to get an email account with the specified id
            var emailAccount = await EmailAccountService.GetEmailAccountByIdAsync(model.Id);
            if (emailAccount == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                emailAccount = model.ToEntity(emailAccount);
                await EmailAccountService.UpdateEmailAccountAsync(emailAccount);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditEmailAccount",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditEmailAccount"), emailAccount.Id), emailAccount);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = emailAccount.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await EmailAccountModelFactory.PrepareEmailAccountModelAsync(model, emailAccount, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public virtual async Task<IActionResult> ChangePassword(EmailAccountModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            //try to get an email account with the specified id
            var emailAccount = await EmailAccountService.GetEmailAccountByIdAsync(model.Id);
            if (emailAccount == null)
                return RedirectToAction("List");

            //do not validate model
            emailAccount.Password = model.Password;
            await EmailAccountService.UpdateEmailAccountAsync(emailAccount);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Fields.Password.PasswordChanged"));

            return RedirectToAction("Edit", new { id = emailAccount.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("sendtestemail")]
        public virtual async Task<IActionResult> SendTestEmail(EmailAccountModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            //try to get an email account with the specified id
            var emailAccount = await EmailAccountService.GetEmailAccountByIdAsync(model.Id);
            if (emailAccount == null)
                return RedirectToAction("List");

            if (!CommonHelper.IsValidEmail(model.SendTestEmailTo))
            {
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Common.WrongEmail"));
                return View(model);
            }

            try
            {
                if (string.IsNullOrWhiteSpace(model.SendTestEmailTo))
                    throw new NopException("Enter test email address");
                var store = await StoreContext.GetCurrentStoreAsync();
                var subject = store.Name + ". Testing email functionality.";
                var body = "Email works fine.";
                await EmailSender.SendEmailAsync(emailAccount, subject, body, emailAccount.Email, emailAccount.DisplayName, model.SendTestEmailTo, null);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.SendTestEmail.Success"));
            }
            catch (Exception exc)
            {
                NotificationService.ErrorNotification(exc.Message);
            }

            //prepare model
            model = await EmailAccountModelFactory.PrepareEmailAccountModelAsync(model, emailAccount, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            //try to get an email account with the specified id
            var emailAccount = await EmailAccountService.GetEmailAccountByIdAsync(id);
            if (emailAccount == null)
                return RedirectToAction("List");

            try
            {
                await EmailAccountService.DeleteEmailAccountAsync(emailAccount);

                //activity log
                await CustomerActivityService.InsertActivityAsync("DeleteEmailAccount",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteEmailAccount"), emailAccount.Id), emailAccount);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("Edit", new { id = emailAccount.Id });
            }
        }

        #endregion
    }
}