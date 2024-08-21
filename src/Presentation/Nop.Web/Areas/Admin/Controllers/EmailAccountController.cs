using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
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

namespace Nop.Web.Areas.Admin.Controllers;

public partial class EmailAccountController : BaseAdminController
{
    #region Fields

    private const char SEPARATOR = '-';

    protected readonly EmailAccountSettings _emailAccountSettings;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IEmailAccountModelFactory _emailAccountModelFactory;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly IEmailSender _emailSender;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public EmailAccountController(EmailAccountSettings emailAccountSettings,
        ICustomerActivityService customerActivityService,
        IEmailAccountModelFactory emailAccountModelFactory,
        IEmailAccountService emailAccountService,
        IEmailSender emailSender,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        INopFileProvider fileProvider,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStoreContext storeContext,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
        _emailAccountSettings = emailAccountSettings;
        _customerActivityService = customerActivityService;
        _emailAccountModelFactory = emailAccountModelFactory;
        _emailAccountService = emailAccountService;
        _emailSender = emailSender;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _fileProvider = fileProvider;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _settingService = settingService;
        _storeContext = storeContext;
        _webHelper = webHelper;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    protected virtual async Task<string> PrepareOAuthUrlAsync(EmailAccount emailAccount)
    {
        if (string.IsNullOrEmpty(emailAccount.ClientSecret) || string.IsNullOrEmpty(emailAccount.ClientId))
        {
            ModelState.AddModelError(nameof(EmailAccountModel.ClientSecret), await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Fields.ClientSecret.Required"));
            return string.Empty;
        }

        var tokenFilePath = _fileProvider.MapPath(NopMessageDefaults.GmailAuthStorePath);
        var credentialRoot = _fileProvider.Combine(tokenFilePath, emailAccount.Email);

        var codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = emailAccount.ClientId,
                ClientSecret = emailAccount.ClientSecret
            },
            Scopes = NopMessageDefaults.GmailScopes,
            DataStore = new FileDataStore(credentialRoot, true),
            UserDefinedQueryParams = new Dictionary<string, string> { ["emailAccountId"] = emailAccount.Id.ToString() }
        });

        var redirectUri = Url.Action(nameof(AuthReturn), null, null, _webHelper.GetCurrentRequestProtocol());
        var authCode = new AuthorizationCodeWebApp(codeFlow, redirectUri, $"{emailAccount.Id}{SEPARATOR}");
        var authResult = await authCode.AuthorizeAsync(emailAccount.Email, CancellationToken.None);

