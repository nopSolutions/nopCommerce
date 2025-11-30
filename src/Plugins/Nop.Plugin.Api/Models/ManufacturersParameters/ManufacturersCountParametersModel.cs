using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.ManufacturersParameters
{
    [ModelBinder(typeof(ParametersModelBinder<ManufacturersCountParametersModel>))]
    public class ManufacturersCountParametersModel : BaseManufacturersParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}
