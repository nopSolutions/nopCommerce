using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Weixin;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Services.Security;
using Nop.Services.Weixin;

using Senparc.CO2NET.Extensions;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace Senparc.Weixin.MP.CommonService.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute that confirms access oauth2
    /// </summary>
    public sealed class CheckWeixinOAuthAttribute : TypeFilterAttribute
    {
        #region Fields

        private readonly bool _ignoreFilter;

        #endregion

        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        public CheckWeixinOAuthAttribute(bool ignore = false, string oauthRouteName = "", OAuthScope oauthScope = OAuthScope.snsapi_userinfo) : base(typeof(CheckWeixinOAuthFilter))
        {
            _ignoreFilter = ignore;
            Arguments = new object[] { ignore, oauthRouteName, oauthScope };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to ignore the execution of filter actions
        /// </summary>
        public bool IgnoreFilter => _ignoreFilter;

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that confirms access to public store
        /// </summary>
        private class CheckWeixinOAuthFilter : IAuthorizationFilter
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private readonly string _oauthRouteName;
            private readonly OAuthScope _oauthScope;
            private readonly WeixinSettings _weixinSettings;
            private readonly IUrlHelperFactory _urlHelperFactory;
            private readonly IWebHelper _webHelper;
            private readonly IStoreContext _storeContext;
            private readonly SenparcWeixinSetting _senparcWeixinSetting;
            #endregion

            #region Ctor

            public CheckWeixinOAuthFilter(bool ignoreFilter,
                string oauthRouteName,
                OAuthScope oauthScope,
                //ignore, oauthRouteName, oauthScope 
                WeixinSettings weixinSettings,
                IUrlHelperFactory urlHelperFactory,
                IWebHelper webHelper,
                IStoreContext storeContext,
                SenparcWeixinSetting senparcWeixinSetting
                )
            {
                _ignoreFilter = ignoreFilter;
                _oauthRouteName = oauthRouteName;
                _oauthScope = oauthScope;
                _weixinSettings = weixinSettings;
                _urlHelperFactory = urlHelperFactory;
                _webHelper = webHelper;
                _storeContext = storeContext;
                _senparcWeixinSetting = senparcWeixinSetting;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="filterContext">Authorization filter context</param>
            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                if (filterContext == null)
                    throw new ArgumentNullException(nameof(filterContext));

                //check whether this filter has been overridden for the Action
                var actionFilter = filterContext.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter).OfType<CheckWeixinOAuthAttribute>().FirstOrDefault();

                //ignore filter (the action is available even if navigation is not allowed)
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                //whether Oauth is enabled
                if (!_weixinSettings.ForcedAccessWeChatBrowser)
                    return;

                //whether check webbroswer before
                if (_weixinSettings.CheckWebBrowser && !Utilities.WeixinBrowserUtility.SideInWeixinBrowser(filterContext.HttpContext))
                {
                    var wechatBrowserControlerUrl = _urlHelperFactory.GetUrlHelper(filterContext).RouteUrl(NopWeixinDefaults.WechatBrowserControler);
                    filterContext.Result = new RedirectResult(wechatBrowserControlerUrl);
                }

                var session = filterContext.HttpContext.Session.Get<OauthSession>(NopWeixinDefaults.WeixinOauthSession);
                if (session != null && (session.CreatTime + 2160000) > Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now))  //RefreshToken30天有效期，在25天内刷新有效
                {
                    //oauth accesstoken 过期
                    if (session.CreatTime + 6600 < Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now))  //2小时过期，提前10分钟验证
                    {
                        try
                        {
                            var refreshResult = OAuthApi.RefreshToken(_senparcWeixinSetting.WeixinAppId, session.RefreshToken);
                            if (refreshResult.errcode == ReturnCode.请求成功)
                            {
                                session.AccessToken = refreshResult.access_token;
                                session.CreatTime = (int)Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now);
                                session.RefreshToken = refreshResult.refresh_token;

                                filterContext.HttpContext.Session.Set(NopWeixinDefaults.WeixinOauthSession, session);

                                return;
                            }
                        }
                        catch
                        {
                            //do nothing
                        }
                    }
                    else
                    {
                        return; //session的access token 没有过期，直接跳过验证
                    }
                }

                //初始化Oauth State
                var oauthStateString = ((_oauthScope == OAuthScope.snsapi_userinfo) ? "userinfo" : "base") + SystemTime.Now.Ticks.ToString();

                //check request query parameters
                var request = filterContext.HttpContext.Request;
                if (request?.Query == null || !request.Query.Any())
                {
                    //do nothing
                }
                else
                {
                    var openIds = request.Query["openid"];  //这里是推荐人OpenId
                    var openIdsHash = request.Query["openidhash"]; //这里是推荐人OpenIdHash

                    var openId = openIds.Any() ? openIds.FirstOrDefault() : string.Empty;
                    var openIdHash = openIdsHash.Any() ? (long.TryParse(openIdsHash.FirstOrDefault(), out var hashResult) ? hashResult : 0) : 0;

                    if (!string.IsNullOrEmpty(openId))
                    {
                        oauthStateString += "_" + openId;
                    }
                    else if (openIdHash > 0)
                    {
                        oauthStateString += "_" + openIdHash.ToString();
                    }
                }

                //保存state
                filterContext.HttpContext.Session.SetString(NopWeixinDefaults.WeixinOauthStateString, oauthStateString);

                //TODO 仅前端页面才判断是用snapapibase还是userinfo

                //redirect callback url
                var oauthCallbackUrl = _webHelper.GetStoreLocation().TrimEnd('/') + _urlHelperFactory.GetUrlHelper(filterContext).RouteUrl(string.IsNullOrEmpty(_oauthRouteName) ? NopWeixinDefaults.WeixinOauthCallbackControler : _oauthRouteName);

                oauthCallbackUrl += "?returnUrl=" + _webHelper.GetThisPageUrl(true).UrlEncode();

                var url = OAuthApi.GetAuthorizeUrl(_senparcWeixinSetting.WeixinAppId, oauthCallbackUrl, oauthStateString, _oauthScope);
                
                filterContext.Result = new RedirectResult(url);
            }

            #endregion
        }

        #endregion
    }
}