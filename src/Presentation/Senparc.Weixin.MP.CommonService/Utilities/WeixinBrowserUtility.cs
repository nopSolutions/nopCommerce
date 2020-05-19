using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Senparc.Weixin.MP.CommonService.Utilities
{
    /// <summary>
    /// 浏览器公共类
    /// </summary>
    public static class WeixinBrowserUtility
    {
        /// <summary>
        /// 判断是否在微信内置浏览器中
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static bool SideInWeixinBrowser(this HttpContext httpContext)
        {
            var userAgent = CO2NET.Utilities.BrowserUtility.GetUserAgent(httpContext.Request);
            //判断是否在微信浏览器内部
            var isInWeixinBrowser = userAgent != null &&
                        (userAgent.Contains("MicroMessenger", StringComparison.InvariantCultureIgnoreCase)/*MicroMessenger*/ ||
                        userAgent.Contains("Windows Phone", StringComparison.InvariantCultureIgnoreCase)/*Windows Phone*/);
            return isInWeixinBrowser;
        }

        /// <summary>
        /// 判断是否在微信小程序内发起请求（注意：此方法在Android下有效，在iOS下暂时无效！）
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static bool SideInWeixinMiniProgram(this HttpContext httpContext)
        {
            var userAgent = CO2NET.Utilities.BrowserUtility.GetUserAgent(httpContext.Request);
            //判断是否在微信小程序的 web-view 组件内部
            var isInWeixinMiniProgram = userAgent != null && userAgent.Contains("miniProgram", StringComparison.InvariantCultureIgnoreCase)/*miniProgram*/;
            return isInWeixinMiniProgram;
        }
    }
}
