using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product;

/// <summary>
/// Represents tax rates details
/// </summary>
public class TaxRateList : ApiResponse
{
    /// <summary>
    /// Gets or sets a list of all tax rates
    /// </summary>
    [JsonProperty(PropertyName = "taxRates")]
    public List<TaxRate> TaxRates { get; set; }
}