using System.Net;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Models;

namespace Nop.Plugin.Payments.Worldpay.Domain.Requests
{
    /// <summary>
    /// Represents request parameters to create a card token.
    /// </summary>
    public class CreateTokenRequest : WorldpayPostRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets a public key assigned by Worldpay.
        /// </summary>
        [JsonProperty("publicKey")]
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets a cardholder account information.
        /// </summary>
        [JsonProperty("card")]
        public Card Card { get; set; }

        /// <summary>
        /// Gets or sets a customer identifier to associate to the token. 
        /// If the addToVault parameter is set to true and a customerId is not sent in the request, a customerId will automatically be created and sent back in the response.
        /// </summary>
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the card object is to be added to the vault to be stored as a token.
        /// </summary>
        [JsonProperty("addToVault")]
        public bool AddToVault { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get a request endpoint URL
        /// </summary>
        /// <returns>URL</returns>
        public override string GetRequestUrl() => "api/PreVault/Card";

        /// <summary>
        /// Get a request method
        /// </summary>
        /// <returns>Request method</returns>
        public override string GetRequestMethod() => WebRequestMethods.Http.Post;

        #endregion
    }
}