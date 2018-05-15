using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents an account holder billing address details.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets a street address.
        /// </summary>
        [JsonProperty("line1")]
        public string Line1 { get; set; }

        /// <summary>
        /// Gets or sets a city where address is located.
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets a state where address is located; valid values are 2-character state abbreviations.
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets a 5- or 9-digit zip code.
        /// </summary>
        [JsonProperty("zip")]
        public string Zip { get; set; }

        /// <summary>
        /// Gets or sets a country name. Defaults to US.
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets a company name.
        /// </summary>
        [JsonProperty("company")]
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets a phone number.
        /// </summary>
        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}