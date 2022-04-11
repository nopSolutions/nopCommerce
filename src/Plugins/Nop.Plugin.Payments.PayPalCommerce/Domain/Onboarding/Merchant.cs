using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Domain.Onboarding
{
    /// <summary>
    /// Represents merchant details
    /// </summary>
    public class Merchant
    {
        /// <summary>
        /// Gets or sets the internal merchant id
        /// </summary>
        [JsonProperty(PropertyName = "merchant_guid")]
        public string MerchantGuid { get; set; }

        /// <summary>
        /// Gets or sets the merchant id
        /// </summary>
        [JsonProperty(PropertyName = "merchant_id")]
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the merchant email
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the email is confirmed
        /// </summary>
        [JsonProperty(PropertyName = "email_confirmed")]
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets the merchant REST API client id
        /// </summary>
        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the merchant REST API client secret
        /// </summary>
        [JsonProperty(PropertyName = "client_secret")]
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the URL to sign up the merchant
        /// </summary>
        [JsonProperty(PropertyName = "sign_up_url")]
        public string SignUpUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the payments are receivable
        /// </summary>
        [JsonProperty(PropertyName = "payments_receivable")]
        public bool PaymentsReceivable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the merchant grants permission to perform payment
        /// </summary>
        [JsonProperty(PropertyName = "permission_granted")]
        public bool PermissionGranted { get; set; }
    }
}