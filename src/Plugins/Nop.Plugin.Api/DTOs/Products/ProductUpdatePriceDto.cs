using Newtonsoft.Json;
using Nop.Plugin.Api.DTO.Base;

namespace Nop.Plugin.Api.DTO.Products
{
    [JsonObject(Title = "ProductUpdatePrice")]
    //[Validator(typeof(ProductDtoValidator))]
    public class ProductUpdatePriceDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the price
        /// </summary>
        [JsonProperty("price")]
        [JsonRequired]
        public decimal Price { get; set; }
    }

    [JsonObject(Title = "ProductsUpdatePrice")]
    //[Validator(typeof(ProductDtoValidator))]
    public class ProductsUpdatePriceDto
    {
        /// <summary>
        ///     Gets or sets the price
        /// </summary>
        [JsonProperty("ProductsPrice")]
        [JsonRequired]
        public List<ProductUpdatePriceDto> ProductsPrice { get; set; }
    }
}
