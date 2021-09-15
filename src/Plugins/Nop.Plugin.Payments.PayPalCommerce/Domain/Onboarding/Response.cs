using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nop.Plugin.Payments.PayPalCommerce.Domain.Onboarding
{
    /// <summary>
    /// Represents response from the service
    /// </summary>
    /// <typeparam name="TResponse">Response data type</typeparam>
    public class Response<TResponse> where TResponse : class
    {
        /// <summary>
        /// Gets or sets the response result
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ResponseResult? Result { get; set; }

        /// <summary>
        /// Gets or sets the response data
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public TResponse Data { get; set; }

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}