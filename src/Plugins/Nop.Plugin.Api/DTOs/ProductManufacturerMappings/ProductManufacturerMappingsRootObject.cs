using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.ProductManufacturerMappings
{
    public class ProductManufacturerMappingsRootObject : ISerializableObject
    {
        public ProductManufacturerMappingsRootObject()
        {
            ProductManufacturerMappingsDtos = new List<ProductManufacturerMappingsDto>();
        }

        [JsonProperty("product_manufacturer_mappings")]
        public IList<ProductManufacturerMappingsDto> ProductManufacturerMappingsDtos { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "product_manufacturer_mappings";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ProductManufacturerMappingsDto);
        }
    }
}
