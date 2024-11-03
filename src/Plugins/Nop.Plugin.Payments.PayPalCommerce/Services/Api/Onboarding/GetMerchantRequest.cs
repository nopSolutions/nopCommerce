using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Onboarding;

/// <summary>
/// Represents the request to get a merchant details
/// </summary>
public class GetMerchantRequest : IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the ID
    /// </summary>
    [JsonIgnore]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the PayPal account ID of the merchant.
    /// </summary>
    [JsonIgnore]
    public string MerchantId { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v1/customer/partners/{Uri.EscapeDataString(Id)}/merchant-integrations/{Uri.EscapeDataString(MerchantId)}?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Get;

    #endregion
}