using Newtonsoft.Json;

namespace Nop.Plugin.Api.Models.Authentication
{
    public class TokenRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
