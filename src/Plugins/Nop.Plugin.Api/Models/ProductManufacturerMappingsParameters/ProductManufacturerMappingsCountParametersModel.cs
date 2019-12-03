using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.ProductManufacturerMappingsParameters
{
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(typeof(ParametersModelBinder<ProductManufacturerMappingsCountParametersModel>))]
    public class ProductManufacturerMappingsCountParametersModel : BaseManufacturerMappingsParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}