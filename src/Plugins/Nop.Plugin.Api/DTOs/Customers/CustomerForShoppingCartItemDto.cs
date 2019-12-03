using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Customers
{
    // We need this DTO object to avoid loop in the entity to dto mappings. The difference is the missing ShoppingCartItems collection.
    [JsonObject(Title = "customers")]
    public class CustomerForShoppingCartItemDto : BaseCustomerDto
    {
        private ICollection<AddressDto> _addresses;
        
        #region Navigation properties
        
        /// <summary>
        /// Default billing address
        /// </summary>
        [JsonProperty("billing_address")]
        public AddressDto BillingAddress { get; set; }

        /// <summary>
        /// Default shipping address
        /// </summary>
        [JsonProperty("shipping_address")]
        public AddressDto ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets customer addresses
        /// </summary>
        [JsonProperty("addresses")]
        public ICollection<AddressDto> Addresses
        {
            get { return _addresses; }
            set { _addresses = value; }
        }
        #endregion
    }
}
