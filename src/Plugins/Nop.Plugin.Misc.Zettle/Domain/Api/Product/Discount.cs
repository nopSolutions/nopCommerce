using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product
{
    /// <summary>
    /// Represents the discount details
    /// </summary>
    public class Discount : ApiResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the ETag
        /// </summary>
        [JsonProperty(PropertyName = "etag")]
        public string ETag { get; set; }

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
        public DiscountAmount Amount { get; set; }

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
        /// Gets or sets the unique identifier as UUID version 1 of a user who updated the discount
        /// </summary>
        [JsonProperty(PropertyName = "updatedBy")]
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets the updated date
        /// </summary>
        [JsonProperty(PropertyName = "updated")]
        public DateTime? Updated { get; set; }

        /// <summary>
        /// Gets or sets the created date
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public DateTime? Created { get; set; }

        #endregion

        #region Nested classes

        /// <summary>
        /// Represents the discount amount details
        /// </summary>
        public class DiscountAmount
        {
            /// <summary>
            /// Gets or sets the amount
            /// </summary>
            [JsonProperty(PropertyName = "amount")]
            public int? Amount { get; set; }

            /// <summary>
            /// Gets or sets the currency id
            /// </summary>
            [JsonProperty(PropertyName = "currencyId")]
            public string CurrencyId { get; set; }
        }

        #endregion
    }
}