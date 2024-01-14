using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Metadata
    {
        [JsonProperty("channels")]
        public Channels Channels { get; set; }

        [JsonProperty("markets")]
        public Markets Markets { get; set; }
    }
}
