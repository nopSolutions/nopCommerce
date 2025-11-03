using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.Languages
{
    public class LanguagesRootObject : ISerializableObject
    {
        public LanguagesRootObject()
        {
            Languages = new List<LanguageDto>();
        }

        [JsonProperty("languages")]
        public IList<LanguageDto> Languages { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "languages";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(LanguageDto);
        }
    }
}
