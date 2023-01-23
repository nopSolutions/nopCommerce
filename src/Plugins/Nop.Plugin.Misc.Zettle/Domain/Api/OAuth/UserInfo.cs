using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.OAuth
{
    /// <summary>
    /// Represents user details
    /// </summary>
    public class UserInfo : ApiResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the organization unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "organizationUuid")]
        public string OrganizationUuid { get; set; }
    }
}