using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Nop.Core;

namespace Nop.Plugin.Payments.PayPalStandard.Services
{
    /// <summary>
    /// Represents the HTTP client to request PayPal services
    /// </summary>
    public partial class PayPalStandardHttpClient
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly PayPalStandardPaymentSettings _payPalStandardPaymentSettings;

        #endregion

        #region Ctor

        public PayPalStandardHttpClient(HttpClient client,
            PayPalStandardPaymentSettings payPalStandardPaymentSettings)
        {
            //configure client
            client.Timeout = TimeSpan.FromSeconds(20);
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"nopCommerce-{NopVersion.CURRENT_VERSION}");

            _httpClient = client;
            _payPalStandardPaymentSettings = payPalStandardPaymentSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets PDT details
        /// </summary>
        /// <param name="tx">TX</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the asynchronous task whose result contains the PDT details
        /// </returns>
        public async Task<string> GetPdtDetailsAsync(string tx)
        {
            //get response
            var url = _payPalStandardPaymentSettings.UseSandbox ?
                "https://www.sandbox.paypal.com/us/cgi-bin/webscr" :
                "https://www.paypal.com/us/cgi-bin/webscr";
            var requestContent = new StringContent($"cmd=_notify-synch&at={_payPalStandardPaymentSettings.PdtToken}&tx={tx}",
                Encoding.UTF8, MimeTypes.ApplicationXWwwFormUrlencoded);
            var response = await _httpClient.PostAsync(url, requestContent);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Verifies IPN
        /// </summary>
        /// <param name="formString">Form string</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the asynchronous task whose result contains the IPN verification details
        /// </returns>
        public async Task<string> VerifyIpnAsync(string formString)
        {
            //get response
            var url = _payPalStandardPaymentSettings.UseSandbox ?
                "https://ipnpb.sandbox.paypal.com/cgi-bin/webscr" :
                "https://ipnpb.paypal.com/cgi-bin/webscr";
            var requestContent = new StringContent($"cmd=_notify-validate&{formString}",
                Encoding.UTF8, MimeTypes.ApplicationXWwwFormUrlencoded);
            var response = await _httpClient.PostAsync(url, requestContent);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        #endregion
    }
}