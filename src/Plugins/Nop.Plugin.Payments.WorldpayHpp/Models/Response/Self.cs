using System.Text.Json.Serialization;

namespace Nop.Plugin.Payments.WorldpayHpp.Models.Response;
public class Self {
    [JsonPropertyName("href")]
    public string Href { get; set; } 
}
