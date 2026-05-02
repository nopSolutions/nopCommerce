using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Api.Models;

[JsonObject]
[Serializable]
public class Event
{
    /// <summary>
    /// Required. The name for the event
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Optional. The parameters for the event
    /// </summary>
    [JsonProperty("params")]
    public Parameters Params { get; set; }
}