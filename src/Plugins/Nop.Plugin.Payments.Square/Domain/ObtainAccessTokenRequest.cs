using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

        /// <summary>
        /// Gets or sets the type of method to request an OAuth access token
        /// </summary>
        [JsonProperty(PropertyName = "grant_type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public GrantType GrantType { get; set; }

        /// <summary>
        /// Gets or sets a refresh token. A valid refresh token is required if grant_type is set to refresh_token , to indicate the application wants a replacement for an expired OAuth access token.
        /// </summary>
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets a migration token. This parameter is required if grant_type is set to migration_token to indicate that the application wants to get a replacement OAuth access token.
        /// </summary>
        [JsonProperty(PropertyName = "migration_token")]
        public string MigrationToken { get; set; }
    }
}