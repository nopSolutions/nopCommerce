using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Misc.Brevo.MarketingAutomation;

namespace Nop.Plugin.Misc.Brevo.Services
{
    /// <summary>
    /// Represents HTTP client to request Brevo marketing automation services
    /// </summary>
    public class MarketingAutomationHttpClient
    {
        #region Fields

        protected readonly HttpClient _httpClient;

        #endregion

        #region Ctor

        public MarketingAutomationHttpClient(HttpClient httpClient,
            BrevoSettings brevoSettings)
        {
            //configure client
            httpClient.BaseAddress = new Uri(BrevoDefaults.MarketingAutomationUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(10);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, BrevoDefaults.UserAgent);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

            //authentication
            httpClient.DefaultRequestHeaders.Add(BrevoDefaults.MarketingAutomationKeyHeader, brevoSettings.MarketingAutomationKey);

            _httpClient = httpClient;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Request API service
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <param name="request">Request</param>
        /// <returns>The asynchronous task whose result contains response details</returns>
        public async Task RequestAsync<TRequest>(TRequest request) where TRequest : Request
        {
            try
            {
                var requestString = JsonConvert.SerializeObject(request);
                var requestContent = new StringContent(requestString, Encoding.Default, MimeTypes.ApplicationJson);
                var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), request.Path) { Content = requestContent };
                var httpResponse = await _httpClient.SendAsync(requestMessage);
                httpResponse.EnsureSuccessStatusCode();
            }
            catch (AggregateException exception)
            {
                //rethrow actual exception
                throw exception.InnerException;
            }
        }

        #endregion
    }
}