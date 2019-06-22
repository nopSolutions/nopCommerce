using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.AI.Models
{
    public class JsonAiSettings
    {
        [JsonProperty(PropertyName = "instrumentationKey")]
        public string InstrumentationKey { get; set; }

        [JsonProperty(PropertyName = "enableDebug")]
        public bool EnableDebug { get; set; }

        [JsonProperty(PropertyName = "disableExceptionTracking")]
        public bool DisableExceptionTracking { get; set; }

        [JsonProperty(PropertyName = "disableFetchTracking")]
        public bool DisableFetchTracking { get; set; }

        [JsonProperty(PropertyName = "disableAjaxTracking")]
        public bool DisableAjaxTracking { get; set; }

        [JsonProperty(PropertyName = "maxAjaxCallsPerView")]
        public int? MaxAjaxCallsPerView { get; set; }

        [JsonProperty(PropertyName = "overridePageViewDuration")]
        public bool OverridePageViewDuration { get; set; }
    }
}
