using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that adds a detailed validation message that a value cannot be empty
    /// </summary>
    public sealed class NotNullValidationMessageAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public NotNullValidationMessageAttribute() : base(typeof(NotNullValidationMessageFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that adds a detailed validation message that a value cannot be empty
        /// </summary>
        private class NotNullValidationMessageFilter : IAsyncActionFilter
        {
            #region Fields

            private readonly ILocalizationService _localizationService;

            #endregion

            #region Ctor

            public NotNullValidationMessageFilter(ILocalizationService localizationService)
            {
                _localizationService = localizationService;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            private async Task CheckNotNullValidationAsync(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                //only in POST requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Post, StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //whether the model state is invalid
                if (context.ModelState.ErrorCount == 0)
                    return;

                var nullModelValues = context.ModelState
                    .Where(modelState => modelState.Value.ValidationState == ModelValidationState.Invalid
                        && modelState.Value.Errors.Any(error => error.ErrorMessage.Equals(NopValidationDefaults.NotNullValidationLocaleName)))
                    .ToList();
                if (!nullModelValues.Any())
                    return;

                //get model passed to the action
                var model = context.ActionArguments.Values.OfType<BaseNopModel>().FirstOrDefault();
                if (model is null)
                    return;

                //get model properties that failed validation
                var properties = model.GetType().GetProperties();
                var locale = await _localizationService.GetResourceAsync(NopValidationDefaults.NotNullValidationLocaleName);
                foreach (var modelState in nullModelValues)
                {
                    var property = properties
                        .FirstOrDefault(propertyInfo => propertyInfo.Name.Equals(modelState.Key, StringComparison.InvariantCultureIgnoreCase));
                    if (property is null)
                        continue;

                    var displayName = property
                        .GetCustomAttributes(typeof(NopResourceDisplayNameAttribute), true)
                        .OfType<NopResourceDisplayNameAttribute>()
                        .FirstOrDefault()
                        ?.DisplayName
                        ?? property.Name;

                    //set localized error message
                    modelState.Value.Errors.Clear();
                    modelState.Value.Errors.Add(string.Format(locale, displayName));
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                await CheckNotNullValidationAsync(context);
                if (context.Result == null)
                    await next();
            }

            #endregion
        }

        #endregion
    }
}