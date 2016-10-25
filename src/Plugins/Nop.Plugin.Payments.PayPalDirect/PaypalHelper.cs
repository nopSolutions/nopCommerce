using System.Collections.Generic;
using System.Net;
using PayPal.Api;

namespace Nop.Plugin.Payments.PayPalDirect
{
    /// <summary>
    /// Represents paypal helper
    /// </summary>
    public class PaypalHelper
    {
        /// <summary>
        /// Get PayPal Api context 
        /// </summary>
        /// <param name="paypalDirectPaymentSettings">PayPalDirectPayment settings</param>
        /// <returns>ApiContext</returns>
        public static APIContext GetApiContext(PayPalDirectPaymentSettings payPalDirectPaymentSettings)
        {
            var mode = !payPalDirectPaymentSettings.UseSandbox ? "live"
                : ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12) ? "security-test-sandbox" : "sandbox";

            var config = new Dictionary<string, string>
            {
                { "clientId", payPalDirectPaymentSettings.ClientId },
                { "clientSecret", payPalDirectPaymentSettings.ClientSecret },
                { "mode", mode }
            };

            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken) { Config = config };

            return apiContext;
        }
    }
}

