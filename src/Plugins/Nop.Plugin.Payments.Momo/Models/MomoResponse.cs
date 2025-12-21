using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Momo.Models;

public class MomoResponse
{
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("reason")]
    public string Reason { get; set; }

    [JsonProperty("financialTransactionId")]
    public string TransactionId { get; set; }

    [JsonProperty("externalId")]
    public string ExternalId { get; set; }

    [JsonProperty("amount")]
    public string Amount { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("payer")]
    public PayerInfo Payer { get; set; }
}

public class PayerInfo
{
    [JsonProperty("partyIdType")]
    public string PartyIdType { get; set; }

    [JsonProperty("partyId")]
    public string PartyId { get; set; }
}

public class PaymentStatusResponse
{
    public bool Success { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
}
