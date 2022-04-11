using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Domain.Onboarding
{
    /// <summary>
    /// Represents request to revoke access
    /// </summary>
    public class RevokeAccessRequest : Request
    {
        public RevokeAccessRequest(string merchantGuid)
        {
            MerchantGuid = merchantGuid;
        }

        /// <summary>
        /// Gets or sets the internal merchant id
        /// </summary>
        [JsonIgnore]
        public string MerchantGuid { get; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"paypal/merchant/{MerchantGuid}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Delete;
    }
}