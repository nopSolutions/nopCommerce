using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Inventory;

/// <summary>
/// Represents inventory balance update details
/// </summary>
public class InventoryBalanceUpdate : ApiResponse
{
    /// <summary>
    /// Gets or sets the unique identifier as UUID version 1
    /// </summary>
    [JsonProperty(PropertyName = "externalUuid")]
    public string ExternalUuid { get; set; }

    /// <summary>
    /// Gets or sets the inventory balance before update
    /// </summary>
    [JsonProperty(PropertyName = "balanceBefore")]
    public List<InventoryBalanceChange> BalanceBefore { get; set; }

    /// <summary>
    /// Gets or sets the inventory balance after update
    /// </summary>
    [JsonProperty(PropertyName = "balanceAfter")]
    public List<InventoryBalanceChange> BalanceAfter { get; set; }

    /// <summary>
    /// Gets or sets the update details
    /// </summary>
    [JsonProperty(PropertyName = "updated")]
    public InventoryBalanceUpdateDetails UpdateDetails { get; set; }

    #region Nested classes

    /// <summary>
    /// Represents inventory balance change details
    /// </summary>
    public class InventoryBalanceChange
    {
        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "locationUuid")]
        public string LocationUuid { get; set; }

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

        /// <summary>
        /// Gets or sets the created date
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public DateTime? Created { get; set; }
    }

    /// <summary>
    /// Represents inventory balance update details
    /// </summary>
    public class InventoryBalanceUpdateDetails
    {
        /// <summary>
        /// Gets or sets the updated timestamp
        /// </summary>
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime? Timestamp { get; set; }
    }

    #endregion
}