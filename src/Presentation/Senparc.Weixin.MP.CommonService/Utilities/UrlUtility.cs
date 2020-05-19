using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Senparc.CO2NET.Extensions;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Senparc.Weixin.Exceptions;
using Senparc.CO2NET.Helpers;

namespace Senparc.Weixin.MP.CommonService.Utilities
{
    public class UrlUtility
    {
        /// <summary>
        /// 生成OAuth用的CallbackUrl参数（原始状态，未整体进行UrlEncode）
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="oauthCallbackUrl"></param>
        /// <returns></returns>
        public static string GenerateOAuthCallbackUrl(HttpContext httpContext, string oauthCallbackUrl)
        {
            if (httpContext.Request == null)
            {
                throw new WeixinNullReferenceException("httpContext.Request 不能为null！", httpContext);
            }

            var request = httpContext.Request;
            //var location = new Uri($"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}");
            //var returnUrl = location.AbsoluteUri; //httpContext.Request.Url.ToString();    
            var returnUrl = request.AbsoluteUri();
            var urlData = httpContext.Request;
            var scheme = urlData.Scheme;//协议
            var host = urlData.Host.Host;//主机名（不带端口）
            var port = urlData.Host.Port ?? -1;//端口（因为从.NET Framework移植，因此不直接使用urlData.Host）
            //var schemeUpper = scheme.ToUpper();//协议（大写）
            string baseUrl = httpContext.Request.PathBase;//子站点应用路径

            //授权回调字符串
            var callbackUrl = Senparc.Weixin.HttpUtility.UrlUtility.GenerateOAuthCallbackUrl(scheme, host, port, baseUrl, returnUrl, oauthCallbackUrl);
            return callbackUrl;
        }
    }
}
