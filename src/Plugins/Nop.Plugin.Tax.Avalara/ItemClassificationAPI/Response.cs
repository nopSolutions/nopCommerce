using Newtonsoft.Json;

namespace Nop.Plugin.Tax.Avalara.ItemClassificationAPI;

/// <summary>
/// Represents response from the service
/// </summary>
public class Response
{
    #region Properties

    /// <summary>
    /// Gets or sets the information about error
    /// </summary>
    [JsonProperty(PropertyName = "error")]
    public ErrorInfo Error { get; set; }

    #endregion

    #region Nested classes

    /// <summary>
    /// Represents information about the error that occurred
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// Gets or sets the type of error that occurred
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the short one-line message to summaryize what went wrong
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets an array of detailed error messages
        /// </summary>
        [JsonProperty(PropertyName = "details")]
        public List<ErrorDetail> Details { get; set; }
    }

    /// <summary>
    /// Represents error detail
    /// </summary>
    public class ErrorDetail
    {
        /// <summary>
        /// Gets or sets the type of error that occurred
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the number
        /// </summary>
        [JsonProperty(PropertyName = "number")]
        public int? Number { get; set; }

        /// <summary>
        /// Gets or sets the message of error
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the full description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }

    #endregion
}