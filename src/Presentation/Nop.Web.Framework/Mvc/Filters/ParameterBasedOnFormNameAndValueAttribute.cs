using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute that check whether form parameter value equals passed value and return result as an action parameter
    /// </summary>
    public class ParameterBasedOnFormNameAndValueAttribute : TypeFilterAttribute
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
            this.Arguments = new object[] { formKeyName, formValue, actionParameterName };
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that check whether form parameter value equals passed value and return result as an action parameter
        /// </summary>
        private class ParameterBasedOnFormNameAndValueFilter : IActionFilter
        {
            #region Fields

            private readonly string _formKeyName;
            private readonly string _formValue;
            private readonly string _actionParameterName;

            #endregion

            #region Ctor

            public ParameterBasedOnFormNameAndValueFilter(string formKeyName, string formValue, string actionParameterName)
            {
                this._formKeyName = formKeyName;
                this._formValue = formValue;
                this._actionParameterName = actionParameterName;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called before the action executes, after model binding is complete
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                //if form key with '_formKeyName' exists and value of this form parameter equals passed '_formValue', 
                //then set specified '_actionParameterName' to true
                var formValue = context.HttpContext.Request.Form[_formKeyName];
                context.ActionArguments[_actionParameterName] = !string.IsNullOrEmpty(formValue) && formValue.Equals(_formValue);
            }

            /// <summary>
            /// Called after the action executes, before the action result
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuted(ActionExecutedContext context)
            {
                //do nothing
            }

            #endregion
        }

        #endregion
    }
}