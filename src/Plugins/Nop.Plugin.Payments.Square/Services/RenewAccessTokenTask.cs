using System;
using Nop.Core;
using Nop.Plugin.Payments.Square.Domain;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;
using Nop.Services.Tasks;

namespace Nop.Plugin.Payments.Square.Services
{
    /// <summary>
    /// Represents a schedule task to renew the access token
    /// </summary>
    public class RenewAccessTokenTask : IScheduleTask
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IPaymentService _paymentService;
        private readonly ISettingService _settingService;
        private readonly SquarePaymentManager _squarePaymentManager;
        private readonly SquarePaymentSettings _squarePaymentSettings;

        #endregion

        #region Ctor

        public RenewAccessTokenTask(ILocalizationService localizationService,
            ILogger logger,
            IPaymentService paymentService,
            ISettingService settingService,
            SquarePaymentManager squarePaymentManager,
            SquarePaymentSettings squarePaymentSettings)
        {
            this._localizationService = localizationService;
            this._logger = logger;
            this._paymentService = paymentService;
            this._settingService = settingService;
            this._squarePaymentManager = squarePaymentManager;
            this._squarePaymentSettings = squarePaymentSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            //whether plugin is active
            if (!_paymentService.IsPaymentMethodActive(_paymentService.LoadPaymentMethodBySystemName(SquarePaymentDefaults.SystemName)))
                return;

            //do not execute for sandbox environment
            if (_squarePaymentSettings.UseSandbox)
                return;

            try
            {
                //get the new access token
                var newAccessToken = _squarePaymentManager.RenewAccessToken(new RenewAccessTokenRequest
                {
                    ApplicationId = _squarePaymentSettings.ApplicationId,
                    ApplicationSecret = _squarePaymentSettings.ApplicationSecret,
                    ExpiredAccessToken = _squarePaymentSettings.AccessToken
                });
                if (string.IsNullOrEmpty(newAccessToken))
                    throw new NopException("No service response");

                //if access token successfully received, save it for the further usage
                _squarePaymentSettings.AccessToken = newAccessToken;
                _settingService.SaveSetting(_squarePaymentSettings);

                //log information about the successful renew of the access token
                _logger.Information(_localizationService.GetResource("Plugins.Payments.Square.RenewAccessToken.Success"));
            }
            catch (Exception exception)
            {
                //log error on renewing of the access token
                _logger.Error(_localizationService.GetResource("Plugins.Payments.Square.RenewAccessToken.Error"), exception);
            }
        }

        #endregion
    }
}