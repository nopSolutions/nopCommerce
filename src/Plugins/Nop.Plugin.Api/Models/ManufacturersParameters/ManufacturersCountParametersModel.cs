using Nop.Plugin.Api.ModelBinders;
namespace Nop.Plugin.Api.Models.ManufacturersParameters
{
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(typeof(ParametersModelBinder<ManufacturersCountParametersModel>))]
    public class ManufacturersCountParametersModel : BaseManufacturersParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}