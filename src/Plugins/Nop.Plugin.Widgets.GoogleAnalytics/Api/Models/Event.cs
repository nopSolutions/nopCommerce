using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    public JObject Params { get; set; }
}