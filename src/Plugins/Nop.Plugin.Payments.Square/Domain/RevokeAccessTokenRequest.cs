using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Square.Domain
{
    /// <summary>
    /// Represents request parameters to revoke access token
    /// </summary>
    public class RevokeAccessTokenRequest
    {
        /// <summary>
        /// Gets or sets application ID
        /// </summary>
        [JsonProperty(PropertyName = "client_id")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets application secret
        /// </summary>
        [JsonIgnore]
        public string ApplicationSecret { get; set; }

        /// <summary>
        /// Gets or sets the ID of the merchant whose token you want to revoke
        /// </summary>
        [JsonProperty(PropertyName = "merchant_id")]
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the access token of the merchant whose token you want to revoke
        /// </summary>
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }
    }
}