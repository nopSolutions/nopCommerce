using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.FacebookPixel.Domain
{
    public class ConversionsEventCustomData
    {
        /// <summary>
        /// Gets or sets a numeric value associated with this event. this could be a monetary value or a value in some other metric
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public decimal? Value { get; set; }

        /// <summary>
        /// Gets or sets the currency for the value specified, if applicable. currency must be a valid iso 4217 three-digit currency code
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the content ids associated with the event, such as product skus for items in an addtocart event. if content_type is a product, then your content ids must be an array with a single string value. otherwise, this array can contain any number of string values
        /// </summary>
        [JsonProperty(PropertyName = "content_ids")]
        public List<string> ContentIds { get; set; }

        /// <summary>
        /// Gets or sets the content type
        /// </summary>
        [JsonProperty(PropertyName = "content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the category of the content associated with the event
        /// </summary>
        [JsonProperty(PropertyName = "content_category")]
        public string ContentCategory { get; set; }

        /// <summary>
        /// Gets or sets the name of the page or product associated with the event
        /// </summary>
        [JsonProperty(PropertyName = "content_name")]
        public string ContentName { get; set; }

        /// <summary>
        /// Gets or sets a search query made by a user.
        /// </summary>
        [JsonProperty(PropertyName = "search_string")]
        public string SearchString { get; set; }

        /// <summary>
        /// Gets or sets the status of the registration event as a string
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets a list of json objects that contain the product ids associated with the event plus information about the products
        /// </summary>
        [JsonProperty(PropertyName = "contents")]
        public List<object> Contents { get; set; }
    }
}
