using System.Text.Json;

namespace Nop.Plugin.Payments.WorldpayHpp.Models.Webhook;
public class WorldpayWebhookEnvelope
{
    public string EventId { get; set; }
    public DateTime EventTimestamp { get; set; } 
    // Only present for token events
    public string EventType { get; set; }
    public JsonElement EventDetails { get; set; } 
    public bool IsTokenEvent => string.Equals(EventType, "tokenCreated", StringComparison.OrdinalIgnoreCase);
}
