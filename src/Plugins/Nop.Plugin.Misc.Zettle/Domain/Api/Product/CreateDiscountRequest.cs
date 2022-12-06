using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product
{
    /// <summary>
    /// Represents request to create discount
    /// </summary>
    public class CreateDiscountRequest : ProductApiRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the external reference
        /// </summary>
        [JsonProperty(PropertyName = "externalReference")]
        public string ExternalReference { get; set; }

        /// <summary>
        /// Gets or sets the amount
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public Discount.DiscountAmount Amount { get; set; }

        /// <summary>
        /// Gets or sets the percentage
        /// </summary>
        [JsonProperty(PropertyName = "percentage")]
        public decimal? Percentage { get; set; }

        /// <summary>
        /// Gets or sets the image lookup keys
        /// </summary>
        [JsonProperty(PropertyName = "imageLookupKeys")]
        public List<string> ImageLookupKeys { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        [JsonIgnore]
        public override string Path => "organizations/self/discounts";

        /// <summary>
        /// Gets the request method
        /// </summary>
        [JsonIgnore]
        public override string Method => HttpMethods.Post;
    }
}