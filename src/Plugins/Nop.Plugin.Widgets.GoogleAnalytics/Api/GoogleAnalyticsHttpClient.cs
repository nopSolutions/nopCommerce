using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Widgets.GoogleAnalytics.Api.Models;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Api
{
    /// <summary>
    /// Represents HTTP client to request Google Analytics 4 services
    /// </summary>
    public class GoogleAnalyticsHttpClient
    {
        #region Fields

        protected readonly HttpClient _httpClient;

        #endregion

        #region Ctor

        public GoogleAnalyticsHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Request API service
        /// </summary>
        /// <param name="request">Event Request</param>
        /// <param name="googleAnalyticsSettings">Google Analytics settings</param>
        /// <returns>The asynchronous task whose result contains response details</returns>
        public async Task RequestAsync(EventRequest request, GoogleAnalyticsSettings googleAnalyticsSettings)
        {
            //configure client
            try
            {
                var query = new Dictionary<string, string>()
                {
                    ["api_secret"] = googleAnalyticsSettings.ApiSecret,
                    ["measurement_id"] = googleAnalyticsSettings.GoogleId
                };

                var uri = QueryHelpers.AddQueryString(googleAnalyticsSettings.UseSandbox ? GoogleAnalyticsDefaults.EndPointDebugUrl : GoogleAnalyticsDefaults.EndPointUrl, query);

                _httpClient.BaseAddress = new Uri(uri);
                _httpClient.Timeout = TimeSpan.FromSeconds(10);

                var requestString = JsonConvert.SerializeObject(request);
                var requestContent = new StringContent(requestString, Encoding.Default, MimeTypes.ApplicationJson);
                var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), null as Uri) { Content = requestContent };
                var httpResponse = await _httpClient.SendAsync(requestMessage);
                httpResponse.EnsureSuccessStatusCode();

                var responseString = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Response>(responseString);

                if (googleAnalyticsSettings.UseSandbox && (result?.ValidationMessages.Any() ?? false))
                    throw new NopException($@"Google Analytics validation error (Measurement Protocol):
                        {responseString}");
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
