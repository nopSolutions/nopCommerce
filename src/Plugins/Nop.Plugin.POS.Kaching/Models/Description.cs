using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Description
    {
        [JsonProperty("da")]
        public string Da { get; set; }

        [JsonProperty("en")]
        public string En { get; set; }
    }
}
