using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Api.Models;

public class Response
{
    /// <summary>
    /// Gets or sets the inventory balance before update
    /// </summary>
    [JsonProperty(PropertyName = "validationMessages")]
    public List<ValidationMessage> ValidationMessages { get; set; }
}