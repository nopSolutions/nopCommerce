using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.SpecificationAttributes
{
    public class SpecificationAttributesRootObjectDto : ISerializableObject
    {
        public SpecificationAttributesRootObjectDto()
        {
            SpecificationAttributes = new List<SpecificationAttributeDto>();
        }

        [JsonProperty("specification_attributes")]
        public IList<SpecificationAttributeDto> SpecificationAttributes { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "specification_attributes";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (SpecificationAttributeDto);
        }
    }
}