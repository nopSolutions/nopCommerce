using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.ExternalAuth.Twitter.Core;
using Nop.Plugin.ExternalAuth.Twitter.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.ExternalAuth.Twitter.Controllers
{
    public class ExternalAuthTwitterController : Controller
    {
        private readonly ISettingService _settingService;
        private readonly TwitterExternalAuthSettings _twitterExternalAuthSettings;
        private readonly IOAuthProviderTwitterAuthorizer _oAuthProviderTwitterAuthorizer;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;

        public ExternalAuthTwitterController(ISettingService settingService,
            TwitterExternalAuthSettings twitterExternalAuthSettings,
            IOAuthProviderTwitterAuthorizer oAuthProviderTwitterAuthorizer,
            IOpenAuthenticationService openAuthenticationService,
            ExternalAuthenticationSettings externalAuthenticationSettings)
        {
            this._settingService = settingService;
            this._twitterExternalAuthSettings = twitterExternalAuthSettings;
            this._oAuthProviderTwitterAuthorizer = oAuthProviderTwitterAuthorizer;
            this._openAuthenticationService = openAuthenticationService;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.ConsumerKey = _twitterExternalAuthSettings.ConsumerKey;
            model.ConsumerSecret = _twitterExternalAuthSettings.ConsumerSecret;
            
            return View("Nop.Plugin.ExternalAuth.Twitter.Views.ExternalAuthTwitter.Configure", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();
            
            //save settings
            _twitterExternalAuthSettings.ConsumerKey = model.ConsumerKey;
            _twitterExternalAuthSettings.ConsumerSecret = model.ConsumerSecret;
            _settingService.SaveSetting(_twitterExternalAuthSettings);
            
            return View("Nop.Plugin.ExternalAuth.Twitter.Views.ExternalAuthTwitter.Configure", model);
        }

        [ChildActionOnly]
        public ActionResult PublicInfo()
        {
            return View("Nop.Plugin.ExternalAuth.Twitter.Views.ExternalAuthTwitter.PublicInfo");
        }


        public ActionResult Login(string returnUrl)
        {
            var processor = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName("ExternalAuth.Twitter");
            if (processor == null ||
                !processor.IsMethodActive(_externalAuthenticationSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("Twitter module cannot be loaded");

            var viewModel = new LoginModel();
            TryUpdateModel(viewModel);

            var result = _oAuthProviderTwitterAuthorizer.Authorize(returnUrl);
            switch (result.AuthenticationStatus)
            {
                case OpenAuthenticationStatus.Error:
                    {
                        if (!result.Success)
                            foreach (var error in result.Errors)
                                ExternalAuthorizerHelper.AddErrorsToDisplay(error);

                        return new RedirectResult(Url.LogOn(returnUrl));
                    }
                case OpenAuthenticationStatus.AssociateOnLogon:
                    {
                        return new RedirectResult(Url.LogOn(returnUrl));
                    }
                case OpenAuthenticationStatus.AutoRegisteredEmailValidation:
                    {
                        //result
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                    }
                case OpenAuthenticationStatus.AutoRegisteredAdminApproval:
                    {
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                    }
                case OpenAuthenticationStatus.AutoRegisteredStandard:
                    {
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                    }
                default:
                    break;
            }

            if (result.Result != null) return result.Result;
            return HttpContext.Request.IsAuthenticated ? new RedirectResult(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "~/") : new RedirectResult(Url.LogOn(returnUrl));
        }
    }
}