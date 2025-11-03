using System;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.ShoppingCartsParameters
{
    // JsonProperty is used only for swagger
    [ModelBinder(typeof(ParametersModelBinder<ShoppingCartItemsParametersModel>))]
    public class ShoppingCartItemsParametersModel
    {
        public ShoppingCartItemsParametersModel()
        {
            CreatedAtMin = null;
            CreatedAtMax = null;
            UpdatedAtMin = null;
            UpdatedAtMax = null;
            Limit = Constants.Configurations.DefaultLimit;
            Page = Constants.Configurations.DefaultPageValue;
            Fields = string.Empty;
        }

        /// <summary>
        ///     Show shopping cart items created after date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("created_at_min")]
        public DateTime? CreatedAtMin { get; set; }

        /// <summary>
        ///     Show shopping cart items created before date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("created_at_max")]
        public DateTime? CreatedAtMax { get; set; }

        /// <summary>
        ///     Show shopping cart items updated after date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("updated_at_min")]
        public DateTime? UpdatedAtMin { get; set; }

        /// <summary>
        ///     Show shopping cart items updated before date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("updated_at_max")]
        public DateTime? UpdatedAtMax { get; set; }

        /// <summary>
        ///     Amount of results (default: 50) (maximum: 250)
        /// </summary>
        [JsonProperty("limit")]
        public int? Limit { get; set; }

        /// <summary>
        ///     Page to show (default: 1)
        /// </summary>
        [JsonProperty("page")]
        public int? Page { get; set; }

        /// <summary>
        ///     comma-separated list of fields to include in the response
        /// </summary>
        [JsonProperty("fields")]
        public string Fields { get; set; }

        /// <summary>
        ///     Either ShoppingCartType.ShoppingCart or ShoppingCartType.WishList
        /// </summary>
        [JsonProperty("shopping_cart_type")]
        public ShoppingCartType? ShoppingCartType { get; set; }

        /// <summary>
        /// if null, get all shopping cart items for all customers
        /// </summary>
        [JsonProperty("customer_id", Required = Required.AllowNull)]
        public int? CustomerId { get; set; }
    }
}
