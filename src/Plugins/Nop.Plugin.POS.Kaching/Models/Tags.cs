using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Tags
    {
        [JsonProperty("herretoj")]
        public bool Herretoj { get; set; }

        [JsonProperty("dametoj")]
        public bool Dametoj { get; set; }

        [JsonProperty("bornetoj")]
        public bool Bornetoj { get; set; }

        [JsonProperty("fodtoj")]
        public bool Fodtoj { get; set; }

        [JsonProperty("rygsaekke")]
        public bool Rygsaekke { get; set; }

        [JsonProperty("soveposer")]
        public bool Soveposer { get; set; }

        [JsonProperty("telte")]
        public bool Telte { get; set; }

        [JsonProperty("kogegrej")]
        public bool Kogegrej { get; set; }

        [JsonProperty("tilbehor")]
        public bool Tilbehor { get; set; }

        [JsonProperty("diverse")]
        public bool Diverse { get; set; }
    }
}
