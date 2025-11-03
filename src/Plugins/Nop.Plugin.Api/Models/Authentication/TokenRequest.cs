using Newtonsoft.Json;

namespace Nop.Plugin.Api.Models.Authentication
{
    public class TokenRequest
    {
        [JsonProperty("guest")]
        public bool Guest { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("remember_me")]
        public bool RememberMe { get; set; }
    }
}
