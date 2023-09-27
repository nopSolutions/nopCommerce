using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Nop.Web.Framework.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// Represents model binder for string properties
    /// </summary>
    public class StringModelBinder : IModelBinder
    {
        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));
            
            var valueProviderResult =
                bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult.FirstValue is { } str &&
                !string.IsNullOrEmpty(str))
            {
                var noTrim = (bindingContext.ModelMetadata as DefaultModelMetadata)?.Attributes.Attributes.Any(p => p.GetType() == typeof(NoTrimAttribute));

                bindingContext.Result = ModelBindingResult.Success((noTrim.HasValue && noTrim.Value) ? str : str.Trim());
            }

            return Task.CompletedTask;
        }
    }
}