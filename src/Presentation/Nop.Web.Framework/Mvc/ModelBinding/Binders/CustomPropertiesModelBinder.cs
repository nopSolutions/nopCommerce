using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Core.Http.Extensions;

namespace Nop.Web.Framework.Mvc.ModelBinding.Binders;

/// <summary>
/// Represents model binder for CustomProperties
/// </summary>
public partial class CustomPropertiesModelBinder : IModelBinder
{
    async Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var modelName = bindingContext.ModelName;

        var result = new Dictionary<string, string>();
        var request = bindingContext.HttpContext.Request;

        if (request.IsPostRequest() && request.HasFormContentType )
        {
            var form = await request.ReadFormAsync();

            foreach (var item in form.Where(x => x.Key.IndexOf(modelName, StringComparison.Ordinal) == 0))
            {
                var dicKey = item.Key.Replace(modelName + "[", "").Replace("]", "");
                result.Add(dicKey, item.Value.ToString());
            }
        }

        if (bindingContext.HttpContext.Request.Method == "GET")
        {
            var queryStringValue = bindingContext.HttpContext.Request.QueryString.Value;
            if (!string.IsNullOrEmpty(queryStringValue))
            {
                var keys = queryStringValue.TrimStart('?').Split('&').Where(x => x.StartsWith(modelName)).ToList();

                foreach (var key in keys)
                {
                    var dicKey = key[(key.IndexOf('[') + 1)..key.IndexOf(']')];
                    var value = key[(key.IndexOf('=') + 1)..];

                    result.Add(dicKey, value);
                }
            }
        }

        bindingContext.Result = ModelBindingResult.Success(result);
    }
}