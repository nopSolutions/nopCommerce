using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.SpecificationAttributes
{
    public class ProductSpecificationAttributesRootObjectDto : ISerializableObject
    {
        public ProductSpecificationAttributesRootObjectDto()
        {
            ProductSpecificationAttributes = new List<ProductSpecificationAttributeDto>();
        }

        [JsonProperty("product_specification_attributes")]
        public IList<ProductSpecificationAttributeDto> ProductSpecificationAttributes { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "product_specification_attributes";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (ProductSpecificationAttributeDto);
        }
    }
}