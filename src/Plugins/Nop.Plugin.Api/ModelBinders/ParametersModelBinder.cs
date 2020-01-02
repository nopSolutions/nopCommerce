using Nop.Plugin.Api.Converters;

namespace Nop.Plugin.Api.ModelBinders
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    
    public class ParametersModelBinder<T> : IModelBinder where T : class, new()
    {
        private readonly IObjectConverter _objectConverter;

        public ParametersModelBinder(IObjectConverter objectConverter)
        {
            _objectConverter = objectConverter;
        }
        
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            
            if (bindingContext.HttpContext.Request.QueryString.HasValue)
            {
                var queryParameters = bindingContext.HttpContext.Request.Query.ToDictionary(pair => pair.Key, pair => pair.Value.ToString());

                bindingContext.Model = _objectConverter.ToObject<T>(queryParameters);
            }
            else
            {
                bindingContext.Model = new T();
            }

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);

            // This should be true otherwise the model will be null.
            return Task.CompletedTask;
        }
    }
}