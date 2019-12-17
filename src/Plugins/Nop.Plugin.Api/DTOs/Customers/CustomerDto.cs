using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.Validators;

namespace Nop.Plugin.Api.DTOs.Customers
{
    [JsonObject(Title = "customer")]
    public class CustomerDto : BaseCustomerDto
    {
        private ICollection<ShoppingCartItemDto> _shoppingCartItems;
        private ICollection<AddressDto> _addresses;

        [JsonIgnore]
        [JsonProperty("password")]
        public string Password { get; set; }

        #region Navigation properties

        /// <summary>
        /// Gets or sets shopping cart items
        /// </summary>
        [JsonProperty("shopping_cart_items")]
        [DoNotMap]
        public ICollection<ShoppingCartItemDto> ShoppingCartItems
        {
            get
            {
                if (_shoppingCartItems == null)
                {
                    _shoppingCartItems = new List<ShoppingCartItemDto>();
                }

                return _shoppingCartItems;
            }
            set { _shoppingCartItems = value; }
        }

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
            get
            {
                if (_addresses == null)
                {
                    _addresses = new List<AddressDto>();
                }

                return _addresses;
            }
            set { _addresses = value; }
        }
        #endregion
    }
}
