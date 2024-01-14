using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Channels
    {
        [JsonProperty("pos")]
        public bool Pos { get; set; }

        [JsonProperty("online")]
        public bool Online { get; set; }
    }
}
