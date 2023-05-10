using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Api.Models
{
    public class ValidationMessage
    {
        /// <summary>
        /// The path to the field that was invalid
        /// </summary>
        [JsonProperty("fieldPath")]
        public string FieldPath { get; set; }

        /// <summary>
        /// A description of the error
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// A ValidationCode that corresponds to the error
        /// </summary>
        [JsonProperty("validationCode")]
        public string ValidationCode { get; set; }
    }
}
