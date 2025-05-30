using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class BatchResponse
{
    [JsonProperty("batchID")] public string BatchId { get; set; }
    [JsonProperty("endpoint")] public string Endpoint { get; set; }
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("createdAt")] public string CreatedAt { get; set; }
    [JsonProperty("startedAt")] public string StartedAt { get; set; }
    [JsonProperty("endedAt")] public string EndedAt { get; set; }
    [JsonProperty("totalCount")] public int TotalCount { get; set; }
    [JsonProperty("finishedCount")] public int FinishedCount { get; set; }
    [JsonProperty("errorsCount")] public int ErrorsCount { get; set; }
    [JsonProperty("method")] public string Method { get; set; }

    [JsonIgnore]
    public string SyncType
    {
        get
        {
            if (string.IsNullOrEmpty(Endpoint))
                return string.Empty;

            if (Endpoint.Equals(OmnisendDefaults.ContactsEndpoint, StringComparison.InvariantCultureIgnoreCase))
                return "Plugins.Misc.Omnisend.SyncContacts";

            if (Endpoint.Equals(OmnisendDefaults.ProductsEndpoint, StringComparison.InvariantCultureIgnoreCase))
                return "Plugins.Misc.Omnisend.SyncProducts";

            if (Endpoint.Equals(OmnisendDefaults.OrdersEndpoint, StringComparison.InvariantCultureIgnoreCase))
                return "Plugins.Misc.Omnisend.SyncOrders";

            if (Endpoint.Equals(OmnisendDefaults.CategoriesEndpoint, StringComparison.InvariantCultureIgnoreCase))
                return "Plugins.Misc.Omnisend.SyncCategories";

            return string.Empty;
        }
    }
}