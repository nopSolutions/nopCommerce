using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core.Http.Extensions;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that check whether form parameter value equals passed value and return result as an action parameter
/// </summary>
public sealed class ParameterBasedOnFormNameAndValueAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute 
    /// </summary>
    /// <param name="formKeyName">The name of the form key</param>
    /// <param name="formValue">The value of the form parameter with specified key name</param>
    /// <param name="actionParameterName">The name of the action parameter to which the result will be passed</param>
    public ParameterBasedOnFormNameAndValueAttribute(string formKeyName, string formValue, string actionParameterName)
        : base(typeof(ParameterBasedOnFormNameAndValueFilter))
    {
        Arguments = [formKeyName, formValue, actionParameterName];
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that check whether form parameter value equals passed value and return result as an action parameter
    /// </summary>
    private class ParameterBasedOnFormNameAndValueFilter : IAsyncActionFilter
    {
        #region Fields

        protected readonly string _formKeyName;
        protected readonly string _formValue;
        protected readonly string _actionParameterName;

        #endregion

        #region Ctor

        public ParameterBasedOnFormNameAndValueFilter(string formKeyName, string formValue, string actionParameterName)
        {
            _formKeyName = formKeyName;
            _formValue = formValue;
            _actionParameterName = actionParameterName;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task CheckParameterBasedOnFormNameAndValueAsync(ActionExecutingContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            //if form key with '_formKeyName' exists and value of this form parameter equals passed '_formValue', 
            //then set specified '_actionParameterName' to true
            var formValue = await context.HttpContext.Request.GetFormValueAsync(_formKeyName);
            context.ActionArguments[_actionParameterName] = !string.IsNullOrEmpty(formValue) && formValue.Equals(_formValue);
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
            await CheckParameterBasedOnFormNameAndValueAsync(context);
            if (context.Result == null)
                await next();
        }

        #endregion
    }

    #endregion
}