using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Data;
using Nop.Web.Framework.Controllers;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute that saves a selected tabs for tabs
    /// </summary>
    public sealed class SaveSelectedTabAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        /// <param name="persistForTheNextRequest">Whether a message should be persisted for the next request</param>
        public SaveSelectedTabAttribute(bool ignore = false, bool persistForTheNextRequest = true) : base(typeof(SaveSelectedTabFilter))
        {
            PersistForTheNextRequest = persistForTheNextRequest;
            IgnoreFilter = ignore;
            Arguments = new object[] { ignore, persistForTheNextRequest };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to ignore the execution of filter actions
        /// </summary>
        public bool IgnoreFilter { get; }

        /// <summary>
        /// Gets a value indicating whether a message should be persisted for the next request
        /// </summary>
        public bool PersistForTheNextRequest { get; }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter confirming that checks whether current connection is secured and properly redirect if necessary
        /// </summary>
        private class SaveSelectedTabFilter : IAsyncActionFilter
        {
            #region Fields

            protected readonly bool _ignoreFilter;
            private bool _persistForTheNextRequest;
            protected readonly IWebHelper _webHelper;

            #endregion

            #region Ctor

            public SaveSelectedTabFilter(bool ignoreFilter, bool persistForTheNextRequest,
                IWebHelper webHelper)
            {
                _ignoreFilter = ignoreFilter;
                _persistForTheNextRequest = persistForTheNextRequest;
                _webHelper = webHelper;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            private void SaveSelectedTab(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                //only in POST requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Post, StringComparison.InvariantCultureIgnoreCase))
                    return;

                //ignore AJAX requests
                if (_webHelper.IsAjaxRequest(context.HttpContext.Request))
                    return;

                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //check whether this filter has been overridden for the Action
                var actionFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter)
                    .OfType<SaveSelectedTabAttribute>()
                    .FirstOrDefault();

                //ignore filter
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                var persistForTheNextRequest = actionFilter?.PersistForTheNextRequest ?? _persistForTheNextRequest;

                if (context.Controller is BaseController controller)
                    controller.SaveSelectedTabName(persistForTheNextRequest: persistForTheNextRequest);
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
                await next();
                SaveSelectedTab(context);
            }

            #endregion
        }

        #endregion
    }
}