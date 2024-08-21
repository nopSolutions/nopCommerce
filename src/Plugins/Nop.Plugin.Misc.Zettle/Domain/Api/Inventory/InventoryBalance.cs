using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Inventory;

/// <summary>
/// Represents inventory balance details
/// </summary>
public class InventoryBalance : ApiResponse
{
    /// <summary>
    /// Gets or sets the unique identifier as UUID version 1
    /// </summary>
    [JsonProperty(PropertyName = "locationUuid")]
    public string LocationUuid { get; set; }

    /// <summary>
    /// Gets or sets the location type
    /// </summary>
    [JsonProperty(PropertyName = "locationType")]
    public string LocationType { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier as UUID version 1
    /// </summary>
    [JsonProperty(PropertyName = "productUuid")]
    public string ProductUuid { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier as UUID version 1
    /// </summary>
    [JsonProperty(PropertyName = "variantUuid")]
    public string VariantUuid { get; set; }

    /// <summary>
    /// Gets or sets the inventory balance
    /// </summary>
    [JsonProperty(PropertyName = "balance")]
    public int? Balance { get; set; }
}