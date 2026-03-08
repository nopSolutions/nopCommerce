using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Paystack.Models;

/// <summary>
/// Response from Paystack transaction/initialize API
/// </summary>
public class InitializeTransactionResponse
{
    [JsonProperty("status")]
    public bool Status { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("data")]
    public InitializeTransactionData? Data { get; set; }
}

/// <summary>
/// Data payload from initialize transaction response
/// </summary>
public class InitializeTransactionData
{
    [JsonProperty("authorization_url")]
    public string AuthorizationUrl { get; set; } = string.Empty;

    [JsonProperty("access_code")]
    public string AccessCode { get; set; } = string.Empty;

    [JsonProperty("reference")]
    public string Reference { get; set; } = string.Empty;
}
