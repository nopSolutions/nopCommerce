using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.ProductsParameters
{
    [ModelBinder(typeof(ParametersModelBinder<ProductsCountParametersModel>))]
    public class ProductsCountParametersModel : BaseProductsParametersModel
    {
        /// <summary>
        ///     filter for downloadable or non-downloadable products (products with or without shipment)
        /// </summary>
        [JsonProperty("is_download")]
        public bool? IsDownload { get; set; }
    }
}
