using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.FacebookPixel.Domain;

public class ConversionsEvent
{
    /// <summary>
    /// Gets or sets an array of server event objects
    /// </summary>
    [JsonProperty(PropertyName = "data")]
    public List<ConversionsEventDatum> Data { get; set; }
}