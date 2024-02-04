using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.ProductAttributes
{
    public class ProductAttributesRootObjectDto : ISerializableObject
    {
        public ProductAttributesRootObjectDto()
        {
            ProductAttributes = new List<ProductAttributeDto>();
        }

        [JsonProperty("product_attributes")]
        public IList<ProductAttributeDto> ProductAttributes { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "product_attributes";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ProductAttributeDto);
        }
    }
}
