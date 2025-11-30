using Newtonsoft.Json;
using Nop.Plugin.Api.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.DTOs.StateProvinces 
{
    public class StateProvincesRootObject : ISerializableObject
    {
        public StateProvincesRootObject()
        {
            StateProvinces = new List<StateProvinceDto>();
        }

        [JsonProperty("state_provinces")]
        public IList<StateProvinceDto> StateProvinces { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "state_provinces";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(StateProvinceDto);
        }
    }
}
