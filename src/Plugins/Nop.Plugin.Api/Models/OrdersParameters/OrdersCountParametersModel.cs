using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.OrdersParameters
{
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(typeof(ParametersModelBinder<OrdersCountParametersModel>))]
    public class OrdersCountParametersModel : BaseOrdersParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}