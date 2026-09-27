using System.Text.Json.Serialization;

namespace Nop.Plugin.Payments.WorldpayHpp.Models.Response;
public class PaymentResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("_links")]
    public Links Links { get; set; }
}
