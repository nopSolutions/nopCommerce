using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Nop.Web.Framework
{
    [Dependency(typeof(IModelBinderProvider))]
    public class NopModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(Type modelType)
        {
            #region ReturnLocalizedModelBinder

            if (modelType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(modelType))
            {
                var genericArguements = modelType.GetGenericArguments().ToList();
                if (genericArguements.Count == 1)
                {
                    var dictionaryType = typeof (Localization.LocalizedModels<>).MakeGenericType(genericArguements[0]);
                    if (dictionaryType.Equals(modelType))
                        return new Localization.LocalizedPropertyBinder();
                }
            }

            #endregion
            return ModelBinders.Binders.DefaultBinder;
        }
    }
}
