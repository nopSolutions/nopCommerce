using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Data;
using Senparc.Weixin.MP.CommonService.Utilities;

namespace Senparc.Weixin.MP.CommonService.Middleware
{
    /// <summary>
    /// Represents middleware that checks whether database is installed and redirects to weixin URL in otherwise
    /// </summary>
    public class KeepWeixinMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Ctor

        public KeepWeixinMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke middleware actions
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="webHelper">Web helper</param>
        /// <returns>Task</returns>
        public async Task Invoke(HttpContext context, IWebHelper webHelper, IWorkContext workContext)
        {
            //whether database is installed
            if (DataSettingsManager.DatabaseIsInstalled)
            {
                var adminUrl = $"{webHelper.GetStoreLocation()}Admin";//排除Admin路径
                var weixinUrl = $"{webHelper.GetStoreLocation()}Weixin";//排除路径weixin消息接收路径
                var wechatBrowserNoticeUrl = $"{webHelper.GetStoreLocation()}WechatBrowser";//排除非微信浏览器跳转提示路径

                var userUrl = $"{webHelper.GetStoreLocation()}user";

                var checkWebBrowser = true;
                if (checkWebBrowser)
                {
                    //排除路径
                    if (webHelper.GetThisPageUrl(false).StartsWith(weixinUrl, StringComparison.InvariantCultureIgnoreCase) ||
                    webHelper.GetThisPageUrl(false).StartsWith(adminUrl, StringComparison.InvariantCultureIgnoreCase) ||
                    webHelper.GetThisPageUrl(false).StartsWith(wechatBrowserNoticeUrl, StringComparison.InvariantCultureIgnoreCase)
                    )
                    {
                        //do nothing
                    }
                    else
                    {
                        //TODO 初步检查是否微信浏览器，如果没有通过，跳转到提示页
                        if (!context.SideInWeixinBrowser())
                        {
                            //跳转到提示信息页面，可以生成页面二维码图片
                            //redirect
                            context.Response.Redirect(wechatBrowserNoticeUrl);
                            return;
                        }

                        if (webHelper.GetThisPageUrl(false).StartsWith(userUrl, StringComparison.InvariantCultureIgnoreCase))
                        {
                            //TODO 配置User目录需要微信浏览器且是userinfo验证，已经授权，则跳过验证

                        }
                        else
                        {
                            //TODO 配置前端路径是否微信浏览器(且配置是base验证或userinfo验证)，已经授权，则跳过验证

                        }

                    }
                }
            }

            //or call the next middleware in the request pipeline
            await _next(context);
        }

        #endregion
    }
}