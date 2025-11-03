using Newtonsoft.Json;
using Nop.Plugin.Api.DTO.Base;

namespace Nop.Plugin.Api.DTO.ProductWarehouseIventories
{
    [JsonObject(Title = "product_warehouse_inventory")]
    public class ProductWarehouseInventoryDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [JsonProperty("product_id")]
        public int? ProductId { get; set; }

        /// <summary>
        /// Gets or sets the warehouse identifier
        /// </summary>
        [JsonProperty("warehouse_id")]
        public int? WarehouseId { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity
        /// </summary>

        [JsonProperty("stock_quantity")] 
        public int? StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets the reserved quantity (ordered but not shipped yet)
        /// </summary>
        [JsonProperty("reserved_quantity")]
        public int? ReservedQuantity { get; set; }
    }
}
