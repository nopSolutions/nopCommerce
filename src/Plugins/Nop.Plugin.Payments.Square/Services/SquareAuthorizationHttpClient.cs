using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.Square.Domain;

namespace Nop.Plugin.Payments.Square.Services
{
    /// <summary>
    /// Represents plugin authorization HTTP client
    /// </summary>
    public class SquareAuthorizationHttpClient
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly SquarePaymentSettings _squarePaymentSettings;

        #endregion

        #region Ctor

        public SquareAuthorizationHttpClient(HttpClient client,
            SquarePaymentSettings squarePaymentSettings)
        {
            //configure client
            client.BaseAddress = new Uri("https://connect.squareup.com/oauth2/");
            client.Timeout = TimeSpan.FromMilliseconds(5000);
            client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Client {squarePaymentSettings.ApplicationSecret}");
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, SquarePaymentDefaults.UserAgent);
            client.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

            _httpClient = client;
            _squarePaymentSettings = squarePaymentSettings;
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
        /// <returns>The asynchronous task whose result contains access and refresh tokens</returns>
        public async Task<(string AccessToken, string RefreshToken)> ObtainAccessTokenAsync(string authorizationCode)
        {
            try
            {
                //get response
                var request = new ObtainAccessTokenRequest
                {
                    ApplicationId = _squarePaymentSettings.ApplicationId,
                    ApplicationSecret = _squarePaymentSettings.ApplicationSecret,
                    GrantType = GrantType.New,
                    AuthorizationCode = authorizationCode
                };
                var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, MimeTypes.ApplicationJson);
                var response = await _httpClient.PostAsync("token", requestContent);

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
        /// <returns>The asynchronous task whose result contains access and refresh tokens</returns>
        public async Task<(string AccessToken, string RefreshToken)> RenewAccessTokenAsync()
        {
            try
            {
                //get response
                var request = new ObtainAccessTokenRequest
                {
                    ApplicationId = _squarePaymentSettings.ApplicationId,
                    ApplicationSecret = _squarePaymentSettings.ApplicationSecret,
                    GrantType = GrantType.Refresh,
                    RefreshToken = _squarePaymentSettings.RefreshToken
                };
                var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, MimeTypes.ApplicationJson);
                var response = await _httpClient.PostAsync("token", requestContent);

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
        /// <returns>The asynchronous task whose result determines whether tokens are revoked</returns>
        public async Task<bool> RevokeAccessTokensAsync()
        {
            try
            {
                //get response
                var request = new RevokeAccessTokenRequest
                {
                    ApplicationId = _squarePaymentSettings.ApplicationId,
                    AccessToken = _squarePaymentSettings.AccessToken
                };
                var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, MimeTypes.ApplicationJson);
                var response = await _httpClient.PostAsync("revoke", requestContent);

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