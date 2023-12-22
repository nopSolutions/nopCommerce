using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Tax.Avalara.ItemClassificationAPI;

/// <summary>
/// Represents a request to HS classify an item for a single country of destination
/// </summary>
public class CreateHSClassificationRequest : Request
{
    public CreateHSClassificationRequest(string companyId)
    {
        CompanyId = companyId;
    }

    /// <summary>
    /// Gets or sets the company Id
    /// </summary>
    [JsonIgnore]
    public string CompanyId { get; }

    /// <summary>
    /// The id of the HS classification request. This id is created by CreateHSClassificationRequest and returned in the response
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Country of destination (ISO code of country)
    /// </summary>
    /// <remarks>
    /// Required
    /// </remarks>
    [JsonProperty(PropertyName = "countryOfDestination")]
    public string CountryOfDestination { get; set; }

    /// <summary>
    /// Item
    /// </summary>
    /// <remarks>
    /// Required
    /// </remarks>
    [JsonProperty(PropertyName = "item")]
    public ItemClassificationModel Item { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    public override string Path => $"api/v2/companies/{CompanyId}/classifications/hs/";

    /// <summary>
    /// Gets the request method
    /// </summary>
    public override string Method => HttpMethods.Post;
}