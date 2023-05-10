using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Inventory
{
    /// <summary>
    /// Represents request to create inventory balance changes
    /// </summary>
    public class CreateTrackingRequest : InventoryApiRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets the the list of inventory balance changes
        /// </summary>
        [JsonProperty(PropertyName = "productChanges")]
        public List<ProductBalanceChange> ProductChanges { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "returnBalanceForLocationUuid")]
        public string ReturnLocationUuid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "externalUuid")]
        public string ExternalUuid { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => "organizations/self/v2/inventory/bulk";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Post;

        #endregion

        #region Nested classes

        /// <summary>
        /// Represents product balance change details
        /// </summary>
        public class ProductBalanceChange
        {
            /// <summary>
            /// Gets or sets the unique identifier as UUID version 1
            /// </summary>
            [JsonProperty(PropertyName = "productUuid")]
            public string ProductUuid { get; set; }

            /// <summary>
            /// Gets or sets the status of the change
            /// </summary>
            [JsonProperty(PropertyName = "trackingStatusChange")]
            public string TrackingStatusChange { get; set; }

            /// <summary>
            /// Gets or sets the list of balance changes
            /// </summary>
            [JsonProperty(PropertyName = "variantChanges")]
            public List<VariantBalanceChange> VariantChanges { get; set; }
        }

        /// <summary>
        /// Represents product variant balance change details
        /// </summary>
        public class VariantBalanceChange
        {
            /// <summary>
            /// Gets or sets the unique identifier as UUID version 1
            /// </summary>
            [JsonProperty(PropertyName = "fromLocationUuid")]
            public string FromLocationUuid { get; set; }

            /// <summary>
            /// Gets or sets the unique identifier as UUID version 1
            /// </summary>
            [JsonProperty(PropertyName = "toLocationUuid")]
            public string ToLocationUuid { get; set; }

            /// <summary>
            /// Gets or sets the unique identifier as UUID version 1
            /// </summary>
            [JsonProperty(PropertyName = "variantUuid")]
            public string VariantUuid { get; set; }

            /// <summary>
            /// Gets or sets the inventory balance change value
            /// </summary>
            [JsonProperty(PropertyName = "change")]
            public int Change { get; set; }
        }

        #endregion
    }
}