using System;
using System.Linq;
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
        #region Fields

        private readonly bool _ignoreFilter;
        private readonly bool _persistForTheNextRequest;

        #endregion

        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        /// <param name="persistForTheNextRequest">Whether a message should be persisted for the next request</param>
        public SaveSelectedTabAttribute(bool ignore = false, bool persistForTheNextRequest = true) : base(typeof(SaveSelectedTabFilter))
        {
            _persistForTheNextRequest = persistForTheNextRequest;
            _ignoreFilter = ignore;
            Arguments = new object[] { ignore, persistForTheNextRequest };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to ignore the execution of filter actions
        /// </summary>
        public bool IgnoreFilter => _ignoreFilter;

        /// <summary>
        /// Gets a value indicating whether a message should be persisted for the next request
        /// </summary>
        public bool PersistForTheNextRequest => _persistForTheNextRequest;

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter confirming that checks whether current connection is secured and properly redirect if necessary
        /// </summary>
        private class SaveSelectedTabFilter : IActionFilter
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private bool _persistForTheNextRequest;
            private readonly IWebHelper _webHelper;

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

            #region Methods

            /// <summary>
            /// Called before the action executes, after model binding is complete
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuting(ActionExecutingContext context)
            {
                //do nothing
            }
            /// <summary>
            /// Called after the action executes, before the action result
            /// </summary>
            /// <param name="filterContext">A context for action filters</param>
            public void OnActionExecuted(ActionExecutedContext filterContext)
            {
                if (filterContext == null)
                    throw new ArgumentNullException(nameof(filterContext));

                if (filterContext.HttpContext.Request == null)
                    return;

                //only in POST requests
                if (!filterContext.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Post, StringComparison.InvariantCultureIgnoreCase))
                    return;
                //ignore AJAX requests
                if (_webHelper.IsAjaxRequest(filterContext.HttpContext.Request))
                    return;

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                //check whether this filter has been overridden for the Action
                var actionFilter = filterContext.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter).OfType<SaveSelectedTabAttribute>().FirstOrDefault();

                //ignore filter
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                var persistForTheNextRequest = actionFilter?.PersistForTheNextRequest ?? _persistForTheNextRequest;

                var controller = filterContext.Controller as BaseController;
                if (controller != null)
                    controller.SaveSelectedTabName(persistForTheNextRequest: persistForTheNextRequest);
            }
            
            #endregion
        }

        #endregion
    }
}