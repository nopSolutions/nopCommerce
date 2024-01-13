using Newtonsoft.Json;

namespace PayPalCheckoutSdk.Core;

/// <summary>
/// Represents an exception details
/// </summary>
public class ExceptionDetails
{
    #region Properties

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the message
    /// </summary>
    [JsonProperty(PropertyName = "message")]
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the debug id
    /// </summary>
    [JsonProperty(PropertyName = "debug_id")]
    public string DebugId { get; set; }

    /// <summary>
    /// Gets or sets the details
    /// </summary>
    [JsonProperty(PropertyName = "details")]
    public List<Detail> Details { get; set; }

    /// <summary>
    /// Gets or sets the links
    /// </summary>
    [JsonProperty(PropertyName = "links")]
    public List<Link> Links { get; set; }

    #endregion

    #region Nested classes

    public class Detail
    {
        /// <summary>
        /// Gets or sets the field
        /// </summary>
        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the location
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the issue
        /// </summary>
        [JsonProperty(PropertyName = "issue")]
        public string Issue { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }

    public class Link
    {
        /// <summary>
        /// Gets or sets the href
        /// </summary>
        [JsonProperty(PropertyName = "href")]
        public string Href { get; set; }

        /// <summary>
        /// Gets or sets the rel
        /// </summary>
        [JsonProperty(PropertyName = "rel")]
        public string Rel { get; set; }

        /// <summary>
        /// Gets or sets the encryption type
        /// </summary>
        [JsonProperty(PropertyName = "encType")]
        public string EncType { get; set; }
    }

    #endregion
}