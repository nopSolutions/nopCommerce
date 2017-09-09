using System.Collections.Generic;
using PayPal.Api;

namespace Nop.Plugin.Payments.PayPalDirect
{
    /// <summary>
    /// Represents PayPal helper
    /// </summary>
    public class PaypalHelper
    {
        #region Constants

        /// <summary>
        /// nopCommerce partner code
        /// </summary>
        private const string BN_CODE = "nopCommerce_SP";

        #endregion

        #region Methods

        /// <summary>
        /// Get PayPal Api context 
        /// </summary>
        /// <param name="paypalDirectPaymentSettings">PayPalDirectPayment settings</param>
        /// <returns>ApiContext</returns>
        public static APIContext GetApiContext(PayPalDirectPaymentSettings payPalDirectPaymentSettings)
        {
            var mode = payPalDirectPaymentSettings.UseSandbox ? "sandbox" : "live";

            var config = new Dictionary<string, string>
            {
                { "clientId", payPalDirectPaymentSettings.ClientId },
                { "clientSecret", payPalDirectPaymentSettings.ClientSecret },
                { "mode", mode }
            };

            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken) { Config = config };

            if (apiContext.HTTPHeaders == null)
                apiContext.HTTPHeaders = new Dictionary<string, string>();
            apiContext.HTTPHeaders["PayPal-Partner-Attribution-Id"] = BN_CODE;

            return apiContext;
        }

        #endregion
    }
}

