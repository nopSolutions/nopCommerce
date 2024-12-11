using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the card data
/// </summary>
public class CardData
{
    #region Properties

    /// <summary>
    /// Gets or sets the level 2 card processing data collections. If your merchant account has been configured for Level 2 processing this field will be passed to the processor on your behalf. Please contact your PayPal Technical Account Manager to define level 2 data for your business.
    /// </summary>
    [JsonProperty(PropertyName = "level_2")]
    public CardDataLevel2 Level2 { get; set; }

    /// <summary>
    /// Gets or sets the level 3 card processing data collections, If your merchant account has been configured for Level 3 processing this field will be passed to the processor on your behalf. Please contact your PayPal Technical Account Manager to define level 3 data for your business.
    /// </summary>
    [JsonProperty(PropertyName = "level_3")]
    public CardDataLevel3 Level3 { get; set; }

    #endregion
}