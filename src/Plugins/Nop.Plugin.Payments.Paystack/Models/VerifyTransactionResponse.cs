using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Paystack.Models;

/// <summary>
/// Response from Paystack transaction/verify API
/// </summary>
public class VerifyTransactionResponse
{
    [JsonProperty("status")]
    public bool Status { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("data")]
    public VerifyTransactionData? Data { get; set; }
}

/// <summary>
/// Transaction data from verify response
/// </summary>
public class VerifyTransactionData
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

    [JsonProperty("customer")]
    public VerifyTransactionCustomer? Customer { get; set; }

    [JsonProperty("authorization")]
    public VerifyTransactionAuthorization? Authorization { get; set; }
}

/// <summary>
/// Customer info in verify response
/// </summary>
public class VerifyTransactionCustomer
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [JsonProperty("last_name")]
    public string? LastName { get; set; }

    [JsonProperty("customer_code")]
    public string? CustomerCode { get; set; }
}

/// <summary>
/// Authorization (card) info in verify response
/// </summary>
public class VerifyTransactionAuthorization
{
    [JsonProperty("authorization_code")]
    public string? AuthorizationCode { get; set; }

    [JsonProperty("bin")]
    public string? Bin { get; set; }

    [JsonProperty("last4")]
    public string? Last4 { get; set; }

    [JsonProperty("exp_month")]
    public string? ExpMonth { get; set; }

    [JsonProperty("exp_year")]
    public string? ExpYear { get; set; }

    [JsonProperty("channel")]
    public string? Channel { get; set; }

    [JsonProperty("card_type")]
    public string? CardType { get; set; }

    [JsonProperty("bank")]
    public string? Bank { get; set; }

    [JsonProperty("reusable")]
    public bool Reusable { get; set; }
}
