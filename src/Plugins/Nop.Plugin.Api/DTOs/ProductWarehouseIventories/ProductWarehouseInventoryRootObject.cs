using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.ProductWarehouseIventories
{
    public class ProductWarehouseInventoryRootObject : ISerializableObject
    {
        public ProductWarehouseInventoryRootObject()
        {
            ProductWarehouseInventoryDtos = new List<ProductWarehouseInventoryDto>();
        }

        [JsonProperty("product_warehouse_inventories")]
        public IList<ProductWarehouseInventoryDto> ProductWarehouseInventoryDtos { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "product_warehouse_inventories";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ProductWarehouseInventoryDto);
        }
    }
}