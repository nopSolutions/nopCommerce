using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Mvc.ModelBinding.Binders;

/// <summary>
/// Represents a model binder provider for specific properties
/// </summary>
public partial class NopModelBinderProvider : IModelBinderProvider
{
    IModelBinder IModelBinderProvider.GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.PropertyName == nameof(BaseNopModel.CustomProperties) && context.Metadata.ModelType == typeof(Dictionary<string, string>))
            return new CustomPropertiesModelBinder();

        if (!context.Metadata.IsComplexType && context.Metadata.ModelType == typeof(string))
        {
            //only handle strings bound from value providers (query/route/form). A string with an explicit
            //non-value-provider source ([FromHeader], [FromBody], [FromServices], ...) has to be left to the
            //built-in binders; StringModelBinder reads value providers only and would bind it to null
            var bindingSource = context.BindingInfo?.BindingSource;
            if (bindingSource == null || BindingSource.ModelBinding.CanAcceptDataFrom(bindingSource))
                return new StringModelBinder();
        }

        return null;
    }
}