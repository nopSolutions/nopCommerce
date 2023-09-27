using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.OAuth
{
    /// <summary>
    /// Represents authentication details
    /// </summary>
    public class Authentication : ApiResponse
    {
        /// <summary>
        /// Gets or sets the access token that is exchanged with the authorisation code
        /// </summary>
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the remaining lifetime of an access token in seconds
        /// </summary>
        [JsonProperty(PropertyName = "expires_in")]
        public int? ExpiresIn { get; set; }
    }
}