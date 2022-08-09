using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nop.Web.Framework.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// Represents model binder for CustomProperties
    /// </summary>
    [Obsolete]
    public class CustomPropertiesModelBinder : IModelBinder
    {
        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            var result = new Dictionary<string, object>();
            if (bindingContext.HttpContext.Request.Method == "POST")
            {
                var keys = bindingContext.HttpContext.Request.Form.Keys.ToList().Where(x => x.IndexOf(modelName) == 0);

                if (keys != null && keys.Any())
                {
                    foreach (var key in keys)
                    {
                        var dicKey = key.Replace(modelName + "[", "").Replace("]", "");
                        bindingContext.HttpContext.Request.Form.TryGetValue(key, out var value);
                        result.Add(dicKey, value.ToString());
                    }
                }
            }
            if (bindingContext.HttpContext.Request.Method == "GET")
            {
                var keys = bindingContext.HttpContext.Request.QueryString.Value.Split('&').Where(x => x.StartsWith(modelName));

                if (keys != null && keys.Any())
                {
                    foreach (var key in keys)
                    {
                        var dicKey = key[(key.IndexOf("[") + 1)..key.IndexOf("]")];
                        var value = key[(key.IndexOf("=") + 1)..];

                        result.Add(dicKey, value);
                    }
                }
            }
            bindingContext.Result = ModelBindingResult.Success(result);
            
            return Task.CompletedTask;

        }
    }
}
