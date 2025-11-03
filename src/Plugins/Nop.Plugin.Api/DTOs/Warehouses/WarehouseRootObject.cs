using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.Warehouses
{
    public class WarehousesRootObject : ISerializableObject
    {
        public WarehousesRootObject()
        {
            Warehouses = new List<WarehouseDto>();
        }

        [JsonProperty("warehouses")]
        public IList<WarehouseDto> Warehouses { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "warehouses";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(WarehouseDto);
        }
    }
}
