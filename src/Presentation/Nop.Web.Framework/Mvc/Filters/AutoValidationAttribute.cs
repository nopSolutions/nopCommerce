using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that validates models using FluentValidation before executing an action
/// </summary>
public sealed class AutoValidationAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="ignore">Whether to ignore the execution of filter actions</param>
    public AutoValidationAttribute(bool ignore = false) : base(typeof(AutoValidationFilter))
    {
        IgnoreFilter = ignore;
        Arguments = [ignore];
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to ignore the execution of filter actions
    /// </summary>
    public bool IgnoreFilter { get; }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that validates models using FluentValidation before executing an action
    /// </summary>
    private class AutoValidationFilter : IAsyncActionFilter
    {
        #region Fields

        protected readonly bool _ignoreFilter;

        #endregion

        #region Ctor

        public AutoValidationFilter(bool ignoreFilter)
        {
            _ignoreFilter = ignoreFilter;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Whether to ignore this filter
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>Result</returns>
        private bool IgnoreFilter(FilterContext context)
        {
            //check whether this filter has been overridden for the Action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<AutoValidationAttribute>()
                .FirstOrDefault();

            return actionFilter?.IgnoreFilter ?? _ignoreFilter;
        }

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ValidateModelAsync(ActionExecutingContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (IgnoreFilter(context))
                return;

            if (context.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor)
                return;

            //validate action parameters
            foreach (var parameter in controllerActionDescriptor.Parameters)
            {
                if (!context.ActionArguments.TryGetValue(parameter.Name, out var subject) || subject is null)
                    continue;

                var parameterType = subject.GetType();
                if (parameterType is not { IsClass: true, IsEnum: false, IsValueType: false, IsPrimitive: false })
                    continue;

                //try to get associated validator
                if (EngineContext.Current.Resolve(typeof(IValidator<>).MakeGenericType(parameterType)) is not IValidator validator)
                    continue;

                //prepare validation context
                var validateWithRuleSet = parameter is IParameterInfoParameterDescriptor infoParameterDescriptor
                    && infoParameterDescriptor.ParameterInfo.CustomAttributes.Any(ca => ca.AttributeType == typeof(ValidateAttribute));
                var validationContext = validateWithRuleSet
                    ? ValidationContext<object>.CreateWithOptions(subject, options => options.IncludeRuleSets(NopValidationDefaults.ValidationRuleSet))
                    : new ValidationContext<object>(subject);

                //get validation result
                var validationResult = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                        context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext" />.</param>
        /// <param name="next">
        /// The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate" />. Invoked to execute the next action filter or the action itself.
        /// </param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that on completion indicates the filter has executed.</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await ValidateModelAsync(context);

            if (context.Result == null)
                await next();
        }

        #endregion
    }

    #endregion
}