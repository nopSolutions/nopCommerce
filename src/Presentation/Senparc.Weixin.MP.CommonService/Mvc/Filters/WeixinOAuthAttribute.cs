using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Weixin;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Weixin;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace Senparc.Weixin.MP.CommonService.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute enabling Weixin OAuth validation
    /// </summary>
    public sealed class WeixinOAuthAttribute : TypeFilterAttribute
    {
        #region Fields

        private readonly bool _ignoreFilter;
        private readonly string _appIdFilter;
        private readonly string _oauthCallbackUrlFilter;
        private readonly OAuthScope _oauthScopeFilter;

        #endregion

        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public WeixinOAuthAttribute(bool ignore = false, string appId = null, string oauthCallbackUrl = null, OAuthScope oauthScope = OAuthScope.snsapi_userinfo)
            : base(typeof(WeixinOAuthFilter))
        {
            _ignoreFilter = ignore;
            _appIdFilter = appId ?? "获取默认配置的appId";
            _oauthCallbackUrlFilter = oauthCallbackUrl;
            _oauthScopeFilter = oauthScope;
            Arguments = new object[] { ignore, appId, oauthCallbackUrl, oauthScope };
        }

        #endregion

        #region Properties

        public bool IgnoreFilter => _ignoreFilter;
        /// <summary>
        /// AppId
        /// </summary>
        public string AppIdFilter => _appIdFilter;
        /// <summary>
        /// 跳转页面
        /// </summary>
        public string OauthCallbackUrlFilter => _oauthCallbackUrlFilter;
        /// <summary>
        /// OAuthScope类型
        /// </summary>
        public OAuthScope OauthScopeFilter => _oauthScopeFilter;

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter enabling Weixin OAuth validation
        /// </summary>
        private class WeixinOAuthFilter : IAuthorizationFilter
        {
            #region Fields
            private readonly bool _ignoreFilter;
            private readonly string _appIdFilter;
            private readonly string _oauthCallbackUrlFilter;
            private readonly OAuthScope _oauthScopeFilter;

            private readonly ILogger _logger;
            private readonly IWebHelper _webHelper;
            private readonly IUrlHelperFactory _urlHelperFactory;
            private readonly WeixinSettings _weixinSettings;

            #endregion

            #region Ctor

            public WeixinOAuthFilter(
                bool ignoreFilter,
                string appIdFilter,
                string oauthCallbackUrlFilter,
                OAuthScope oauthScopeFilter,  //前4个参数顺序不能变
                ILogger logger,
                IWebHelper webHelper,
                IUrlHelperFactory urlHelperFactory,
                WeixinSettings weixinSettings)
            {
                _ignoreFilter = ignoreFilter;
                _appIdFilter = appIdFilter;
                _oauthCallbackUrlFilter = oauthCallbackUrlFilter;
                _oauthScopeFilter = oauthScopeFilter;
                _logger = logger;
                _webHelper = webHelper;
                _urlHelperFactory = urlHelperFactory;
                _weixinSettings = weixinSettings;
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

                //check whether this filter has been overridden for the action
                var actionFilter = filterContext.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter).OfType<WeixinOAuthAttribute>().FirstOrDefault();

                //ignore filter (the action is available even if a customer hasn't access to the area)
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                //there is WeixinOAuthFilter, so check access
                if (filterContext.Filters.Any(filter => filter is WeixinOAuthFilter))
                {
                    //whether Oauth is enabled
                    if (!_weixinSettings.ForcedAccessWeChatBrowser)
                        return;

                    //whether check webbroswer before
                    if (_weixinSettings.CheckWebBrowser && !Utilities.WeixinBrowserUtility.SideInWeixinBrowser(filterContext.HttpContext))
                    {
                        var wechatBrowserControlerUrl = _urlHelperFactory.GetUrlHelper(filterContext).RouteUrl(NopWeixinDefaults.WechatBrowserControler);
                        filterContext.Result = new RedirectResult(wechatBrowserControlerUrl);
                    }

                    //has oauth logined
                    if (filterContext.HttpContext.Session != null)
                    {
                        var session = filterContext.HttpContext.Session.Get<OauthSession>(NopWeixinDefaults.WeixinOauthSession);
                        if (session != null &&
                            (session.CreatTime + 2160000) < Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now)  //RefreshToken30天有效期，在25天开始验证
                            )
                        {
                            //oauth accesstoken 过期
                            if (session.CreatTime + 6600 < Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now))  //2小时过期，七天10分钟验证
                            {
                                try
                                {
                                    var refreshResult = OAuthApi.RefreshToken(_appIdFilter, session.RefreshToken);
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
                                return;
                            }

                        }
                    }

                    //初始化Oauth State
                    var oauthStateString = string.Empty;
                    if (_oauthScopeFilter == OAuthScope.snsapi_userinfo)
                        oauthStateString = "userinfo_" + SystemTime.Now.Ticks.ToString();
                    else
                        oauthStateString = "base_" + SystemTime.Now.Ticks.ToString();

                    //保存state
                    filterContext.HttpContext.Session.SetString(NopWeixinDefaults.WeixinOauthStateString, oauthStateString);

                    //TODO 仅前端页面才判断是用snapapibase还是userinfo

                    //redirect callback url
                    var oauthCallbackUrl = _urlHelperFactory.GetUrlHelper(filterContext).RouteUrl(NopWeixinDefaults.WeixinOauthCallbackControler);
                    oauthCallbackUrl += "?returnUrl=" + _webHelper.GetThisPageUrl(true);//.UrlEncode();

                    var url = OAuthApi.GetAuthorizeUrl(_appIdFilter, oauthCallbackUrl, oauthStateString, _oauthScopeFilter);
                    filterContext.Result = new RedirectResult(url);
                }

            }

            #endregion
        }

        #endregion
    }
}