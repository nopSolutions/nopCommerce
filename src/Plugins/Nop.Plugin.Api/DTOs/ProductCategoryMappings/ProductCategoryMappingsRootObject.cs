using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.ProductCategoryMappings
{
    public class ProductCategoryMappingsRootObject : ISerializableObject
    {
        public ProductCategoryMappingsRootObject()
        {
            ProductCategoryMappingDtos = new List<ProductCategoryMappingDto>();
        }

        [JsonProperty("product_category_mappings")]
        public IList<ProductCategoryMappingDto> ProductCategoryMappingDtos { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "product_category_mappings";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (ProductCategoryMappingDto);
        }
    }
}