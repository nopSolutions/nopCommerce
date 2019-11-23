using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Square.Domain
{
    /// <summary>
    /// Represents returned values of request to obtain access token 
    /// </summary>
    public class ObtainAccessTokenResponse
    {
        /// <summary>
        /// Gets or sets application access token
        /// </summary>
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the token type. Currently this value is always 'bearer'
        /// </summary>
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the date when access token expires
        /// </summary>
        [JsonProperty(PropertyName = "expires_at")]
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the ID of the authorizing merchant's business
        /// </summary>
        [JsonProperty(PropertyName = "merchant_id")]
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the subscription plan the merchant signed up for
        /// </summary>
        [JsonProperty(PropertyName = "plan_id")]
        public string PlanId { get; set; }

        /// <summary>
        /// Gets or sets the OpenID token belonging to this person, only present if the OPENID scope is included in the authorize request
        /// </summary>
        [JsonProperty(PropertyName = "id_token")]
        public string OpenIdToken { get; set; }

        /// <summary>
        /// Gets or sets a refresh token
        /// </summary>
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}