using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Weixin;
using Nop.Data;
using Nop.Services.Weixin;
using Nop.Services.Customers;
using System.Net.Http;
using Nop.Core.Http.Extensions;

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

            //这两个都是保存推荐人或分享人的信息，当前用户的openid 在Oauth2中获取
            private const string WUSER_OPENID_QUERY_PARAMETER_NAME = "openid";
            private const string WUSER_OPENID_HASH_QUERY_PARAMETER_NAME = "openidhash";

            #endregion

            #region Fields

            private readonly ICustomerService _customerService;
            private readonly IWorkContext _workContext;
            private readonly IWUserService _wUserService;
            private readonly IHttpContextAccessor _httpContextAccessor;

            #endregion

            #region Ctor

            public CheckOpenIdFilter(ICustomerService customerService,
                IWUserService wUserService,
                IHttpContextAccessor httpContextAccessor,
                IWorkContext workContext)
            {
                _customerService = customerService;
                _workContext = workContext;
                _httpContextAccessor = httpContextAccessor;
                _wUserService = wUserService;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Set the wuser openid of current customer
            /// </summary>
            /// <param name="affiliate">Affiliate</param>
            protected void SetCustomerOpenId(string refereeOpenId, long refereeOpenIdHash)
            {
                var update = false;

                //不能自己推荐自己
                if (!string.IsNullOrEmpty(refereeOpenId) && refereeOpenId.Length < 32 && refereeOpenId != _workContext.CurrentCustomer.OpenId)
                {
                    var refereeUser = _wUserService.GetWUserByOpenId(refereeOpenId);
                    if (refereeUser != null)
                    {
                        _workContext.CurrentCustomer.RefereeId = refereeUser.Id;
                        _workContext.CurrentCustomer.RefereeIdUpdateTime = Convert.ToInt32(Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now));
                        update = true;
                    }
                }
                else if (refereeOpenIdHash > 0)
                {
                    var refereeUser = _wUserService.GetWUserByOpenIdHash(refereeOpenIdHash);
                    if (refereeUser != null && refereeUser.OpenId != _workContext.CurrentCustomer.OpenId)
                    {
                        _workContext.CurrentCustomer.RefereeId = refereeUser.Id;
                        _workContext.CurrentCustomer.RefereeIdUpdateTime = Convert.ToInt32(Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now));
                        update = true;
                    }
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

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                //ignore search engines and back ground task
                if (_workContext.CurrentCustomer.IsSearchEngineAccount()|| _workContext.CurrentCustomer.IsBackgroundTaskAccount())
                    return;

                //Customer与OpenId绑定
                var oauthSession = _httpContextAccessor.HttpContext.Session.Get<OauthSession>(NopWeixinDefaults.WeixinOauthSession);
                if (oauthSession != null && !string.IsNullOrEmpty(oauthSession.OpenId) && string.IsNullOrEmpty(_workContext.CurrentCustomer.OpenId))
                {
                    _workContext.CurrentCustomer.OpenId = oauthSession.OpenId;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                }

                //check request query parameters
                var request = context.HttpContext.Request;
                if (request?.Query == null || !request.Query.Any())
                    return;

                var openIds = request.Query[WUSER_OPENID_QUERY_PARAMETER_NAME];  //这里是推荐人OpenId
                var openIdsHash = request.Query[WUSER_OPENID_HASH_QUERY_PARAMETER_NAME]; //这里是推荐人OpenIdHash

                var openId = openIds.Any() ? openIds.FirstOrDefault() : string.Empty;
                var openIdHash = openIdsHash.Any() ? (long.TryParse(openIdsHash.FirstOrDefault(), out var hashResult) ? hashResult : 0) : 0;

                SetCustomerOpenId(openId, openIdHash);
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