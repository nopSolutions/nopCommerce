using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace Nop.Web.Framework.Mvc.ModelBinding
{
    /// <summary>
    /// ModelState extensions
    /// </summary>
    public static class ModelStateExtensions
    {
        private static Dictionary<string, object> SerializeModelState(ModelStateEntry modelState)
        {
            var errors = new List<string>();
            for (var i = 0; i < modelState.Errors.Count; i++)
            {
                var modelError = modelState.Errors[i];
                var errorText = ValidationHelpers.GetModelErrorMessageOrDefault(modelError);

                if (!string.IsNullOrEmpty(errorText))
                {
                    errors.Add(errorText);
                }
            }

            var dictionary = new Dictionary<string, object>();
            dictionary["errors"] = errors.ToArray();
            return dictionary;
        }

        /// <summary>
        /// Serialize errors
        /// </summary>
        /// <param name="modelStateDictionary">ModelStateDictionary</param>
        /// <returns>Result</returns>
        public static object SerializeErrors(this ModelStateDictionary modelStateDictionary)
        {
            return modelStateDictionary.Where(entry => entry.Value.Errors.Any())
                .ToDictionary(entry => entry.Key, entry => SerializeModelState(entry.Value));
        }
    }
}