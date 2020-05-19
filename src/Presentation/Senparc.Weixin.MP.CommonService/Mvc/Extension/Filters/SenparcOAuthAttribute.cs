using System;
using System.Diagnostics.CodeAnalysis;
using Senparc.Weixin.MP.AdvancedAPIs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Senparc.Weixin.MP.CommonService.Utilities;

namespace Senparc.Weixin.MP.CommonService.Mvc.Extension.Filters
{
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Unsealed so that subclassed types can set properties in the default constructor or override our behavior.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public abstract class SenparcOAuthAttribute : ActionFilterAttribute,/* AuthorizeAttribute,*/ IAuthorizationFilter
    {
        protected string _appId { get; set; }
        protected string _oauthCallbackUrl { get; set; }
        protected OAuthScope _oauthScope { get; set; }

        /// <summary>
        /// AppId
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="oauthCallbackUrl">网站内路径（如：/TenpayV3/OAuthCallback），以/开头！当前页面地址会加在Url中的returlUrl=xx参数中</param>
        /// <param name="oauthScope">默认为 OAuthScope.snsapi_userinfo</param>
        public SenparcOAuthAttribute(string appId, string oauthCallbackUrl, OAuthScope oauthScope = OAuthScope.snsapi_userinfo)
        {
            _appId = appId;
            _oauthCallbackUrl = oauthCallbackUrl;
            _oauthScope = oauthScope;
        }

        /// <summary>
        /// 判断用户是否已经登录
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public abstract bool IsLogined(HttpContext httpContext);

        protected virtual bool AuthorizeCore(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            if (!IsLogined(httpContext))
            {
                return false;//未登录
            }

            return true;
        }

        public virtual void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (AuthorizeCore(filterContext.HttpContext))
            {

            }
            else
            {
                if (IsLogined(filterContext.HttpContext))
                {
                    //已经登录
                }
                else
                {
                    var callbackUrl = UrlUtility.GenerateOAuthCallbackUrl(filterContext.HttpContext, _oauthCallbackUrl);
                    var state = string.Format("{0}|{1}", "Nopcommerce", SystemTime.Now.Ticks);

                    var url = OAuthApi.GetAuthorizeUrl(_appId, callbackUrl, state, _oauthScope);
                    filterContext.Result = new RedirectResult(url/*, true*/);
                }
            }
        }
    }
}
