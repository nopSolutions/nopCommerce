using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api;

/// <summary>
/// Represents the error response
/// </summary>
public class ErrorResponse : IApiResponse
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
    public List<ErrorDetail> Details { get; set; }

    /// <summary>
    /// Gets or sets the links
    /// </summary>
    [JsonProperty(PropertyName = "links")]
    public List<Link> Links { get; set; }

    #endregion

    #region Nested classes

    /// <summary>
    /// Represents the error detail
    /// </summary>
    public class ErrorDetail
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

    #endregion
}