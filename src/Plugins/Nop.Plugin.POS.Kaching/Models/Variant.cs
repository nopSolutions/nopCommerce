using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Variant
    {
        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("dimension_values")]
        public DimensionValues DimensionValues { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
    }
}
