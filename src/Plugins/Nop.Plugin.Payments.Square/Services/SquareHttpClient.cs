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
    /// Represents the HTTP client to request Square services
    /// </summary>
    public partial class SquareHttpClient
    {
        #region Fields

        private readonly HttpClient _httpClient;

        #endregion

        #region Ctor

        public SquareHttpClient(HttpClient client)
        {
            //configure client
            client.BaseAddress = new Uri("https://connect.squareup.com/oauth2/");
            client.Timeout = TimeSpan.FromMilliseconds(5000);
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, SquarePaymentDefaults.UserAgent);
            client.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

            _httpClient = client;
        }

        #endregion

        #region Properties

        public string BaseAddress => _httpClient.BaseAddress.ToString();

        #endregion

        #region Methods

        /// <summary>
        /// Exchange the authorization code for an access token
        /// </summary>
        /// <param name="accessTokenRequest">Request parameters to obtain access token</param>
        /// <returns>The asynchronous task whose result contains the access token</returns>
        public async Task<string> ObtainAccessTokenAsync(ObtainAccessTokenRequest accessTokenRequest)
        {
            //get response
            var requestContent = new StringContent(JsonConvert.SerializeObject(accessTokenRequest), Encoding.UTF8, MimeTypes.ApplicationJson);
            var response = await _httpClient.PostAsync("token", requestContent);
            response.EnsureSuccessStatusCode();

            //return received access token
            var responseContent = await response.Content.ReadAsStringAsync();
            var accessTokenResponse = JsonConvert.DeserializeObject<ObtainAccessTokenResponse>(responseContent);
            return accessTokenResponse?.AccessToken;
        }

        /// <summary>
        /// Renew the expired access token
        /// </summary>
        /// <param name="accessTokenRequest">Request parameters to renew access token</param>
        /// <returns>The asynchronous task whose result contains the access token</returns>
        public async Task<string> RenewAccessTokenAsync(RenewAccessTokenRequest accessTokenRequest)
        {
            //add authorization header
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Client {accessTokenRequest.ApplicationSecret}");

            //get response
            var requestContent = new StringContent(JsonConvert.SerializeObject(accessTokenRequest), Encoding.UTF8, MimeTypes.ApplicationJson);
            var response = await _httpClient.PostAsync($"clients/{accessTokenRequest.ApplicationId}/access-token/renew", requestContent);
            response.EnsureSuccessStatusCode();

            //return received access token
            var responseContent = await response.Content.ReadAsStringAsync();
            var accessTokenResponse = JsonConvert.DeserializeObject<RenewAccessTokenResponse>(responseContent);
            return accessTokenResponse?.AccessToken;
        }

        /// <summary>
        /// Revoke all access tokens
        /// </summary>
        /// <param name="revokeTokenRequest">Request parameters to revoke access token</param>
        /// <returns>The asynchronous task whose result determines whether tokens are revoked</returns>
        public async Task<bool> RevokeAccessTokensAsync(RevokeAccessTokenRequest revokeTokenRequest)
        {
            //add authorization header
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Client {revokeTokenRequest.ApplicationSecret}");

            //get response
            var requestContent = new StringContent(JsonConvert.SerializeObject(revokeTokenRequest), Encoding.UTF8, MimeTypes.ApplicationJson);
            var response = await _httpClient.PostAsync("revoke", requestContent);
            response.EnsureSuccessStatusCode();

            //return result
            var responseContent = await response.Content.ReadAsStringAsync();
            var accessTokenResponse = JsonConvert.DeserializeObject<RevokeAccessTokenResponse>(responseContent);
            return accessTokenResponse?.SuccessfullyRevoked ?? false;
        }

        #endregion
    }
}