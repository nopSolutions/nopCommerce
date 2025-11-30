using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTO.Base;
using Nop.Plugin.Api.DTO.Customers;
using Nop.Plugin.Api.DTO.Products;
using Nop.Plugin.Api.DTOs.ShoppingCarts;

namespace Nop.Plugin.Api.DTO.ShoppingCarts
{
    //[Validator(typeof(ShoppingCartItemDtoValidator))]
    [JsonObject(Title = "shopping_cart_item")]
    public class ShoppingCartItemDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the selected attributes
        /// </summary>
        [JsonProperty("product_attributes")]
        public List<ProductItemAttributeDto> Attributes { get; set; }

        /// <summary>
        ///     Gets or sets the price enter by a customer
        /// </summary>
        [JsonProperty("customer_entered_price")]
        public decimal? CustomerEnteredPrice { get; set; }

        /// <summary>
        ///     Gets or sets the quantity
        /// </summary>
        [JsonProperty("quantity")]
        public int? Quantity { get; set; }

        /// <summary>
        ///     Gets or sets the rental product start date (null if it's not a rental product)
        /// </summary>
        [JsonProperty("rental_start_date_utc")]
        public DateTime? RentalStartDateUtc { get; set; }

        /// <summary>
        ///     Gets or sets the rental product end date (null if it's not a rental product)
        /// </summary>
        [JsonProperty("rental_end_date_utc")]
        public DateTime? RentalEndDateUtc { get; set; }

        /// <summary>
        ///     Gets or sets the date and time of instance creation
        /// </summary>
        [JsonProperty("created_on_utc")]
        public DateTime? CreatedOnUtc { get; set; }

        /// <summary>
        ///     Gets or sets the date and time of instance update
        /// </summary>
        [JsonProperty("updated_on_utc")]
        public DateTime? UpdatedOnUtc { get; set; }

        /// <summary>
        ///     Gets the log type
        /// </summary>
        [JsonProperty("shopping_cart_type", Required = Required.Always)]
        public ShoppingCartType ShoppingCartType { get; set; }

        [JsonProperty("product_id")]
        public int? ProductId { get; set; }

        /// <summary>
        ///     Gets or sets the product
        /// </summary>
        [JsonProperty("product")]
        public ProductDto ProductDto { get; set; }

        [JsonProperty("customer_id")]
        public int? CustomerId { get; set; }
    }
}
