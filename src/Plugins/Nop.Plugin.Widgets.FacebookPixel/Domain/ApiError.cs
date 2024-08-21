using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.FacebookPixel.Domain;

/// <summary>
/// Represents API error
/// </summary>
public class ApiError
{
    /// <summary>
    /// Gets or sets a human-readable description of the error
    /// </summary>
    [JsonProperty(PropertyName = "message")]
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets an error type
    /// </summary>
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets an error code
    /// </summary>
    [JsonProperty(PropertyName = "code")]
    public int? Code { get; set; }

    /// <summary>
    /// Gets or sets an additional information about the error
    /// </summary>
    [JsonProperty(PropertyName = "error_subcode")]
    public int? Subcode { get; set; }

    /// <summary>
    /// Gets or sets the message to display to the user
    /// </summary>
    [JsonProperty(PropertyName = "error_user_msg")]
    public string UserMessage { get; set; }

    /// <summary>
    /// Gets or sets the title of the dialog, if shown
    /// </summary>
    [JsonProperty(PropertyName = "error_user_title")]
    public string UserTitle { get; set; }

    /// <summary>
    /// Gets or sets the internal support identifier
    /// </summary>
    [JsonProperty(PropertyName = "fbtrace_id")]
    public string DebugId { get; set; }
}