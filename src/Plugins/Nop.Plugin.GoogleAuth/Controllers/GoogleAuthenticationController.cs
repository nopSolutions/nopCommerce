using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Plugin.GoogleAuth.Models;
using Nop.Plugin.GoogleAuth;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Nop.Plugin.GoogleAuth.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class GoogleAuthenticationController : BasePluginController
    {
        #region Fields

        private readonly GoogleExternalAuthSettings _googleExternalAuthSettings;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IOptionsMonitorCache<GoogleOptions> _optionsCache;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public GoogleAuthenticationController(GoogleExternalAuthSettings googleExternalAuthSettings,
            IAuthenticationPluginManager authenticationPluginManager,
            IExternalAuthenticationService externalAuthenticationService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IOptionsMonitorCache<GoogleOptions> optionsCache,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _googleExternalAuthSettings = googleExternalAuthSettings;
            _authenticationPluginManager = authenticationPluginManager;
            _externalAuthenticationService = externalAuthenticationService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _optionsCache = optionsCache;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                ClientId = _googleExternalAuthSettings.ClientKeyIdentifier,
                ClientSecret = _googleExternalAuthSettings.ClientSecret
            };

            return View("~/Plugins/GoogleAuth/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //save settings
            _googleExternalAuthSettings.ClientKeyIdentifier = model.ClientId;
            _googleExternalAuthSettings.ClientSecret = model.ClientSecret;
            await _settingService.SaveSettingAsync(_googleExternalAuthSettings);

            //clear Facebook authentication options cache
            _optionsCache.TryRemove(GoogleDefaults.AuthenticationScheme);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        public async Task<IActionResult> Login(string returnUrl)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var methodIsAvailable = await _authenticationPluginManager
                .IsPluginActiveAsync(GoogleAuthenticationDefaults.SystemName, await _workContext.GetCurrentCustomerAsync(), store.Id);
            if (!methodIsAvailable)
                throw new NopException("Google authentication module cannot be loaded");

            if (string.IsNullOrEmpty(_googleExternalAuthSettings.ClientKeyIdentifier) ||
                string.IsNullOrEmpty(_googleExternalAuthSettings.ClientSecret))
            {
                throw new NopException("Google authentication module not configured");
            }

            //configure login callback action
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginCallback", "GoogleAuthentication", new { returnUrl = returnUrl })
            };
            authenticationProperties.SetString(GoogleAuthenticationDefaults.ErrorCallback, Url.RouteUrl("Login", new { returnUrl }));

            return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            //authenticate Facebook user
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded || !authenticateResult.Principal.Claims.Any())
                return RedirectToRoute("Login");

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = GoogleDefaults.DisplayName,
                AccessToken = await HttpContext.GetTokenAsync(GoogleDefaults.AuthenticationScheme, "access_token"),
                Email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value,
                ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
                Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
            };

            //authenticate Nop user
            return await _externalAuthenticationService.AuthenticateAsync(authenticationParameters, returnUrl);
        }

        public async Task<IActionResult> DataDeletionStatusCheck(int earId)
        {
            var externalAuthenticationRecord = await _externalAuthenticationService.GetExternalAuthenticationRecordByIdAsync(earId);
            if (externalAuthenticationRecord is not null)
                _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Plugins.GoogleAuth.AuthenticationDataExist"));
            else
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.GoogleAuth.AuthenticationDataDeletedSuccessfully"));

            return RedirectToRoute("CustomerInfo");
        }

        #endregion
    }
}