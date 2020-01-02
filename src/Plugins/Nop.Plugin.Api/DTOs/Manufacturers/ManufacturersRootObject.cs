using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.DTOs.Manufacturers
{
    public class ManufacturersRootObject : ISerializableObject
    {
        public ManufacturersRootObject()
        {
            Manufacturers = new List<ManufacturerDto>();
        }

        [JsonProperty("manufacturers")]
        public IList<ManufacturerDto> Manufacturers { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "manufacturers";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ManufacturerDto);
        }
    }
}