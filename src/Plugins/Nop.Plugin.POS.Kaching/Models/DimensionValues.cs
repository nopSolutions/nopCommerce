using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class DimensionValues
    {
        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }
    }
}
