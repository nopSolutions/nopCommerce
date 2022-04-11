using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Domain.Onboarding
{
    /// <summary>
    /// Represents request to get URL to sign up
    /// </summary>
    public class OnboardingRequest : Request
    {
        /// <summary>
        /// Gets or sets the merchant email
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the merchant language culture
        /// </summary>
        [JsonProperty(PropertyName = "culture")]
        public string Culture { get; set; }

        /// <summary>
        /// Gets or sets the merchant store URL
        /// </summary>
        [JsonProperty(PropertyName = "store_url")]
        public string StoreUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL to redirect the merchant after onboarding
        /// </summary>
        [JsonProperty(PropertyName = "redirect_url")]
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"paypal/onboarding";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Post;
    }
}