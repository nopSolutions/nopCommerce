using System;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTO.Base;

namespace Nop.Plugin.Api.DTO
{
    [JsonObject(Title = "address")]
    //[Validator(typeof(AddressDtoValidator))]
    public class AddressDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the first name
        /// </summary>
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        /// <summary>
        ///     Gets or sets the last name
        /// </summary>
        [JsonProperty("last_name")]
        public string LastName { get; set; }

        /// <summary>
        ///     Gets or sets the email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the company
        /// </summary>
        [JsonProperty("company")]
        public string Company { get; set; }

        /// <summary>
        ///     Gets or sets the country identifier
        /// </summary>
        [JsonProperty("country_id")]
        public int? CountryId { get; set; }

        /// <summary>
        ///     Gets or sets the country name
        /// </summary>
        [JsonProperty("country")]
        public string CountryName { get; set; }

        /// <summary>
        ///     Gets or sets the state/province identifier
        /// </summary>
        [JsonProperty("state_province_id")]
        public int? StateProvinceId { get; set; }

        /// <summary>
        ///     Gets or sets the city
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        ///     Gets or sets the address 1
        /// </summary>
        [JsonProperty("address1")]
        public string Address1 { get; set; }

        /// <summary>
        ///     Gets or sets the address 2
        /// </summary>
        [JsonProperty("address2")]
        public string Address2 { get; set; }

        /// <summary>
        ///     Gets or sets the zip/postal code
        /// </summary>
        [JsonProperty("zip_postal_code")]
        public string ZipPostalCode { get; set; }

        /// <summary>
        ///     Gets or sets the phone number
        /// </summary>
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     Gets or sets the fax number
        /// </summary>
        [JsonProperty("fax_number")]
        public string FaxNumber { get; set; }

        /// <summary>
        ///     Gets or sets the custom attributes (see "AddressAttribute" entity for more info)
        /// </summary>
        [JsonProperty("customer_attributes")]
        public string CustomAttributes { get; set; }

        /// <summary>
        ///     Gets or sets the date and time of instance creation
        /// </summary>
        [JsonProperty("created_on_utc")]
        public DateTime? CreatedOnUtc { get; set; }

        /// <summary>
        ///     Gets or sets the state/province
        /// </summary>
        [JsonProperty("province")]
        public string StateProvinceName { get; set; }
    }
}
