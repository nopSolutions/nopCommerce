using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Weixin;
using Nop.Data;
using Nop.Services.Weixin;
using Nop.Services.Customers;

namespace Senparc.Weixin.MP.CommonService.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that checks and updates WUser OpenId of customer
    /// </summary>
    public sealed class CheckOpenIdAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public CheckOpenIdAttribute() : base(typeof(CheckOpenIdFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that checks and updates WUser OpenId of customer
        /// </summary>
        private class CheckOpenIdFilter : IActionFilter
        {
            #region Constants

            private const string WUSER_OPENID_QUERY_PARAMETER_NAME = "openid";
            private const string WUSER_OPENID_REFEREE_QUERY_PARAMETER_NAME = "refereeid";

            #endregion

            #region Fields

            private readonly ICustomerService _customerService;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public CheckOpenIdFilter(ICustomerService customerService,
                IWorkContext workContext)
            {
                _customerService = customerService;
                _workContext = workContext;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Set the wuser openid of current customer
            /// </summary>
            /// <param name="affiliate">Affiliate</param>
            protected void SetCustomerOpenId(string openId, string refereeOpenId)
            {
                //ignore search engines
                if (_workContext.CurrentCustomer.IsSearchEngineAccount())
                    return;

                var update = false;

                if (!string.IsNullOrEmpty(openId) && openId.Length < 32 && openId != _workContext.CurrentCustomer.OpenId)
                {
                    _workContext.CurrentCustomer.OpenId = openId;
                    update = true;
                }
                if (!string.IsNullOrEmpty(refereeOpenId) && refereeOpenId.Length < 32 && refereeOpenId != _workContext.CurrentCustomer.OpenIdReferee)
                {
                    _workContext.CurrentCustomer.OpenIdReferee = refereeOpenId
;
                    update = true;
                }

                //update openId Info
                if (update)
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
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

                //check request query parameters
                var request = context.HttpContext.Request;
                if (request?.Query == null || !request.Query.Any())
                    return;

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                var openIds = request.Query[WUSER_OPENID_QUERY_PARAMETER_NAME];
                var refereeOpenIds = request.Query[WUSER_OPENID_REFEREE_QUERY_PARAMETER_NAME];

                var openId = openIds.Any() ? openIds.FirstOrDefault() : string.Empty;
                var refereeOpenId = refereeOpenIds.Any() ? refereeOpenIds.FirstOrDefault() : string.Empty;

                SetCustomerOpenId(openId, refereeOpenId);
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