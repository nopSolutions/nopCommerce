using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Tax.Avalara.ItemClassificationAPI;

public class GetHSClassificationRequestc : Request
{
    public GetHSClassificationRequestc(string hsClassificationId, string companyId)
    {
        HSClassificationId = hsClassificationId;
        CompanyId = companyId;
    }

    /// <summary>
    /// Gets or sets the HS classification Id
    /// </summary>
    [JsonIgnore]
    public string HSClassificationId { get; }

    /// <summary>
    /// Gets or sets the company Id
    /// </summary>
    [JsonIgnore]
    public string CompanyId { get; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    public override string Path => $"api/v2/companies/{CompanyId}/classifications/hs/{HSClassificationId}";

    /// <summary>
    /// Gets the request method
    /// </summary>
    public override string Method => HttpMethods.Get;
}