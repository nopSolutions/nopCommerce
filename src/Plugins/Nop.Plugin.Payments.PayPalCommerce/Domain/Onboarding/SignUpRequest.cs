using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Domain.Onboarding
{
    /// <summary>
    /// Represents request to sign up the merchant
    /// </summary>
    public class SignUpRequest : Request
    {
        public SignUpRequest(string merchantGuid)
        {
            MerchantGuid = merchantGuid;
        }

        /// <summary>
        /// Gets or sets the internal merchant id
        /// </summary>
        [JsonIgnore]
        public string MerchantGuid { get; }

        /// <summary>
        /// Gets or sets the authentication parameters
        /// </summary>
        [JsonProperty(PropertyName = "shared_id")]
        public string SharedId { get; set; }

        /// <summary>
        /// Gets or sets the authentication parameters
        /// </summary>
        [JsonProperty(PropertyName = "auth_code")]
        public string AuthCode { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"paypal/signup/{MerchantGuid}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Post;
    }
}