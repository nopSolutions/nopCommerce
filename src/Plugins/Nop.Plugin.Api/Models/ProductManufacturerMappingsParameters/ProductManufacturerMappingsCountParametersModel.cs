using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.ProductManufacturerMappingsParameters
{
    [ModelBinder(typeof(ParametersModelBinder<ProductManufacturerMappingsCountParametersModel>))]
    public class ProductManufacturerMappingsCountParametersModel : BaseManufacturerMappingsParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}
