using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Plugin.Payments.Square.Domain;
using Nop.Plugin.Payments.Square.Models;
using Nop.Plugin.Payments.Square.Services;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.Square.Controllers
{
    public class PaymentSquareController : BasePaymentController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly SquarePaymentManager _squarePaymentManager;
        private readonly SquarePaymentSettings _squarePaymentSettings;

        #endregion

        #region Ctor

        public PaymentSquareController(ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            SquarePaymentManager squarePaymentManager,
            SquarePaymentSettings squarePaymentSettings)
        {
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._squarePaymentManager = squarePaymentManager;
            this._squarePaymentSettings = squarePaymentSettings;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            //whether user has the authority
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prepare model
            var model = new ConfigurationModel
            {
                ApplicationId = _squarePaymentSettings.ApplicationId,
                ApplicationSecret = _squarePaymentSettings.ApplicationSecret,
                AccessToken = _squarePaymentSettings.AccessToken,
                UseSandbox = _squarePaymentSettings.UseSandbox,
                TransactionModeId = (int)_squarePaymentSettings.TransactionMode,
                TransactionModes = _squarePaymentSettings.TransactionMode.ToSelectList(),
                LocationId = _squarePaymentSettings.LocationId,
                AdditionalFee = _squarePaymentSettings.AdditionalFee,
                AdditionalFeePercentage = _squarePaymentSettings.AdditionalFeePercentage
            };

            //prepare business locations, every payment a merchant processes is associated with one of these locations
            if (!string.IsNullOrEmpty(model.AccessToken))
            {
                model.Locations = _squarePaymentManager.GetActiveLocations().Select(location =>
                {
                    var name = location.BusinessName;
                    if (!location.Name.Equals(location.BusinessName))
                        name = $"{name} ({location.Name})";
                    return new SelectListItem { Text = name, Value = location.Id };
                }).ToList();
            }

            //add the special item for 'there are no location' with value 0
            if (!model.Locations.Any())
            {
                var noLocationText = _localizationService.GetResource("Plugins.Payments.Square.Fields.Location.NotExist");
                model.Locations.Add(new SelectListItem { Text = noLocationText, Value = "0" });
            }

            return View("~/Plugins/Payments.Square/Views/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            //whether user has the authority
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _squarePaymentSettings.ApplicationId = model.ApplicationId;
            _squarePaymentSettings.ApplicationSecret = model.ApplicationSecret;
            _squarePaymentSettings.AccessToken = model.AccessToken;
            _squarePaymentSettings.UseSandbox = model.UseSandbox;
            _squarePaymentSettings.TransactionMode = (TransactionMode)model.TransactionModeId;
            _squarePaymentSettings.LocationId = model.LocationId;
            _squarePaymentSettings.AdditionalFee = model.AdditionalFee;
            _squarePaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            _settingService.SaveSetting(_squarePaymentSettings);

            //warn admin that the location is a required parameter
            if (string.IsNullOrEmpty(_squarePaymentSettings.LocationId) || _squarePaymentSettings.LocationId.Equals("0"))
                WarningNotification(_localizationService.GetResource("Plugins.Payments.Square.Fields.Location.Hint"));
            else
                SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("obtainAccessToken")]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult ObtainAccessToken(ConfigurationModel model)
        {
            //whether user has the authority
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //create new verification string
            _squarePaymentSettings.AccessTokenVerificationString = Guid.NewGuid().ToString();
            _settingService.SaveSetting(_squarePaymentSettings);

            //get the URL to directs a Square merchant's web browser
            var redirectUrl = _squarePaymentManager.GenerateAuthorizeUrl(_squarePaymentSettings.AccessTokenVerificationString);

            return Redirect(redirectUrl);
        }

        public IActionResult AccessTokenCallback()
        {
            //handle access token callback
            try
            {
                if (string.IsNullOrEmpty(_squarePaymentSettings.ApplicationId) || string.IsNullOrEmpty(_squarePaymentSettings.ApplicationSecret))
                    throw new NopException("Plugin is not configured");

                //check whether there are errors in the request
                if (this.Request.Query.TryGetValue("error", out StringValues error) |
                    this.Request.Query.TryGetValue("error_description", out StringValues errorDescription))
                {
                    throw new NopException($"{error} - {errorDescription}");
                }

                //validate verification string
                if (!this.Request.Query.TryGetValue("state", out StringValues verificationString) || !verificationString.Equals(_squarePaymentSettings.AccessTokenVerificationString))
                    throw new NopException("The verification string did not pass the validation");

                //check whether there is an authorization code in the request
                if (!this.Request.Query.TryGetValue("code", out StringValues authorizationCode))
                    throw new NopException("No service response");

                //exchange the authorization code for an access token
                var accessToken = _squarePaymentManager.ObtainAccessToken(new ObtainAccessTokenRequest
                {
                    ApplicationId = _squarePaymentSettings.ApplicationId,
                    ApplicationSecret = _squarePaymentSettings.ApplicationSecret,
                    AuthorizationCode = authorizationCode,
                    RedirectUrl = this.Url.RouteUrl(SquarePaymentDefaults.AccessTokenRoute)
                });
                if (string.IsNullOrEmpty(accessToken))
                    throw new NopException("No service response");

                //if access token successfully received, save it for the further usage
                _squarePaymentSettings.AccessToken = accessToken;
                _settingService.SaveSetting(_squarePaymentSettings);

                SuccessNotification(_localizationService.GetResource("Plugins.Payments.Square.ObtainAccessToken.Success"));
            }
            catch (Exception exception)
            {
                //display errors
                ErrorNotification(_localizationService.GetResource("Plugins.Payments.Square.ObtainAccessToken.Error"));
                if (!string.IsNullOrEmpty(exception.Message))
                    ErrorNotification(exception.Message);
            }

            return RedirectToAction("Configure", "PaymentSquare", new { area = AreaNames.Admin });
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("revokeAccessTokens")]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult RevokeAccessTokens(ConfigurationModel model)
        {
            //whether user has the authority
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            try
            {
                //try to revoke all access tokens
                var successfullyRevoked = _squarePaymentManager.RevokeAccessTokens(new RevokeAccessTokenRequest
                {
                    ApplicationId = _squarePaymentSettings.ApplicationId,
                    ApplicationSecret = _squarePaymentSettings.ApplicationSecret,
                    AccessToken = _squarePaymentSettings.AccessToken
                });
                if (!successfullyRevoked)
                    throw new NopException("Tokens were not revoked");

                //if access token successfully revoked, delete it from the settings
                _squarePaymentSettings.AccessToken = string.Empty;
                _settingService.SaveSetting(_squarePaymentSettings);

                SuccessNotification(_localizationService.GetResource("Plugins.Payments.Square.RevokeAccessTokens.Success"));
            }
            catch (Exception exception)
            {
                ErrorNotification(_localizationService.GetResource("Plugins.Payments.Square.RevokeAccessTokens.Error"));
                if (!string.IsNullOrEmpty(exception.Message))
                    ErrorNotification(exception.Message);
            }

            return Configure();
        }

        #endregion
    }
}