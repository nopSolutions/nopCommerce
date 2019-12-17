using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.SpecificationAttributes
{
    // JsonProperty is used only for swagger
    [ModelBinder(typeof(ParametersModelBinder<SpecifcationAttributesCountParametersModel>))]
    public class SpecifcationAttributesCountParametersModel
    {
        public SpecifcationAttributesCountParametersModel()
        {

        }
    }
}