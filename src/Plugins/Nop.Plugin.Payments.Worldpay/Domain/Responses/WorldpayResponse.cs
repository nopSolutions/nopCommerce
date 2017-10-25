using System;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Domain.Enums.Converters;

namespace Nop.Plugin.Payments.Worldpay.Domain.Responses
{
    /// <summary>
    /// Represents return values of all Worldpay requests
    /// </summary>
    public class WorldpayResponse
    {
        /// <summary>
        /// Gets or sets a result of the method call.
        /// </summary>
        [JsonConverter(typeof(NullableStringEnumConverter))]
        [JsonProperty("result")]
        public ResponseResultType? Result { get; set; }

        /// <summary>
        /// Gets or sets a response code for the method call.
        /// </summary>
        [JsonProperty("responseCode")]
        public ResponseCode? Code { get; set; }

        /// <summary>
        /// Gets or sets a text description of the response.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a response date.
        /// </summary>
        [JsonProperty("responseDateTime")]
        public DateTime ResponseDate { get; set; }        
    }
}