        return authResult.RedirectUri?.ToString();
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> List(bool showtour = false)
    {
        //prepare model
        var model = await _emailAccountModelFactory.PrepareEmailAccountSearchModelAsync(new EmailAccountSearchModel());

        //show configuration tour
        if (showtour)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
            var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

            if (!hideCard && !closeCard)
                ViewBag.ShowTour = true;
        }

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> List(EmailAccountSearchModel searchModel)
    {
        //prepare model
        var model = await _emailAccountModelFactory.PrepareEmailAccountListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> MarkAsDefaultEmail(int id)
    {
        var defaultEmailAccount = await _emailAccountService.GetEmailAccountByIdAsync(id);
        if (defaultEmailAccount == null)
            return RedirectToAction("List");

        _emailAccountSettings.DefaultEmailAccountId = defaultEmailAccount.Id;
        await _settingService.SaveSettingAsync(_emailAccountSettings);

        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(new EmailAccountModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> Create(EmailAccountModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var emailAccount = model.ToEntity<EmailAccount>();

            //set password manually
            emailAccount.Password = model.Password;
            await _emailAccountService.InsertEmailAccountAsync(emailAccount);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewEmailAccount",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewEmailAccount"), emailAccount.Id), emailAccount);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Added"));

            return continueEditing ? RedirectToAction("Edit", new { id = emailAccount.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> Edit(int id, bool showtour = false)
    {
        //try to get an email account with the specified id
        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(id);
        if (emailAccount == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(null, emailAccount);

        if (emailAccount.EmailAuthenticationMethod == EmailAuthenticationMethod.GmailOAuth2)
            model.AuthUrl = await PrepareOAuthUrlAsync(emailAccount);

        //show configuration tour
        if (showtour)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
            var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

            if (!hideCard && !closeCard)
                ViewBag.ShowTour = true;
        }

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> Edit(EmailAccountModel model, bool continueEditing)
    {
        //try to get an email account with the specified id
        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(model.Id);
        if (emailAccount == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            emailAccount = model.ToEntity(emailAccount);
            await _emailAccountService.UpdateEmailAccountAsync(emailAccount);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditEmailAccount",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditEmailAccount"), emailAccount.Id), emailAccount);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Updated"));

            return continueEditing ? RedirectToAction("Edit", new { id = emailAccount.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(model, emailAccount, true);

        if (emailAccount.EmailAuthenticationMethod == EmailAuthenticationMethod.GmailOAuth2)
            model.AuthUrl = await PrepareOAuthUrlAsync(emailAccount);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("changesecret")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> ChangeSecret(EmailAccountModel model)
    {
        //try to get an email account with the specified id
        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(model.Id);
        if (emailAccount == null)
            return RedirectToAction("List");

        //do not validate model
        emailAccount.ClientSecret = model.ClientSecret;
        await _emailAccountService.UpdateEmailAccountAsync(emailAccount);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Fields.ClientSecret.ClientSecretChanged"));

        return RedirectToAction("Edit", new { id = emailAccount.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("changepassword")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> ChangePassword(EmailAccountModel model)
    {
        //try to get an email account with the specified id
        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(model.Id);
        if (emailAccount == null)
            return RedirectToAction("List");

        //do not validate model
        emailAccount.Password = model.Password;
        await _emailAccountService.UpdateEmailAccountAsync(emailAccount);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Fields.Password.PasswordChanged"));

        return RedirectToAction("Edit", new { id = emailAccount.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("sendtestemail")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> SendTestEmail(EmailAccountModel model)
    {
        //try to get an email account with the specified id
        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(model.Id);
        if (emailAccount == null)
            return RedirectToAction("List");

        if (!CommonHelper.IsValidEmail(model.SendTestEmailTo))
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
            return View(await _emailAccountModelFactory.PrepareEmailAccountModelAsync(model, emailAccount, true));
        }

        try
        {
            if (string.IsNullOrWhiteSpace(model.SendTestEmailTo))
                throw new NopException("Enter test email address");
            var store = await _storeContext.GetCurrentStoreAsync();
            var subject = store.Name + ". Testing email functionality.";
            var body = "Email works fine.";
            await _emailSender.SendEmailAsync(emailAccount, subject, body, emailAccount.Email, emailAccount.DisplayName, model.SendTestEmailTo, null);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.SendTestEmail.Success"));

            return RedirectToAction("Edit", new { id = emailAccount.Id });
        }
        catch (Exception exc)
        {
            _notificationService.ErrorNotification(exc.Message);
        }

        //prepare model
        model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(model, emailAccount, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get an email account with the specified id
        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(id);
        if (emailAccount == null)
            return RedirectToAction("List");

        try
        {
            await _emailAccountService.DeleteEmailAccountAsync(emailAccount);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteEmailAccount",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteEmailAccount"), emailAccount.Id), emailAccount);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Deleted"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("Edit", new { id = emailAccount.Id });
        }
    }

    public async Task<IActionResult> AuthReturn(AuthorizationCodeResponseUrl authorizationCode)
    {
        if (string.IsNullOrEmpty(authorizationCode.State))
            return RedirectToAction(nameof(List));

        var strAccountId = authorizationCode.State.Split(SEPARATOR).FirstOrDefault();

        if (!int.TryParse(strAccountId, out var accountId) ||
            await _emailAccountService.GetEmailAccountByIdAsync(accountId) is not EmailAccount emailAccount)
        {
            _notificationService.ErrorNotification("Email account could not be loaded");
            return RedirectToAction(nameof(List));
        }

        if (!string.IsNullOrEmpty(authorizationCode.Error))
        {
            _notificationService.ErrorNotification(authorizationCode.Error);
            return RedirectToAction(nameof(Edit), new { id = emailAccount.Id });
        }

        if (string.IsNullOrEmpty(authorizationCode?.Code))
            return RedirectToAction(nameof(Edit), new { id = emailAccount.Id });

        var tokenFilePath = _fileProvider.MapPath(NopMessageDefaults.GmailAuthStorePath);
        var credentialRoot = _fileProvider.Combine(tokenFilePath, emailAccount.Email);

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = emailAccount.ClientId,
                ClientSecret = emailAccount.ClientSecret
            },
            Scopes = NopMessageDefaults.GmailScopes,
            DataStore = new FileDataStore(credentialRoot, true)
        });

        var redirectUri = Url.Action(nameof(AuthReturn), null, null, _webHelper.GetCurrentRequestProtocol());
        var tokenResponse = await flow.ExchangeCodeForTokenAsync(emailAccount.Email, authorizationCode.Code, redirectUri, CancellationToken.None);

        if (tokenResponse is null || tokenResponse.IsStale)
            return RedirectToAction(nameof(Edit), new { id = emailAccount.Id });

        _notificationService.SuccessNotification("The token was successfully retrieved from the server");

        return RedirectToAction(nameof(Edit), new { id = emailAccount.Id });
    }

    #endregion
}