using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Payments.Square.Domain;
using Nop.Plugin.Payments.Square.Models;
using Nop.Plugin.Payments.Square.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
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
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly SquarePaymentManager _squarePaymentManager;
        private readonly SquarePaymentSettings _squarePaymentSettings;

        #endregion

        #region Ctor

        public PaymentSquareController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            SquarePaymentManager squarePaymentManager,
            SquarePaymentSettings squarePaymentSettings)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _squarePaymentManager = squarePaymentManager;
            _squarePaymentSettings = squarePaymentSettings;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prepare model
            var model = new ConfigurationModel
            {
                ApplicationSecret = _squarePaymentSettings.ApplicationSecret,
                UseSandbox = _squarePaymentSettings.UseSandbox,
                TransactionModeId = (int)_squarePaymentSettings.TransactionMode,
                LocationId = _squarePaymentSettings.LocationId,
                AdditionalFee = _squarePaymentSettings.AdditionalFee,
                AdditionalFeePercentage = _squarePaymentSettings.AdditionalFeePercentage
            };
            if (model.UseSandbox)
            {
                model.SandboxApplicationId = _squarePaymentSettings.ApplicationId;
                model.SandboxAccessToken = _squarePaymentSettings.AccessToken;
            }
            else
            {
                model.ApplicationId = _squarePaymentSettings.ApplicationId;
                model.AccessToken = _squarePaymentSettings.AccessToken;
            }

            //prepare business locations, every payment a merchant processes is associated with one of these locations
            if (!string.IsNullOrEmpty(_squarePaymentSettings.AccessToken))
            {
                model.Locations = _squarePaymentManager.GetActiveLocations().Select(location =>
                {
                    var name = location.BusinessName;
                    if (!location.Name.Equals(location.BusinessName))
                        name = $"{name} ({location.Name})";
                    return new SelectListItem { Text = name, Value = location.Id };
                }).ToList();
                if (model.Locations.Any())
                {
                    var selectLocationText = _localizationService.GetResource("Plugins.Payments.Square.Fields.Location.Select");
                    model.Locations.Insert(0, new SelectListItem { Text = selectLocationText, Value = "0" });
                }
            }

            //add the special item for 'there are no location' with value 0
            if (!model.Locations.Any())
            {
                var noLocationText = _localizationService.GetResource("Plugins.Payments.Square.Fields.Location.NotExist");
                model.Locations.Add(new SelectListItem { Text = noLocationText, Value = "0" });
            }

            //warn admin that the location is a required parameter
            if (string.IsNullOrEmpty(_squarePaymentSettings.LocationId) || _squarePaymentSettings.LocationId.Equals("0"))
                _notificationService.WarningNotification(_localizationService.GetResource("Plugins.Payments.Square.Fields.Location.Hint"));

            //migrate to using refresh tokens
            if (!_squarePaymentSettings.UseSandbox && _squarePaymentSettings.RefreshToken == Guid.Empty.ToString())
            {
                var migrateMessage = $"Your access token is deprecated.<br /> " +
                    $"1. In the <a href=\"http://squ.re/nopcommerce1\" target=\"_blank\">Square Developer Portal</a> make sure your application is on Connect API version 2019-03-13 or later.<br /> " +
                    $"2. On this page click 'Obtain access token' below.<br />";
                _notificationService.ErrorNotification(migrateMessage, encode: false);
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            if (model.UseSandbox)
            {
                _squarePaymentSettings.ApplicationId = model.SandboxApplicationId;
                _squarePaymentSettings.ApplicationSecret = string.Empty;
                _squarePaymentSettings.AccessToken = model.SandboxAccessToken;
            }
            else
            {
                _squarePaymentSettings.ApplicationId = model.ApplicationId;
                _squarePaymentSettings.ApplicationSecret = model.ApplicationSecret;
                if (_squarePaymentSettings.UseSandbox)
                    _squarePaymentSettings.AccessToken = string.Empty;
            }
            _squarePaymentSettings.LocationId = model.UseSandbox == _squarePaymentSettings.UseSandbox ? model.LocationId : string.Empty;
            _squarePaymentSettings.UseSandbox = model.UseSandbox;
            _squarePaymentSettings.TransactionMode = (TransactionMode)model.TransactionModeId;
            _squarePaymentSettings.AdditionalFee = model.AdditionalFee;
            _squarePaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            _settingService.SaveSetting(_squarePaymentSettings);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("obtainAccessToken")]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult ObtainAccessToken(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //create new verification string
            _squarePaymentSettings.AccessTokenVerificationString = Guid.NewGuid().ToString();
            _settingService.SaveSetting(_squarePaymentSettings);

            //get the URL to directs a Square merchant's web browser
            var redirectUrl = _squarePaymentManager.GenerateAuthorizeUrl();

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
                if (Request.Query.TryGetValue("error", out var error) | Request.Query.TryGetValue("error_description", out var errorDescription))
                    throw new NopException($"{error} - {errorDescription}");

                //validate verification string
                if (!Request.Query.TryGetValue("state", out var verificationString) || !verificationString.Equals(_squarePaymentSettings.AccessTokenVerificationString))
                    throw new NopException("The verification string did not pass the validation");

                //check whether there is an authorization code in the request
                if (!Request.Query.TryGetValue("code", out var authorizationCode))
                    throw new NopException("No service response");

                //exchange the authorization code for an access token
                var (accessToken, refreshToken) = _squarePaymentManager.ObtainAccessToken(authorizationCode);
                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                    throw new NopException("No service response");

                //if access token successfully received, save it for the further usage
                _squarePaymentSettings.AccessToken = accessToken;
                _squarePaymentSettings.RefreshToken = refreshToken;
                _settingService.SaveSetting(_squarePaymentSettings);

                _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Payments.Square.ObtainAccessToken.Success"));
            }
            catch (Exception exception)
            {
                //display errors
                _notificationService.ErrorNotification(_localizationService.GetResource("Plugins.Payments.Square.ObtainAccessToken.Error"));
                if (!string.IsNullOrEmpty(exception.Message))
                    _notificationService.ErrorNotification(exception.Message);
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            try
            {
                //try to revoke all access tokens
                var successfullyRevoked = _squarePaymentManager.RevokeAccessTokens();
                if (!successfullyRevoked)
                    throw new NopException("Tokens were not revoked");

                //if access token successfully revoked, delete it from the settings
                _squarePaymentSettings.AccessToken = string.Empty;
                _settingService.SaveSetting(_squarePaymentSettings);

                _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Payments.Square.RevokeAccessTokens.Success"));
            }
            catch (Exception exception)
            {
                var error = _localizationService.GetResource("Plugins.Payments.Square.RevokeAccessTokens.Error");
                if (!string.IsNullOrEmpty(exception.Message))
                    error = $"{error} - {exception.Message}";
                _notificationService.ErrorNotification(exception.Message);
            }

            return Configure();
        }

        #endregion
    }
}