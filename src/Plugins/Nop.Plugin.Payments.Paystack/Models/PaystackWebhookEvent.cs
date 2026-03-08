using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Paystack.Models;

/// <summary>
/// Paystack webhook event payload (sent to callback URL)
/// </summary>
public class PaystackWebhookEvent
{
    [JsonProperty("event")]
    public string Event { get; set; } = string.Empty;

    [JsonProperty("data")]
    public PaystackWebhookEventData? Data { get; set; }
}

/// <summary>
/// Webhook event data (transaction details)
/// </summary>
public class PaystackWebhookEventData
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("reference")]
    public string Reference { get; set; } = string.Empty;

    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("gateway_response")]
    public string? GatewayResponse { get; set; }

    [JsonProperty("paid_at")]
    public string? PaidAt { get; set; }

    [JsonProperty("created_at")]
    public string? CreatedAt { get; set; }

    [JsonProperty("channel")]
    public string? Channel { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;
}
