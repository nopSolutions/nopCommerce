using System;
using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Timing
    {
        [JsonProperty("timestamp")]
        public double Timestamp { get; set; }

        [JsonProperty("timestamp_date_string")]
        public string TimestampDateString { get; set; }

        [JsonProperty("timestamp_string")]
        public DateTimeOffset TimestampString { get; set; }

        [JsonProperty("timestamp_week_string")]
        public string TimestampWeekString { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }
    }
}