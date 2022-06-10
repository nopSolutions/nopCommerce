using Newtonsoft.Json;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.ExternalAuth.Facebook.Models
{
    public record FacebookUserDTO : BaseNopModel
    {
        [JsonProperty("algorithm")]
        public string Algorithm { get; set; }

        [JsonProperty("expires")]
        public int Expires { get; set; }

        [JsonProperty("issued_at")]
        public int IssuedAt { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}
