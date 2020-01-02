using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.Square.Domain;
using Nop.Services.Configuration;

namespace Nop.Plugin.Payments.Square.Services
{
    /// <summary>
    /// Represents plugin authorization HTTP client
    /// </summary>
    public class SquareAuthorizationHttpClient
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public SquareAuthorizationHttpClient(HttpClient client,
            ISettingService settingService)
        {
            //configure client
            client.BaseAddress = new Uri("https://connect.squareup.com/oauth2/");
            client.Timeout = TimeSpan.FromMilliseconds(5000);
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, SquarePaymentDefaults.UserAgent);
            client.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

            _httpClient = client;
            _settingService = settingService;
        }

        #endregion

        #region Properties

        public string BaseAddress => _httpClient.BaseAddress.ToString();

        #endregion

        #region Methods

        /// <summary>
        /// Exchange the authorization code for an access token
        /// </summary>
        /// <param name="authorizationCode">Authorization code</param>
        /// <param name="storeId">Store identifier for which access token should be obtained</param>
        /// <returns>The asynchronous task whose result contains access and refresh tokens</returns>
        public async Task<(string AccessToken, string RefreshToken)> ObtainAccessTokenAsync(string authorizationCode, int storeId)
        {
            try
            {
                var settings = _settingService.LoadSetting<SquarePaymentSettings>(storeId);

                //get response
                var request = new ObtainAccessTokenRequest
                {
                    ApplicationId = settings.ApplicationId,
                    ApplicationSecret = settings.ApplicationSecret,
                    GrantType = GrantType.New,
                    AuthorizationCode = authorizationCode
                };
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, "token");
                httpRequest.Headers.Add(HeaderNames.Authorization, $"Client {settings.ApplicationSecret}");
                httpRequest.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, MimeTypes.ApplicationJson);

                var response = await _httpClient.SendAsync(httpRequest);

                //return received access token
                var responseContent = await response.Content.ReadAsStringAsync();
                var accessTokenResponse = JsonConvert.DeserializeObject<ObtainAccessTokenResponse>(responseContent);
                return (accessTokenResponse?.AccessToken, accessTokenResponse?.RefreshToken);
            }
            catch (AggregateException exception)
            {
                //rethrow actual exception
                throw exception.InnerException;
            }
        }

        /// <summary>
        /// Renew the expired access token
        /// </summary>
        /// <param name="storeId">Store identifier for which access token should be updated</param>
        /// <returns>The asynchronous task whose result contains access and refresh tokens</returns>
        public async Task<(string AccessToken, string RefreshToken)> RenewAccessTokenAsync(int storeId)
        {
            try
            {
                var settings = _settingService.LoadSetting<SquarePaymentSettings>(storeId);

                //get response
                var request = new ObtainAccessTokenRequest
                {
                    ApplicationId = settings.ApplicationId,
                    ApplicationSecret = settings.ApplicationSecret,
                    GrantType = GrantType.Refresh,
                    RefreshToken = settings.RefreshToken
                };
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, "token");
                httpRequest.Headers.Add(HeaderNames.Authorization, $"Client {settings.ApplicationSecret}");
                httpRequest.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, MimeTypes.ApplicationJson);

                var response = await _httpClient.SendAsync(httpRequest);

                //return received access token
                var responseContent = await response.Content.ReadAsStringAsync();
                var accessTokenResponse = JsonConvert.DeserializeObject<ObtainAccessTokenResponse>(responseContent);
                return (accessTokenResponse?.AccessToken, accessTokenResponse?.RefreshToken);
            }
            catch (AggregateException exception)
            {
                //rethrow actual exception
                throw exception.InnerException;
            }
        }

        /// <summary>
        /// Revoke all access tokens
        /// </summary>
        /// <param name="storeId">Store identifier for which access token should be revoked</param>
        /// <returns>The asynchronous task whose result determines whether tokens are revoked</returns>
        public async Task<bool> RevokeAccessTokensAsync(int storeId)
        {
            try
            {
                var settings = _settingService.LoadSetting<SquarePaymentSettings>(storeId);

                //get response
                var request = new RevokeAccessTokenRequest
                {
                    ApplicationId = settings.ApplicationId,
                    AccessToken = settings.AccessToken
                };
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, "revoke");
                httpRequest.Headers.Add(HeaderNames.Authorization, $"Client {settings.ApplicationSecret}");
                httpRequest.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, MimeTypes.ApplicationJson);

                var response = await _httpClient.SendAsync(httpRequest);

                //return result
                var responseContent = await response.Content.ReadAsStringAsync();
                var accessTokenResponse = JsonConvert.DeserializeObject<RevokeAccessTokenResponse>(responseContent);
                return accessTokenResponse?.SuccessfullyRevoked ?? false;
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