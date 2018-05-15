using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Square.Domain
{
    /// <summary>
    /// Represents request parameters to obtain access token
    /// </summary>
    public class ObtainAccessTokenRequest
    {
        /// <summary>
        /// Gets or sets application ID
        /// </summary>
        [JsonProperty(PropertyName = "client_id")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets application secret
        /// </summary>
        [JsonProperty(PropertyName = "client_secret")]
        public string ApplicationSecret { get; set; }

        /// <summary>
        /// Gets or sets the authorization code to exchange
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string AuthorizationCode { get; set; }

        /// <summary>
        /// Gets or sets the redirect URL 
        /// </summary>
        [JsonProperty(PropertyName = "redirect_uri")]
        public string RedirectUrl { get; set; }
    }
}