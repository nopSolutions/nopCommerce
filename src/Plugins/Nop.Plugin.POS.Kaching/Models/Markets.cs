using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Markets
    {
        [JsonProperty("dk")]
        public bool Dk { get; set; }
    }
}
