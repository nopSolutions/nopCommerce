using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.Validators;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.ModelBinders
{
    public class JsonModelBinder<T> : IModelBinder where T : class, new()
    {
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;

        public JsonModelBinder(ILocalizationService localizationService, ILanguageService languageService)
        {
            _localizationService = localizationService;
            _languageService = languageService;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            // Use ASP.NET Core's default model binding
            var loggerFactory = bindingContext.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            var defaultBinder = new SimpleTypeModelBinder(typeof(T), loggerFactory);
            await defaultBinder.BindModelAsync(bindingContext);

            if (!bindingContext.ModelState.IsValid)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var model = bindingContext.Result.Model as T;
            if (model == null)
            {
                bindingContext.ModelState.AddModelError("json", "Failed to bind JSON model.");
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var propertyValuePairs = GetPropertyValuePairs(model);
            var routeDataId = GetRouteDataId(bindingContext.ActionContext);

            if (routeDataId != null)
            {
                InsertIdInTheValuePairs(propertyValuePairs, routeDataId);
            }

            await ValidateValueTypesAsync(bindingContext, propertyValuePairs);

            if (bindingContext.ModelState.IsValid)
            {
                var delta = new Delta<T>(propertyValuePairs);
                await ValidateModelAsync(bindingContext, propertyValuePairs, delta.Dto);
                
                if (bindingContext.ModelState.IsValid)
                {
                    bindingContext.Model = delta;
                    bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
                    return;
                }
            }

            bindingContext.Result = ModelBindingResult.Failed();
        }

        private Dictionary<string, object> GetPropertyValuePairs(T model)
        {
            return model.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name, p => p.GetValue(model));
        }

        private object GetRouteDataId(ActionContext actionContext)
        {
            return actionContext.RouteData.Values.TryGetValue("id", out var id) ? id : null;
        }

        private async Task ValidateValueTypesAsync(ModelBindingContext bindingContext, Dictionary<string, object> propertyValuePairs)
        {
            var errors = new Dictionary<string, string>();
            var typeValidator = new TypeValidator<T>();

            if (!typeValidator.IsValid(propertyValuePairs))
            {
                int languageId = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault()?.Id ?? 0;
                string invalidTypeText = await _localizationService.GetResourceAsync("Api.InvalidType", languageId, false);
                string invalidPropertyTypeText = await _localizationService.GetResourceAsync("Api.InvalidPropertyType", languageId, false);

                foreach (var invalidProperty in typeValidator.InvalidProperties)
                {
                    var key = string.Format(invalidTypeText, invalidProperty);
                    if (!errors.ContainsKey(key))
                    {
                        errors.Add(key, invalidPropertyTypeText);
                    }
                }
            }

            foreach (var error in errors)
            {
                bindingContext.ModelState.AddModelError(error.Key, error.Value);
            }
        }

        private async Task ValidateModelAsync(ModelBindingContext bindingContext, Dictionary<string, object> propertyValuePairs, T dto)
        {
            foreach (var property in dto.GetType().GetProperties())
            {
                var validationAttribute = property.GetCustomAttribute<BaseValidationAttribute>() ??
                                          property.PropertyType.GetCustomAttribute<BaseValidationAttribute>();

                if (validationAttribute != null)
                {
                    await validationAttribute.ValidateAsync(property.GetValue(dto));
                    foreach (var error in validationAttribute.GetErrors())
                    {
                        bindingContext.ModelState.AddModelError(error.Key, error.Value);
                    }
                }
            }
        }

        private void InsertIdInTheValuePairs(IDictionary<string, object> propertyValuePairs, object requestId)
        {
            if (propertyValuePairs.ContainsKey("id"))
            {
                propertyValuePairs["id"] = requestId;
            }
            else
            {
                propertyValuePairs.Add("id", requestId);
            }
        }
    }
}
