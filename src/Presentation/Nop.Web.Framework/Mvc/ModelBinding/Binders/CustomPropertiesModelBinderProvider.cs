using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// Represents a model binder provider for CustomProperties
    /// </summary>
    [Obsolete]
    public class CustomPropertiesModelBinderProvider : IModelBinderProvider
    {
        IModelBinder IModelBinderProvider.GetBinder(ModelBinderProviderContext context)
        {
            var propertyBinders = context.Metadata.Properties
                    .ToDictionary(modelProperty => modelProperty, modelProperty => context.CreateBinder(modelProperty));

            if (context.Metadata.ModelType == typeof(Dictionary<string, object>) && context.Metadata.PropertyName == nameof(BaseNopModel.CustomProperties))
                return new CustomPropertiesModelBinder();
            else
                return null;
        }
    }
}
