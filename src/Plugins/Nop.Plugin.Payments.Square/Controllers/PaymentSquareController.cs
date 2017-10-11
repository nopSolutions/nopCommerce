using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Tasks;
using Nop.Plugin.Payments.Square.Domain;
using Nop.Plugin.Payments.Square.Models;
using Nop.Plugin.Payments.Square.Services;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Tasks;
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
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly SquarePaymentManager _squarePaymentManager;
        private readonly SquarePaymentSettings _squarePaymentSettings;

        #endregion

        #region Ctor

        public PaymentSquareController(ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IScheduleTaskService scheduleTaskService,
            SquarePaymentManager squarePaymentManager,
            SquarePaymentSettings squarePaymentSettings)
        {
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._scheduleTaskService = scheduleTaskService;
            this._squarePaymentManager = squarePaymentManager;
            this._squarePaymentSettings = squarePaymentSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether this plugin is configured
        /// </summary>
        /// <returns>True if it is configured; otherwise false</returns>
        private bool IsConfigured()
        {
            return !string.IsNullOrEmpty(_squarePaymentSettings.ApplicationId)
                && !string.IsNullOrEmpty(_squarePaymentSettings.ApplicationSecret);
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
                IsConfigured = IsConfigured(),
                ApplicationId = _squarePaymentSettings.ApplicationId,
                ApplicationSecret = _squarePaymentSettings.ApplicationSecret,
                AccessToken = _squarePaymentSettings.AccessToken,
                AccessTokenRenewalPeriod = _squarePaymentSettings.AccessTokenRenewalPeriod,
                TransactionModeId = (int)_squarePaymentSettings.TransactionMode,
                TransactionModes = _squarePaymentSettings.TransactionMode.ToSelectList(),
                LocationId = _squarePaymentSettings.LocationId,
                AdditionalFee = _squarePaymentSettings.AdditionalFee,
                AdditionalFeePercentage = _squarePaymentSettings.AdditionalFeePercentage
            };

            //prepare business locations, every payment a merchant processes is associated with one of these locations
            if (!string.IsNullOrEmpty(model.AccessToken))
            {
                model.Locations = _squarePaymentManager.GetActiveLocations()
                    .Select(location => new SelectListItem { Text = location.BusinessName, Value = location.Id }).ToList();
            }

            //add the special item for 'there are no location' with value 0
            if (!model.Locations.Any())
            {
                var noLocationText = _localizationService.GetResource("Plugins.Payments.Square.Fields.Location.NotExist");
                model.Locations.Add(new SelectListItem { Text = noLocationText, Value = "0" });
            }

            //get access token renewal period in days
            var task = _scheduleTaskService.GetTaskByType(SquarePaymentDefaults.RenewAccessTokenTask);
            if (task != null)
                model.AccessTokenRenewalPeriod = task.Seconds / 60 / 60 / 24;

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
            _squarePaymentSettings.AccessTokenRenewalPeriod = model.AccessTokenRenewalPeriod;
            _squarePaymentSettings.TransactionMode = (TransactionMode)model.TransactionModeId;
            _squarePaymentSettings.LocationId = model.LocationId;
            _squarePaymentSettings.AdditionalFee = model.AdditionalFee;
            _squarePaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            _settingService.SaveSetting(_squarePaymentSettings);

            //get task parameters
            var enableTask = model.AccessTokenRenewalPeriod > 0;
            var taskPeriodInSeconds = model.AccessTokenRenewalPeriod * 24 * 60 * 60;

            //update renew access token task, if task parameters was changed
            var task = _scheduleTaskService.GetTaskByType(SquarePaymentDefaults.RenewAccessTokenTask);
            if (task != null)
            {
                if (task.Enabled != enableTask || task.Seconds != taskPeriodInSeconds)
                {
                    task.Enabled = enableTask;
                    task.Seconds = taskPeriodInSeconds;
                    _scheduleTaskService.UpdateTask(task);

                    SuccessNotification(_localizationService.GetResource("Plugins.Payments.Square.TaskChanged"));
                }
            }
            else
            {
                //or create the new one if not exists
                _scheduleTaskService.InsertTask(new ScheduleTask
                {
                    Enabled = enableTask,
                    Seconds = taskPeriodInSeconds,
                    Name = SquarePaymentDefaults.RenewAccessTokenTaskName,
                    Type = SquarePaymentDefaults.RenewAccessTokenTask,
                });

                SuccessNotification(_localizationService.GetResource("Plugins.Payments.Square.TaskChanged"));
            }

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

            //get the URL to directs a Square merchant's web browser
            var redirectUrl = _squarePaymentManager.GenerateAuthorizeUrl();

            return Redirect(redirectUrl);
        }

        public IActionResult AccessTokenCallback()
        {
            //handle access token callback
            try
            {
                if (!IsConfigured())
                    throw new NopException("Plugin is not configured");

                //check whether there are errors in the request
                if (this.Request.Query.TryGetValue("error", out StringValues error) |
                    this.Request.Query.TryGetValue("error_description", out StringValues errorDescription))
                {
                    throw new NopException($"{error} - {errorDescription}");
                }

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
                ErrorNotification(exception.Message);
            }

            return RedirectToAction("Configure", "PaymentSquare", new { area = AreaNames.Admin });
        }

        #endregion
    }
}