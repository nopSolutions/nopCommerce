using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nop.Web.Framework.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// Represents model binder for CustomProperties
    /// </summary>
    public class CustomPropertiesModelBinder : IModelBinder
    {
        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            var result = new Dictionary<string, string>();
            if (bindingContext.HttpContext.Request.Method == "POST")
            {
                var keys = bindingContext.HttpContext.Request.Form.Keys
                    .Where(x => x.IndexOf(modelName, StringComparison.Ordinal) == 0).ToList();

                foreach (var key in keys)
                {
                    var dicKey = key.Replace(modelName + "[", "").Replace("]", "");
                    bindingContext.HttpContext.Request.Form.TryGetValue(key, out var value);
                    result.Add(dicKey, value.ToString());
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
                        var dicKey = key[(key.IndexOf("[", StringComparison.Ordinal) + 1)..key.IndexOf("]", StringComparison.Ordinal)];
                        var value = key[(key.IndexOf("=", StringComparison.Ordinal) + 1)..];

                        result.Add(dicKey, value);
                    }
                }
            }

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}
