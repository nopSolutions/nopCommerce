using Newtonsoft.Json;

namespace Nop.Plugin.Misc.FreshAddressNewsletterIntegration.Models
{
    public class FreshAddressResponse
    {
        [JsonProperty(PropertyName = "EMAIL")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "FINDING")]
        public string Finding { get; set; }

        [JsonProperty(PropertyName = "COMMENT")]
        public string Comment { get; set; }

        [JsonProperty(PropertyName = "COMMENT_CODE")]
        public string CommentCode { get; set; }

        [JsonProperty(PropertyName = "SUGG_EMAIL")]
        public string SuggestedEmail { get; set; }

        [JsonProperty(PropertyName = "SUGG_COMMENT")]
        public string SuggestedComment { get; set; }

        [JsonProperty(PropertyName = "ERROR_RESPONSE")]
        public string ErrorResponse { get; set; }

        [JsonProperty(PropertyName = "ERROR")]
        public string Error { get; set; }

        [JsonProperty(PropertyName = "UUID")]
        public string UUID { get; set; }

        public bool IsValid
        {
            get
            {
                return Finding == "V";
            }
        }
    }
}
