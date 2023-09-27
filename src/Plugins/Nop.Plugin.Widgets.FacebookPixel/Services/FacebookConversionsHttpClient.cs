using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Widgets.FacebookPixel.Domain;

namespace Nop.Plugin.Widgets.FacebookPixel.Services
{
    /// <summary>
    /// Represents the HTTP client to request Facebook Conversions API
    /// </summary>
    public class FacebookConversionsHttpClient
    {
        #region Fields

        protected readonly HttpClient _httpClient;

        #endregion

        #region Ctor

        public FacebookConversionsHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send event through conversions api
        /// </summary>
        /// <param name="facebookPixelConfiguration">Facebook pixel configuration object</param>
        /// <param name="conversionsEvent">Conversions api event object</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the asynchronous task whose result contains the response details
        /// </returns>
        public async Task<string> SendEventAsync(FacebookPixelConfiguration facebookPixelConfiguration, ConversionsEvent conversionsEvent)
        {
            var urlString = string.Join($"/", new string[]
            {
                FacebookPixelDefaults.FbConversionsApiBaseAddress,
                FacebookPixelDefaults.FbConversionsApiVersion,
                facebookPixelConfiguration.PixelId,
                FacebookPixelDefaults.FbConversionsApiEventEndpoint
            }) + $"?access_token=" + facebookPixelConfiguration.AccessToken;

            var jsonString = JsonConvert.SerializeObject(conversionsEvent, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var requestContent = new StringContent(jsonString, Encoding.UTF8, MimeTypes.ApplicationJson);
            var response = await _httpClient.PostAsync(urlString, requestContent);

            return await response.Content.ReadAsStringAsync();
        }

        #endregion
    }
}