using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.ExternalAuth.Facebook.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Plugin.ExternalAuth.Facebook.Controllers
{
    public class FacebookAuthenticationController : BasePluginController
    {
        #region Fields

        private readonly FacebookExternalAuthSettings _facebookExternalAuthSettings;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public FacebookAuthenticationController(FacebookExternalAuthSettings facebookExternalAuthSettings,
            IExternalAuthenticationService externalAuthenticationService,
            ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext)
        {
            this._facebookExternalAuthSettings = facebookExternalAuthSettings;
            this._externalAuthenticationService = externalAuthenticationService;
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area("Admin")]
        public IActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<FacebookExternalAuthSettings>(storeScope);

            var model = new ConfigurationModel
            {
                ClientId = settings.ClientKeyIdentifier,
                ClientSecret = settings.ClientSecret,
                ActiveStoreScopeConfiguration = storeScope
            };
            if (storeScope > 0)
            {
                model.ClientId_OverrideForStore = _settingService.SettingExists(settings, setting => setting.ClientKeyIdentifier, storeScope);
                model.ClientSecret_OverrideForStore = _settingService.SettingExists(settings, setting => setting.ClientSecret, storeScope);
            }

            return View("~/Plugins/ExternalAuth.Facebook/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        [AuthorizeAdmin]
        [Area("Admin")]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<FacebookExternalAuthSettings>(storeScope);

            //save settings
            settings.ClientKeyIdentifier = model.ClientId;
            settings.ClientSecret = model.ClientSecret;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.ClientKeyIdentifier, model.ClientId_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.ClientSecret, model.ClientSecret_OverrideForStore, storeScope, false);
           
            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        public IActionResult Login(string returnUrl)
        {
            if (!_externalAuthenticationService.ExternalAuthenticationMethodIsAvailable(FacebookAuthenticationDefaults.ProviderSystemName))
                throw new NopException("Facebook authentication module cannot be loaded");

            if (string.IsNullOrEmpty(_facebookExternalAuthSettings.ClientKeyIdentifier) || string.IsNullOrEmpty(_facebookExternalAuthSettings.ClientSecret))
                throw new NopException("Facebook authentication module not configured");

            //configure login callback action
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginCallback", "FacebookAuthentication", new { returnUrl = returnUrl })
            };

            return Challenge(authenticationProperties, FacebookDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            //authenticate Facebook user
            var authenticateContext = new AuthenticateContext(FacebookDefaults.AuthenticationScheme);
            await this.HttpContext?.Authentication?.AuthenticateAsync(authenticateContext);

            //get authenticated user identity
            var userIdentity = authenticateContext.Principal?.Identities?.FirstOrDefault(identity => identity.IsAuthenticated);
            if (userIdentity == null || !userIdentity.Claims.Any())
                return RedirectToRoute("Login");

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = FacebookAuthenticationDefaults.ProviderSystemName,
                AccessToken = new AuthenticationProperties(authenticateContext.Properties).GetTokenValue("access_token"),
                Email = userIdentity.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value,
                ExternalIdentifier = userIdentity.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                ExternalDisplayIdentifier = userIdentity.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
                Claims = userIdentity.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
            };

            //authenticate Nop user
            return _externalAuthenticationService.Authenticate(authenticationParameters, returnUrl);
        }

        #endregion
    }
}