using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Square.Domain
{
    /// <summary>
    /// Represents request parameters to renew access token
    /// </summary>
    public class RenewAccessTokenRequest
    {
        /// <summary>
        /// Gets or sets application ID
        /// </summary>
        [JsonIgnore]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets application secret
        /// </summary>
        [JsonIgnore]
        public string ApplicationSecret { get; set; }

        /// <summary>
        /// Gets or sets the token you want to renew
        /// </summary>
        [JsonProperty(PropertyName = "access_token")]
        public string ExpiredAccessToken { get; set; }
    }
}