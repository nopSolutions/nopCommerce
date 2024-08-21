using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product;

/// <summary>
/// Represents the tax rate details
/// </summary>
public class TaxRate : ApiResponse
{
    #region Properties

    /// <summary>
    /// Gets or sets the unique identifier as UUID version 1
    /// </summary>
    [JsonProperty(PropertyName = "uuid")]
    public string Uuid { get; set; }

    /// <summary>
    /// Gets or sets the label
    /// </summary>
    [JsonProperty(PropertyName = "label")]
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets the percentage
    /// </summary>
    [JsonProperty(PropertyName = "percentage")]
    public decimal? Percentage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tax rate is default
    /// </summary>
    [JsonProperty(PropertyName = "default")]
    public bool? IsDefault { get; set; }

    #endregion
}