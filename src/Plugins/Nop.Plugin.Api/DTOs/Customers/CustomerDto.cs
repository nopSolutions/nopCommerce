using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTO.ShoppingCarts;

namespace Nop.Plugin.Api.DTO.Customers
{
    [JsonObject(Title = "customer")]
    //[Validator(typeof(CustomerDtoValidator))]
    public class CustomerDto : BaseCustomerDto
    {
        private ICollection<AddressDto> _addresses;

        [JsonIgnore]
        [JsonProperty("password")]
        public string Password { get; set; }

        #region Navigation properties

        /// <summary>
        ///     Default billing address
        /// </summary>
        [JsonProperty("billing_address")]
        public AddressDto BillingAddress { get; set; }

        /// <summary>
        ///     Default shipping address
        /// </summary>
        [JsonProperty("shipping_address")]
        public AddressDto ShippingAddress { get; set; }

        /// <summary>
        ///     Gets or sets customer addresses
        /// </summary>
        [JsonProperty("addresses")]
        public ICollection<AddressDto> Addresses
        {
            get
            {
                if (_addresses == null)
                {
                    _addresses = new List<AddressDto>();
                }

                return _addresses;
            }
            set => _addresses = value;
        }

        #endregion
    }
}
