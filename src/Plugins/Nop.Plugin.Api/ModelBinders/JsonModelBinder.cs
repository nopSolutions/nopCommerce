using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Validators;
using Nop.Services.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.ModelBinders
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using System;

    public class JsonModelBinder<T> : IModelBinder where T : class, new()
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly ILocalizationService _localizationService;

        private readonly int _languageId;

        public JsonModelBinder(IJsonHelper jsonHelper, ILocalizationService localizationService, ILanguageService languageService)
        {
            _jsonHelper = jsonHelper;
            _localizationService = localizationService;

            // Languages are ordered by display order so the first language will be with the smallest display order.
            var firstLanguage = languageService.GetAllLanguages().FirstOrDefault();
            if (firstLanguage != null)
            {
                _languageId = firstLanguage.Id;
            }
            else
            {
                _languageId = 0;
            }
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyValuePairs = GetPropertyValuePairs(bindingContext);
            if (propertyValuePairs == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            if (bindingContext.ModelState.IsValid)
            {
                // You will have id parameter passed in the model binder only when you have put request.
                // because get and delete do not use the model binder.
                // Here we insert the id in the property value pairs to be validated by the dto validator in a later point.
                var routeDataId = GetRouteDataId(bindingContext.ActionContext);

                if (routeDataId != null)
                {
                    // Here we insert the route data id in the value paires.
                    // If id is contained in the category json body the one from the route data is used instead.
                    InsertIdInTheValuePaires(propertyValuePairs, routeDataId);
                }

                // We need to call this method here so it will be certain that the routeDataId will be in the propertyValuePaires
                // when the request is PUT.
                ValidateValueTypes(bindingContext, propertyValuePairs);

                Delta<T> delta = null;

                if (bindingContext.ModelState.IsValid)
                {
                    delta = new Delta<T>(propertyValuePairs);
                    ValidateModel(bindingContext, propertyValuePairs, delta.Dto);
                }

                if (bindingContext.ModelState.IsValid)
                {
                    bindingContext.Model = delta;
                    bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }

        private Dictionary<string, object> GetPropertyValuePairs(ModelBindingContext bindingContext)
        {
            Dictionary<string, object> result = null;

            if (bindingContext.ModelState.IsValid)
            {
                try
                {
                    //get the root dictionary and root property (these will throw exceptions if they fail)
                    result = _jsonHelper.GetRequestJsonDictionaryFromStream(bindingContext.HttpContext.Request.Body, true);
                    var rootPropertyName = _jsonHelper.GetRootPropertyName<T>();

                    result = (Dictionary<string, object>)result[rootPropertyName];
                }
                catch (Exception ex)
                {
                    bindingContext.ModelState.AddModelError("json", ex.Message);
                }
            }

            return result;
        }

        private object GetRouteDataId(ActionContext actionContext)
        {
            object routeDataId = null;

            if (actionContext.RouteData.Values.ContainsKey("id"))
            {
                routeDataId = actionContext.RouteData.Values["id"];
            }

            return routeDataId;
        }

        private void ValidateValueTypes(ModelBindingContext bindingContext, Dictionary<string, object> propertyValuePaires)
        {
            var errors = new Dictionary<string, string>();

            // Validate if the property value pairs passed maches the type.
            var typeValidator = new TypeValidator<T>();

            if (!typeValidator.IsValid(propertyValuePaires))
            {
                foreach (var invalidProperty in typeValidator.InvalidProperties)
                {
                    var key = string.Format(_localizationService.GetResource("Api.InvalidType", _languageId, false), invalidProperty);

                    if (!errors.ContainsKey(key))
                    {
                        errors.Add(key, _localizationService.GetResource("Api.InvalidPropertyType", _languageId, false));
                    }
                }
            }
            
            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    bindingContext.ModelState.AddModelError(error.Key, error.Value);
                }
            }
        }

        private void ValidateModel(ModelBindingContext bindingContext, Dictionary<string, object> propertyValuePaires, T dto)
        {
            // this method validates each property by checking if it has an attribute that inherits from BaseValidationAttribute
            // these attribtues are different than FluentValidation attributes, so they need to be validated manually

            var dtoProperties = dto.GetType().GetProperties();
            foreach (var property in dtoProperties)
            {
                // Check property type
                var validationAttribute = property.PropertyType.GetCustomAttribute(typeof(BaseValidationAttribute)) as BaseValidationAttribute;

                // If not on property type, check the property itself.
                if (validationAttribute == null)
                {
                    validationAttribute = property.GetCustomAttribute(typeof(BaseValidationAttribute)) as BaseValidationAttribute;
                }

                if (validationAttribute != null)
                {
                    validationAttribute.Validate(property.GetValue(dto));
                    var errors = validationAttribute.GetErrors();

                    if (errors.Count > 0)
                    {
                        foreach (var error in errors)
                        {
                            bindingContext.ModelState.AddModelError(error.Key, error.Value);
                        }
                    }
                }
            }
        }

        private void InsertIdInTheValuePaires(Dictionary<string, object> propertyValuePaires, object requestId)
        {
            if (propertyValuePaires.ContainsKey("id"))
            {
                propertyValuePaires["id"] = requestId;
            }
            else
            {
                propertyValuePaires.Add("id", requestId);
            }
        }
    }
}