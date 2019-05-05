using System;
using Nop.Core;
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
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly ISettingService _settingService;
        private readonly SquarePaymentManager _squarePaymentManager;
        private readonly SquarePaymentSettings _squarePaymentSettings;

        #endregion

        #region Ctor

        public RenewAccessTokenTask(ILocalizationService localizationService,
            ILogger logger,
            IPaymentPluginManager paymentPluginManager,
            ISettingService settingService,
            SquarePaymentManager squarePaymentManager,
            SquarePaymentSettings squarePaymentSettings)
        {
            _localizationService = localizationService;
            _logger = logger;
            _paymentPluginManager = paymentPluginManager;
            _settingService = settingService;
            _squarePaymentManager = squarePaymentManager;
            _squarePaymentSettings = squarePaymentSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            //whether plugin is active
            if (!_paymentPluginManager.IsPluginActive(SquarePaymentDefaults.SystemName))
                return;

            //do not execute for sandbox environment
            if (_squarePaymentSettings.UseSandbox)
                return;

            try
            {
                //get the new access token
                var (newAccessToken, refreshToken) = _squarePaymentManager.RenewAccessToken();
                if (string.IsNullOrEmpty(newAccessToken) || string.IsNullOrEmpty(refreshToken))
                    throw new NopException("No service response");

                //if access token successfully received, save it for the further usage
                _squarePaymentSettings.AccessToken = newAccessToken;
                _squarePaymentSettings.RefreshToken = refreshToken;
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